using Application.IServices;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infrastructure.Services.Helpers
{
    public class HttpWrapper : IHttpWrapper
    {
        private HttpClient _client;

        public HttpWrapper(HttpClient client)
        {
            _client = client;
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return _client.GetAsync(requestUri);
        }
    }
}
