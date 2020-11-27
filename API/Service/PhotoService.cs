using System.IO;
using System.Threading.Tasks;
using API.Helper;
using API.Interface;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.Service
{
    public class PhotoService : IPhotoService
    {
        private readonly IOptions<CloudinarySettings> _config;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            _config = config;
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            var account = new Account(_config.Value.CloudName, _config.Value.ApiKey, _config.Value.ApiSecret);
            Cloudinary cloudinary = new Cloudinary(account);
            if(file.Length >0){
                using var filestream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, filestream),
                    Transformation = new Transformation().Height(500).Width(500).Gravity("face").Crop("fill")
                };
                uploadResult = await cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }
        
        public async Task<DeletionResult>  DeletePhotoAsync(string photoId)
        {
            var account = new Account(_config.Value.CloudName, _config.Value.ApiKey, _config.Value.ApiSecret);
            Cloudinary cloudinary = new Cloudinary(account);
            var deleteResult = await cloudinary.DestroyAsync(new DeletionParams(photoId));
            return deleteResult;
        }
    }
}