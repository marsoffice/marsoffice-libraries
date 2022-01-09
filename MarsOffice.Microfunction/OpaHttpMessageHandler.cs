using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace MarsOffice.Microfunction
{
    public class OpaHttpMessageHandler : DelegatingHandler
    {
        private readonly string _opaToken;

        public OpaHttpMessageHandler(string opaToken)
        {
            _opaToken = opaToken;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_opaToken))
            {
                request.RequestUri = new Uri(QueryHelpers.AddQueryString(request.RequestUri.ToString(), "code", _opaToken));
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}