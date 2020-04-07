using System.Net.Http;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IHttpWrapper
    {
       Task<HttpResponseMessage> GetAsync(string requestUri);
    }
}
