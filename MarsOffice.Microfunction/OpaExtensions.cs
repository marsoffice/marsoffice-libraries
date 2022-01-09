using System;
using MarsOffice.Microfunction;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OpaExtensions
    {
        public static IServiceCollection AddOpaClient(this IServiceCollection services, string opaBaseUrl, string opaToken)
        {
            services.AddHttpClient("OPA", (svc, hc) =>
                {
                    hc.BaseAddress = new Uri(opaBaseUrl);
                }).AddHttpMessageHandler((svc) => new OpaHttpMessageHandler(opaToken));
            return services;
        }
    }
}