using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Services.Contracts
{
    public interface IImageService
    {
        public Task<string> UploadImageAsync(IFormFile file, string userId);
        public Task<byte[]> GetImageDataAsync(string imageId);
    }
}
