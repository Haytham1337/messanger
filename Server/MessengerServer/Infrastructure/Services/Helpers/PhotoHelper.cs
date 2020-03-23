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

        private readonly IHttpHelper _client;

        [Obsolete]
        public PhotoHelper(IConfiguration config, IHostingEnvironment env,IHttpHelper client)
        {
            _config = config;

            _env = env;

            _client = client;
        }

        [Obsolete]
        public async Task<string> SavePhotoAsync(AddPhotoDto model)
        {
            var ext = model.UploadedFile.FileName.Substring(model.UploadedFile.FileName.LastIndexOf('.'));

            if (_config[$"PhotoExtensions:{ext}"] != null)
            {
                var photo = $"{model.UserId}{model.UploadedFile.Name}";

                var path = $"{_env.WebRootPath}\\avatars\\{photo}";

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await model.UploadedFile.CopyToAsync(fileStream);
                }

                return photo;
            }

            throw new PhotoInCorrectException("Given extension is incorrect!!", 400);
        }

        public async Task<string> SavePhotoFromUriAsync(string uri)
        {
            var httpResponce = await _client.GetAsync(uri);

            if (httpResponce.IsSuccessStatusCode)
            {
                var photoName = uri.Split('/')[3]+".jpg";

                var path = $"{_env.WebRootPath}\\avatars\\{photoName}";

                using var responseStream = await httpResponce.Content.ReadAsStreamAsync();

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await responseStream.CopyToAsync(fileStream);
                }

                return photoName;
            }

            throw new PhotoInCorrectException("Given photo is incorrect!!", 400);
        }
    }
}
