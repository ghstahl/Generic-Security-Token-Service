using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace P7.Core.Cache
{
    public class ExpirableObjectCache<T> where T : class
    {
        private SemaphoreSlim _writeLock;

        private T _object;

        public TimeSpan Expiration { get; set; }

        public DateTime? LastUpdate { get; internal set; }


        public delegate Task<T> Refresh();
        public delegate Task<T> RefreshWithHttpContext(HttpContext httpContext,IServiceProvider serviceProvider);
        private Refresh _refresh;
        private RefreshWithHttpContext _refreshWithHttpContext;
        public ExpirableObjectCache(TimeSpan expiration, Refresh refresher)
        {
            Expiration = expiration;
            _refresh = refresher;
            _writeLock = new SemaphoreSlim(1, 1);
        }
        public ExpirableObjectCache(TimeSpan expiration, RefreshWithHttpContext refreshWithHttpContext)
        {
            Expiration = expiration;
            _refreshWithHttpContext = refreshWithHttpContext;
            _writeLock = new SemaphoreSlim(1, 1);
        }
        public bool IsExpired
        {
            get { return !LastUpdate.HasValue || DateTime.Now - LastUpdate.Value > Expiration; }
        }

        public async Task<T> GetValueAsync()
        {
            await _writeLock.WaitAsync();
            try
            {
                if (IsExpired)
                {
                    _object = null;
                }

                if (_object == null)
                {
                    _object = await _refresh() as T;
                    LastUpdate = DateTime.Now;
                }

                return _object;
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                _writeLock.Release();
            }
        }
        public async Task<T> GetValueAsync(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            await _writeLock.WaitAsync();
            try
            {
                if (IsExpired)
                {
                    _object = null;
                }

                if (_object == null)
                {
                    _object = await _refreshWithHttpContext(httpContext, serviceProvider) as T;
                    LastUpdate = DateTime.Now;
                }

                return _object;
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                _writeLock.Release();
            }
        }
    }
}
