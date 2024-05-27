using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Placeholder_Image_Generator_API.Models;
using Placeholder_Image_Generator_API.Services;
using Placeholder_Image_Generator_API.Services.PlaceholderImageService;

namespace Placeholder_Image_Generator_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IPlaceholderImageService _placeholderImageService;
        public ImageController(IPlaceholderImageService placeholderImageService)
        {
            _placeholderImageService = placeholderImageService ;
        }

        /*For Testing*/
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<GeneratedImage>>> GetBaseImage()
        {
            var result = await _placeholderImageService.GetPlacegolderImageWithDefaultValues();
            if (result.IsSuccess == false)
            {
                return BadRequest("Something Bad Happened");
            }
            return File(result.Data.ImageBinaries,result.Data.FileType);
        }
    }
}
