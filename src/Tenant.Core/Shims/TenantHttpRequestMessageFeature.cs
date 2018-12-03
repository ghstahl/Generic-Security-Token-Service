using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace Tenant.Core.Shims
{
    public class TenantHttpRequestMessageFeature 
    {
        private HttpRequestMessage _httpRequestMessage;
        private HttpRequest _httpRequest;

        public TenantHttpRequestMessageFeature(HttpRequest httpRequest)
        {
            if (httpRequest == null)
                throw new ArgumentNullException(nameof(httpRequest));
            _httpRequest = httpRequest;
        }

        public HttpRequestMessage HttpRequestMessage
        {
            get
            {
                return _httpRequestMessage ?? (_httpRequestMessage =
                           CreateHttpRequestMessage(_httpRequest));
            }
            set
            {
                _httpRequestMessage = value;
            }
        }

        private static HttpRequestMessage CreateHttpRequestMessage(HttpRequest request)
        {
            var requestUri = request.Scheme + "://" + request.Host + request.PathBase + request.Path + request.QueryString;
            var httpRequestMessage = new HttpRequestMessage(new HttpMethod(request.Method), requestUri)
            {
                Content = new StreamContent(request.Body)
            };
            foreach (var header in request.Headers)
            {
                if (!httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, (IEnumerable<string>)header.Value))
                    httpRequestMessage.Content.Headers.TryAddWithoutValidation(header.Key, (IEnumerable<string>)header.Value);
            }
            return httpRequestMessage;
        }
    }
}
