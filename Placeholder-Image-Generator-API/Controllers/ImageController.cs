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
        #region Fields

        private readonly IPlaceholderImageService _placeholderImageService;

        #endregion

        #region Constructor

        public ImageController(IPlaceholderImageService placeholderImageService)
        {
            _placeholderImageService = placeholderImageService ;
        }

        #endregion

        /*For Testing*/
        //[HttpGet]
        //public async Task<ActionResult<ServiceResponse<GeneratedImage>>> GetBaseImage()
        //{
        //    var result = await _placeholderImageService.GetPlacegolderImageWithDefaultValues();
        //    if (result.IsSuccess == false)
        //    {
        //        return BadRequest("Something Bad Happened");
        //    }
        //    return File(result.Data.ImageBinaries,result.Data.FileType);
        //}

        #region Main API Endpoints

        [HttpGet("/{size-and-format}")]
        public IActionResult GetImage([FromQuery] string? text)
        {
            return Ok($"text is: {text}");
        }

        [HttpGet("/{size-and-format}/{background-color}")]
        public IActionResult GetImageWithCustomBackgroundColor([FromQuery] string? text)
        {
            return Ok($"text is: {text}");
        }

        [HttpGet("/{size-and-format}/{background-color}/{text-color}")]
        public IActionResult GetImageWithCustomColors([FromQuery] string? text)
        {
            return Ok($"text is: {text}");
        }

        #endregion
    }
}
