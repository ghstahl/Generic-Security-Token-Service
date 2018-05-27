using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace P7.Core.Scheduler.Scheduling
{
    public interface IScheduledTask
    {
        string Schedule { get; }
    }
}
