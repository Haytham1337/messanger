using Application.IServices;
using Domain.Exceptions.UserExceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PhotoHelper : IPhotoHelper
    {
        private readonly IConfiguration _config;

        private readonly IWebHostEnvironment _env;

        private readonly IHttpHelper _client;

        public PhotoHelper(IConfiguration config, IWebHostEnvironment env,IHttpHelper client)
        {
            _config = config;

            _env = env;

            _client = client;
        }

        [Obsolete]
        public async Task<string> SavePhotoAsync(IFormFile uploadedFile)
        {
            var ext = uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf('.'));

            if (_config[$"PhotoExtensions:{ext}"] != null)
            {
                var photo = $"{this.GeneratePhotoName()}{uploadedFile.Name}";

                var path = $"{_env.WebRootPath}\\avatars\\{photo}";

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
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

        public string GeneratePhotoName(int size = 15)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
