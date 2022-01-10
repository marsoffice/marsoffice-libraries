using System.IO;
using System.Net.Http;

namespace MarsOffice.Microfunction
{
    public static class HttpResponseMessageExtensions
    {
        public static HttpResponseMessage Clone(this HttpResponseMessage response)
        {
            var newResponse = new HttpResponseMessage(response.StatusCode);
            var ms = new MemoryStream();

            foreach (var v in response.Headers) newResponse.Headers.TryAddWithoutValidation(v.Key, v.Value);
            if (response.Content != null)
            {
                response.Content.CopyTo(ms, null, System.Threading.CancellationToken.None);
                ms.Position = 0;
                newResponse.Content = new StreamContent(ms);

                foreach (var v in response.Content.Headers) newResponse.Content.Headers.TryAddWithoutValidation(v.Key, v.Value);

            }
            return newResponse;
        }
    }
}