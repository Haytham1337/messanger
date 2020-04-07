using Application.IServices;
using Domain.Exceptions.UserExceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PhotoHelper : IPhotoHelper
    {
        private readonly IConfiguration _config;

        private readonly IWebHostEnvironment _env;

        private readonly HttpClient _client;

        public PhotoHelper(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;

            _env = env;

            _client = new HttpClient();
        }



        public async Task<string> SavePhotoAsync(IFormFile uploadedFile)
        {
            var ext = uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf('.'));

            if (_config[$"PhotoExtensions:{ext}"] != null)
            {
                CloudStorageAccount storageAccount = CloudStorageAccount
                    .Parse(_config.GetValue<string>("storageKey"));

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("imgcontainer");

                var photo = $"{this.GeneratePhotoName()}{uploadedFile.Name}";

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(photo);

                await blockBlob.UploadFromStreamAsync(uploadedFile.OpenReadStream());

                return photo;
            }

            throw new PhotoInCorrectException("Given extension is incorrect!!", 400);
        }

        public async Task<string> SavePhotoFromUriAsync(string uri)
        {
            var httpResponce = await _client.GetAsync(uri);

            if (httpResponce.IsSuccessStatusCode)
            {
                CloudStorageAccount storageAccount = CloudStorageAccount
                    .Parse(_config.GetValue<string>("storageKey"));

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("imgcontainer");

                var photo = uri.Split('/')[3] + ".jpg";

                using var responseStream = await httpResponce.Content.ReadAsStreamAsync();

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(photo);

                await blockBlob.UploadFromStreamAsync(responseStream);

                return photo;
            }

            throw new PhotoInCorrectException("Given photo is incorrect!!", 400);
        }

        public async Task DeletePhotoAsync(string photo)
        {
            if (!string.IsNullOrEmpty(photo))
            {
                CloudStorageAccount storageAccount = CloudStorageAccount
                       .Parse(_config.GetValue<string>("storageKey"));

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("imgcontainer");

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(photo);

                await blockBlob.DeleteAsync();
            }
        }

        public string GeneratePhotoName(int size = 15)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber).Replace('+', 'p');
            }
        }
    }
}
