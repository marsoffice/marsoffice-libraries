using System;
using System.IO;
using System.Net.Http;
using System.Text;
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
                    using var contentStream = r.Content.ReadAsStream();
                    if (contentStream == null || contentStream.Length == 0)
                    {
                        return false;
                    }
                    using var ms = new MemoryStream();
                    contentStream.CopyTo(ms);
                    var bytes = ms.ToArray();
                    var json = Encoding.UTF8.GetString(bytes)?.Trim();
                    if (string.IsNullOrEmpty(json))
                    {
                        return false;
                    }
                    if (json == "{}" || !json.ToLower().Contains("decision"))
                    {
                        return true;
                    }
                    if (contentStream.CanSeek)
                    {
                        try
                        {
                            contentStream.Seek(0, SeekOrigin.Begin);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                    return false;
                })
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
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