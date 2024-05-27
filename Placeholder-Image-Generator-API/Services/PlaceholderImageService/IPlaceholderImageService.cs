using Placeholder_Image_Generator_API.Models;

namespace Placeholder_Image_Generator_API.Services.PlaceholderImageService
{
    public interface IPlaceholderImageService
    {
        /*For Testing*/
        Task<ServiceResponse<GeneratedImage>> GetPlacegolderImageWithDefaultValues();

    }
}
