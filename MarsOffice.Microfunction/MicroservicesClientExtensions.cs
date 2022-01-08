using System;
using System.Collections.Generic;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MicroservicesClientExtensions
    {
        public static IServiceCollection AddMicroserviceClients(this IServiceCollection services, IEnumerable<string> serviceNames, IConfiguration config)
        {
            foreach (var serv in serviceNames)
            {
                services.AddHttpClient(serv, (svc, hc) =>
                {
                    hc.BaseAddress = new Uri(config[$"{serv}_url"]);
                    hc.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", new[] { $"Bearer {GetToken(config)}" });
                });
            }
            return services;
        }

        private static string GetToken(IConfiguration config)
        {
            TokenCredential tokenCredential;
            var envVar = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT");
            var isDevelopmentEnvironment = string.IsNullOrEmpty(envVar) || envVar.ToLower() == "development";
            if (isDevelopmentEnvironment)
            {
                return "fakejwt";
            }
            tokenCredential = new DefaultAzureCredential();
            return tokenCredential.GetToken(
                new TokenRequestContext(scopes: new string[] { config["scope"] }),
                cancellationToken: System.Threading.CancellationToken.None
            ).Token;
        }
    }
}