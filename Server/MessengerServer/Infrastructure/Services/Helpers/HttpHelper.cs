using Application.IServices;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infrastructure.Services.Helpers
{
    public class HttpHelper:IHttpHelper
    {
        private HttpClient _client;

        public HttpHelper(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return await this._client.GetAsync(requestUri);
        }
    }
}
