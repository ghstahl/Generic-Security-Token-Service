using System.Threading;

namespace GenericSecurityTokenService.Modules
{
    public class MyContextAccessor : IMyContextAccessor
    {
        private static AsyncLocal<(string traceIdentifier, MyContext context)> _contextCurrent
            = new AsyncLocal<(string traceIdentifier, MyContext context)>();

        public MyContext MyContext
        {
            get
            {
                var value = _contextCurrent.Value;
                // Only return the context if the stored request id matches the stored trace identifier
                // context.TraceIdentifier is cleared by HttpContextFactory.Dispose.
                return value.traceIdentifier == value.context?.TraceIdentifier ? value.context : null;
            }
            set
            {
                _contextCurrent.Value = (value?.TraceIdentifier, value);
            }
        }
    }
}