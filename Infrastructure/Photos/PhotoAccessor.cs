using System;
using Application.Interfaces;
using Application.Photos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Photos {
    public class PhotoAccessor : IPhotoAccessor {

        private readonly Cloudinary _cloudinary;

        //Gets access to strongly typed Cloudinary settings in "config"
        public PhotoAccessor(IOptions<CloudinarySettings> config) {
            //set the account
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            //grants access to fields
            _cloudinary = new Cloudinary(acc);
        }

        public PhotoUploadResult AddPhoto(IFormFile file) {

            //create instance of ImageUploadResult
            var uploadResult = new ImageUploadResult();

            //make sure file exists
            if (file.Length > 0) {
                //read file into memory
                //using statement disposes after completion
                using(var stream = file.OpenReadStream()) {
                    //telling cloudinary what we are passing up to it...a file
                    var uploadParams = new ImageUploadParams {
                    //the file to send
                    File = new FileDescription(file.FileName, stream)
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            //check for errors
            if (uploadResult.Error != null)
                throw new Exception(uploadResult.Error.Message);

            return new PhotoUploadResult {
                PublicId = uploadResult.PublicId,
                    Url = uploadResult.SecureUri.AbsoluteUri
            };
        }

        public string DeletePhoto(string publicId) {
            throw new System.NotImplementedException();
        }
    }
}