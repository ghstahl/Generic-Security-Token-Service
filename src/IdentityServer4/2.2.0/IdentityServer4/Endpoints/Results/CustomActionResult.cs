using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace IdentityServer4.Endpoints.Results
{
    public class CustomJsonResult : JsonResult
    {
        public bool SetNoCache { get; set; }
        public int? SetCache { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public CustomJsonResult(object value) : base(value)
        {
        }

        public CustomJsonResult(object value, JsonSerializerSettings serializerSettings) : base(value,
            serializerSettings)
        {
        }

        public override void ExecuteResult(ActionContext context)
        {
            base.ExecuteResult(context);
            var httpContext = context.HttpContext;
            if (SetNoCache)
            {
                httpContext.Response.SetNoCache();
            }

            if (SetCache != null)
            {
                httpContext.Response.SetCache((int) SetCache);
            }
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            await base.ExecuteResultAsync(context);
            var httpContext = context.HttpContext;
            if (SetNoCache)
            {
                httpContext.Response.SetNoCache();
            }
            if (SetCache != null)
            {
                httpContext.Response.SetCache((int)SetCache);
            }
        }
    }

    public class CustomActionResult<T>: ActionResult where T:ActionResult
    {
        private T _inner;

        public CustomActionResult(T inner)
        {
            _inner = inner;
        }
        public bool SetNoCache { get; set; }
        public int? StatusCode { get; set; }
        public int? SetCache { get; set; }
        public Dictionary<string,string> Headers { get; set; }
        public override void ExecuteResult(ActionContext context)
        {
            if (_inner != null)
            {
                _inner.ExecuteResult(context);
            }

            var httpContext = context.HttpContext;
            if (SetNoCache)
            {
                httpContext.Response.SetNoCache();
            }

            if (StatusCode != null)
            {
                httpContext.Response.StatusCode = (int) StatusCode;
            }

            if (SetCache != null)
            {
                httpContext.Response.SetCache((int)SetCache);
            }
          
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (_inner != null)
            {
                await _inner.ExecuteResultAsync(context);
            }

            var httpContext = context.HttpContext;
            if (SetNoCache)
            {
                httpContext.Response.SetNoCache();
            }

            if (StatusCode != null)
            {
                httpContext.Response.StatusCode = (int)StatusCode;
            }

            if (SetCache != null)
            {
                httpContext.Response.SetCache((int)SetCache);
            }
        }
    }
}