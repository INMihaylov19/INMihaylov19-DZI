using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Data.Data;
using TMS.Services.Contracts;

namespace TMS.Services.Implementations
{
    public class ImageService : IImageService
    {
        private readonly TMSContext _context;
        public ImageService(TMSContext context)
        {
            _context = context;
        }
        public async Task<byte[]> GetImageDataAsync(string imageId)
        {
            var image = await _context
                .Images
                .Where(i => i.Id == imageId)
                .Select(i => i.Data)
                .FirstOrDefaultAsync();

            if (image == null)
            {
                throw new ArgumentException("Снимката не беше намерена");
            }

            return image;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string userId)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Няма селектиран файл");
            }

            var image = new Data.Models.Image
            {
                Id = Guid.NewGuid().ToString(),
                FileName = file.FileName,
                ContentType = file.ContentType,
            };

            using (var stream = file.OpenReadStream())
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    image.Data = ms.ToArray();
                }
            }

            _context.Images.Add(image);

            await _context.SaveChangesAsync();

            return image.Id;
        }
    }
}
