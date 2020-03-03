using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace Application.Models.PhotoDto
{
    public class AddPhotoDto
    {
        public int Id { get; set; }

        [Required]
        public IFormFile UploadedFile { get; set; }
    }
}
