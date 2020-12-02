using Application.Photos;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces {
    //What we get back from Cloudinary is:
    //a imageUploadResult{} object - no dependency available to have access to that class
    //so we create this class to store that result
    //Need ID and URL
    public interface IPhotoAccessor {
        PhotoUploadResult AddPhoto(IFormFile file);

        string DeletePhoto(string publicId);
    };
}