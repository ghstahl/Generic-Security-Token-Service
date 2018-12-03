using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Tenant.Core
{
    public class TenantHostLoggerProvider : ILoggerProvider
    {
        public class TenantLogger : ILogger
        {
            private readonly string _categoryName;
            private string _tenant;

            public TenantLogger(string tenant,string categoryName, ILogger logger)
            {
                _tenant = tenant;
                _categoryName = categoryName;
                _logger = logger;
            }
           
            public void Log<TState>(
                LogLevel logLevel, 
                EventId eventId, 
                TState state, 
                Exception exception, 
                Func<TState, Exception, string> formatter)
            {
                if (!IsEnabled(logLevel))
                {
                    return;
                }
                _logger.Log(logLevel,eventId,state,exception, (st, ex) =>
                {
                    return $"tenant[{_tenant}]: {formatter(state, exception)}";
                });

             }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
        }

        private readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();
        private static ILogger _logger;
        private string _tenant;

        public TenantHostLoggerProvider(string tenant,ILogger logger)
        {
            _tenant = tenant;
            _logger = logger;
        }
        public TenantHostLoggerProvider()
        {
        }
        public ILogger CreateLogger(string categoryName)
        {
            ILogger logger = null;
            if (_loggers.TryGetValue(categoryName, out logger))
            {
                return logger;
            }
            logger = new TenantLogger(_tenant,categoryName, _logger);
            return _loggers.GetOrAdd(categoryName, name => logger);
        }

     

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}