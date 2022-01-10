using System;
using System.Net.Http;
using MarsOffice.Microfunction;
using Polly;
using Polly.Extensions.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OpaExtensions
    {
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(r =>
                {
                    return r.Content.Headers.ContentLength.HasValue && r.Content.Headers.ContentLength.Value <= 2;
                })
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt));
        }

        public static IServiceCollection AddOpaClient(this IServiceCollection services, string opaBaseUrl, string opaToken)
        {
            services.AddHttpClient("OPA", (svc, hc) =>
                {
                    hc.BaseAddress = new Uri(opaBaseUrl);
                })
                .AddHttpMessageHandler((svc) => new OpaHttpMessageHandler(opaToken))
                .AddPolicyHandler(GetRetryPolicy());
            return services;
        }
    }
}