using Application.Models.PhotoDto;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IPhotoHelper
    {
        Task<string> SavePhotoAsync(AddPhotoDto model);

        Task<string> SavePhotoFromUriAsync(string uri);
    }
}
