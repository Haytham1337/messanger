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

        public  Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return  _client.GetAsync(requestUri);
        }
    }
}
