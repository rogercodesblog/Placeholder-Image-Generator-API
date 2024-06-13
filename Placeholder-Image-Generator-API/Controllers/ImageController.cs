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
            _placeholderImageService = placeholderImageService;
        }

        #endregion

        #region Main API Endpoints

        [HttpGet("/{sizeandformat}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetImage(string sizeandformat, [FromQuery] string? text)
        {

            var _result = await _placeholderImageService.GetPlaceholderImageAsync(sizeandformat, text);

            if (_result.IsInternalError)
            {
                //Change Message
                return StatusCode(500, "Error");
            }

            if (_result.IsSuccess == false)
            {
                //Change Message
                return BadRequest("Bad");
            }

            return File(_result.Data.ImageBinaries, _result.Data.FileType);
        }

        [HttpGet("/{sizeandformat}/{backgroundcolor}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetImageWithCustomBackgroundColor(string sizeandformat, string backgroundcolor, [FromQuery] string? text)
        {

            var _result = await _placeholderImageService.GetPlaceholderImageWithCustomBackgroundColorAsync(sizeandformat, text, backgroundcolor);

            if (_result.IsInternalError)
            {
                //Change Message
                return StatusCode(500, "Error");
            }

            if (_result.IsSuccess == false)
            {
                //Change Message
                return BadRequest("Bad");
            }

            return File(_result.Data.ImageBinaries, _result.Data.FileType);
        }

        [HttpGet("/{sizeandformat}/{backgroundcolor}/{textcolor}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetImageWithCustomColors(string sizeandformat, string backgroundcolor,string textcolor , [FromQuery] string? text)
        {
            var _result = await _placeholderImageService.GetPlaceholderImageWithCustomBackgroundAndFontColorAsync(sizeandformat, text, backgroundcolor,textcolor);

            if (_result.IsInternalError)
            {
                //Change Message
                return StatusCode(500, "Error");
            }

            if (_result.IsSuccess == false)
            {
                //Change Message
                return BadRequest("Bad");
            }

            return File(_result.Data.ImageBinaries, _result.Data.FileType);
        }

        #endregion
    }
}
