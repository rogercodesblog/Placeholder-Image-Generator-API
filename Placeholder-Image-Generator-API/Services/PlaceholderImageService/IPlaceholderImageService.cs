using Microsoft.AspNetCore.Mvc;
using Placeholder_Image_Generator_API.Models;

namespace Placeholder_Image_Generator_API.Services.PlaceholderImageService
{
    public interface IPlaceholderImageService
    {
        Task<ServiceResponse<GeneratedImage>> GetPlaceholderImageAsync(string sizeAndFormat, string Text);
        Task<ServiceResponse<GeneratedImage>> GetPlaceholderImageWithCustomBackgroundColorAsync(string sizeAndFormat, string Text, string backgroundColor );
        Task<ServiceResponse<GeneratedImage>> GetPlaceholderImageWithCustomBackgroundAndFontColorAsync(string sizeAndFormat, string Text, string backgroundColor, string fontColor);
    }
}
