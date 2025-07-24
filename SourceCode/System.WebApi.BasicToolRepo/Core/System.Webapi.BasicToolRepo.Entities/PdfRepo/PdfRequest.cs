using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace System.Webapi.BasicToolRepo.Entities.PdfRepo
{
    public class PdfRequest
    {
        [Required(ErrorMessage = "Files are required.")]
        [AllowedImageExtensions(ErrorMessage = "Only image files (jpg, jpeg, png, gif, bmp, webp) are allowed.")]
        public required IFormFileCollection Files { get; set; }
    }
}
