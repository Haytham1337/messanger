using System.Net.Http;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IHttpHelper
    {
       Task<HttpResponseMessage> GetAsync(string requestUri);
    }
}
