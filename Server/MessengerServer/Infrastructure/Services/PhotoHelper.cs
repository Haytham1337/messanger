using Application.IServices;
using Application.Models.PhotoDto;
using Domain.Exceptions.UserExceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PhotoHelper : IPhotoHelper
    {
        private readonly IConfiguration _config;
        [Obsolete]
        private readonly IHostingEnvironment _env;

        [Obsolete]
        public PhotoHelper(IConfiguration config, IHostingEnvironment env)
        {
            _config = config;

            _env = env;
        }

        [Obsolete]
        public async Task<string> SavePhotoAsync(AddPhotoDto model)
        {
            var ext = model.UploadedFile.FileName.Substring(model.UploadedFile.FileName.LastIndexOf('.'));

            if (this._config[$"PhotoExtensions:{ext}"] != null)
            {
                var photo = $"{model.UserId}{model.UploadedFile.Name}";

                var path = $"{_env.WebRootPath}\\avatars\\{photo}";

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await model.UploadedFile.CopyToAsync(fileStream);
                }

                return photo;
            }
            else
            {
                throw new PhotoInCorrectException("Given extension is incorrect!!", 400);
            }
        }
    }
}
