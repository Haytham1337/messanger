using Application.Models.PhotoDto;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IPhotoHelper
    {
        Task<string> SavePhotoAsync(IFormFile uploadedFile);

        Task<string> SavePhotoFromUriAsync(string uri);
    }
}
