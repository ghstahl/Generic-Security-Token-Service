using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using P7.Core.Scheduler.Scheduling;

namespace P7.Core.Scheduler
{
    public class SomeOtherTask : IScheduledTask
    {
        public string Schedule => "0 5 * * *";

        public async Task Invoke(CancellationToken cancellationToken)
        {
            await Task.Delay(5000, cancellationToken);
        }
    }
}