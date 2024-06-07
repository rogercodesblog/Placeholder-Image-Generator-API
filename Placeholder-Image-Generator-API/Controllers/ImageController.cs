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

        [HttpGet("/{sizeandformat}")]
        public async Task<ActionResult>GetImage(string sizeandformat , [FromQuery] string? text)
        {
            var _result = await _placeholderImageService.GetPlaceholderImageAsync(sizeandformat, text);
            return File(_result.Data.ImageBinaries, _result.Data.FileType);
        }

        [HttpGet("/{sizeandformat}/{backgroundcolor}")]
        public IActionResult GetImageWithCustomBackgroundColor([FromQuery] string? text)
        {
            return Ok($"text is: {text}");
        }

        [HttpGet("/{sizeandformat}/{backgroundcolor}/{textcolor}")]
        public IActionResult GetImageWithCustomColors([FromQuery] string? text)
        {
            return Ok($"text is: {text}");
        }

        #endregion
    }
}
