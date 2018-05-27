using System.Linq;
using System.Reflection;

namespace P7.Core.Utils
{
    public interface IEventSource<T>  
    {
        void RegisterEventSink(T sink);
        void UnregisterEventSink(T sink);
        void UnregisterAll();
    }
}