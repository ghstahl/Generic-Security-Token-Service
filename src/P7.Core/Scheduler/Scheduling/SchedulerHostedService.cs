using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using P7.Core.Scheduler.Cron;

namespace P7.Core.Scheduler.Scheduling
{
    public class SchedulerHostedService : HostedService
    {
        public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

        private readonly List<SchedulerTaskWrapper> _scheduledTasks = new List<SchedulerTaskWrapper>();
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IEnumerable<IScheduledTask> enumerable;

        public SchedulerHostedService(IEnumerable<IScheduledTask> scheduledTasks, IServiceScopeFactory serviceScopeFactory)
        {
            var referenceTime = DateTime.UtcNow;

            foreach (var scheduledTask in scheduledTasks)
            {
                _scheduledTasks.Add(new SchedulerTaskWrapper
                {
                    Schedule = CrontabSchedule.Parse(scheduledTask.Schedule),
                    Task = scheduledTask,
                    NextRunTime = referenceTime
                });
            }

            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public SchedulerHostedService(IEnumerable<IScheduledTask> enumerable)
        {
            this.enumerable = enumerable;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ExecuteOnceAsync(cancellationToken);

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }

        private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            var referenceTime = DateTime.UtcNow;

            var tasksThatShouldRun = _scheduledTasks.Where(t => t.ShouldRun(referenceTime)).ToList();

            foreach (var taskThatShouldRun in tasksThatShouldRun)
            {
                taskThatShouldRun.Increment();

                await taskFactory.StartNew(
                    async () => {
                        try
                        {
                            using (var scope = _serviceScopeFactory.CreateScope())
                            {
                                var t = taskThatShouldRun.Task.GetType();
                                var method = t.GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public);
                                var arguments = method.GetParameters()
                                    .Select(a => a.ParameterType == typeof(CancellationToken) ? cancellationToken : scope.ServiceProvider.GetService(a.ParameterType))
                                    .ToArray();



                                //invoke.
                                if (typeof(Task).Equals(method.ReturnType))
                                {
                                    await (Task)method.Invoke(taskThatShouldRun.Task, arguments);
                                }
                                else
                                {
                                    method.Invoke(taskThatShouldRun.Task, arguments);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            var args = new UnobservedTaskExceptionEventArgs(
                                ex as AggregateException ?? new AggregateException(ex));

                            UnobservedTaskException?.Invoke(this, args);

                            if (!args.Observed)
                            {
                                throw;
                            }
                        }
                    },
                    cancellationToken);
            }
        }

        private class SchedulerTaskWrapper
        {
            public CrontabSchedule Schedule { get; set; }
            public IScheduledTask Task { get; set; }

            public DateTime LastRunTime { get; set; }
            public DateTime NextRunTime { get; set; }

            public void Increment()
            {
                LastRunTime = NextRunTime;
                NextRunTime = Schedule.GetNextOccurrence(NextRunTime);
            }

            public bool ShouldRun(DateTime currentTime)
            {
                return NextRunTime < currentTime && LastRunTime != NextRunTime;
            }
        }
    }
}