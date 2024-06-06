using Microsoft.Extensions.Configuration;
using Placeholder_Image_Generator_API.Models;
using System.Drawing;
using System.Drawing.Imaging;

namespace Placeholder_Image_Generator_API.Services.PlaceholderImageService
{
    /// <summary>
    /// Service for Generating placeholder images based on provided parameters
    /// </summary>
    public class PlaceholderImageService : IPlaceholderImageService
    {
        #region Properties and Fields
        /// <summary>
        /// Defines the width of the image
        /// </summary>
        private int Width { get; set; }
        /// <summary>
        /// Defines the height of the image
        /// </summary>
        private int Height { get; set; }
        /// <summary>
        /// Defines the max size of width and heigth
        /// </summary>
        public int MaxSideSize { get; set; }
        /// <summary>
        /// Defines the max length of the image text
        /// </summary>
        public int MaxTextLength { get; set; }

        /// <summary>
        /// Defines the background color of the image
        /// </summary>
        private string BackgroundColor { get; set; }
        /// <summary>
        /// Defines the text color that will be placed on the image (if requested)
        /// </summary>
        private string TextColor { get; set; }
        /// <summary>
        /// Defines the MIME type
        /// </summary>
        private string ImageType { get; set; }
        /// <summary>
        /// Set the ImageFormat type required to create the image using System.Drawing namespace
        /// </summary>
        private ImageFormat ImageFormat { get; set; }
        /// <summary>
        /// Configuration field to access the appsettings.json data trough Dependency Injection
        /// </summary>
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        /// <summary>
        /// Service Costructor, use this to inject "_configuration" to get the default values from appsettings.json
        /// Also, calls the method "GetDefaultValues()" to set them
        /// </summary>
        /// <param name="configuration"></param>
        public PlaceholderImageService(IConfiguration configuration)
        {
            _configuration = configuration;
            GetDefaultValues();
        }
        #endregion

        #region Interface Methods

        public async Task<ServiceResponse<GeneratedImage>> GetPlaceholderImageAsync(string sizeAndFormat, string text)
        {
            //ServiceResponse object to give information about the process
            ServiceResponse<GeneratedImage> _response = new ServiceResponse<GeneratedImage>();

            //Verify size and Format
            //this will also help us to know before hand if the string got
            //the required delimiters to work with it, avoiding errors in the future
            if (IsProvidedFormatValid(sizeAndFormat))
            {
                //if we get in here, it means the format is wrong, return a bad request
            }

            //Verify Text and Set Property Values
            if (!VerifyAndSetSizeAndFormat(sizeAndFormat))
            {
                //if we get in here, it means the format is wrong, return a bad request
            }

            if( Width > MaxSideSize || Height > MaxSideSize )
            {
                //Size Limit
            }

            if(Width <=0|| Height <= 0)
            {
                //We required a minimum size of 1, (who would do this?)
            }

            if (text.Length > MaxTextLength)
            {
                //can't be longer than max length
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                text = $"{Width}x{Height}";
            }

            //Place Text on top of image here, before sending it

            _response.Data = new GeneratedImage();
            _response.IsSuccess = true;
            _response.Data.FileType = ImageType;
            _response.Data.ImageBinaries = GenerateBaseImage();

            return _response;
        }

        public Task<ServiceResponse<GeneratedImage>> GetPlaceholderImageWithCustomBackgroundColorAsync(string sizeAndFormat, string Text, string backgroundColor)
        {
            //ServiceResponse object to give information about the process
            ServiceResponse<GeneratedImage> response = new ServiceResponse<GeneratedImage>();

            //Verify size and Format
            //this will also help us to know before hand if the string got
            //the required delimiters to work with it, avoiding errors in the future
            if (IsProvidedFormatValid(sizeAndFormat))
            {
                //if we get in here, it means the format is wrong, return a bad request
            }

            //Verify Text and Set Property Values
            if (!VerifyAndSetSizeAndFormat(sizeAndFormat))
            {
                //if we get in here, it means the format is wrong, return a bad request
            }

            //Verify Background Color (check if its hex or color name, then set)

            //check if colorname is hex or name, or search the provided name in the colors enum

            //if it doesn't exists in the enum list then is a hex, verify if its hex (cuz something can happen)

            //put the color if its the color, otherwise put the default color

            //Generate Image Based on Value

            throw new NotImplementedException();
        }

        public Task<ServiceResponse<GeneratedImage>> GetPlaceholderImageWithCustomColorsAsync(string sizeAndFormat, string Text, string backgroundColor, string fontColor)
        {

            //Steps:

            //Verify size and Format

            //Verify Text

            //Verify Background Color (check if its hex or color name, then set)

            //Verify Text Color (check if its hex or color name, then set)

            //Generate Image Based on Value

            throw new NotImplementedException();
        }

        //public async Task<ServiceResponse<GeneratedImage>> GetPlacegolderImageWithDefaultValues()
        //{
        //    var imagebytes = GenerateBaseImage();

        //    ServiceResponse<GeneratedImage> _response = new ServiceResponse<GeneratedImage>()
        //    {
        //        Data = new GeneratedImage() {  ImageBinaries = new byte[imagebytes.Length] }
        //    };

        //    _response.IsSuccess = true;
        //    _response.Message = "Returned image bytes";
        //    //_response.Data.ImageBinaries = GenerateBaseImage();
        //    _response.Data.ImageBinaries = imagebytes;
        //    _response.Data.FileType = ImageType;

        //    return _response;
        //}


        #endregion

        #region Private Methods and Helpers

        /// <summary>
        /// Generates the base image based on provided width,height and background color
        /// </summary>
        /// <returns>Returns image binaries trough a memory stream (byte array)</returns>
        private byte[] GenerateBaseImage()
        {
            Bitmap bitmap = new Bitmap(Width, Height);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    bitmap.SetPixel(x, y, ColorTranslator.FromHtml(BackgroundColor));
                }
            }
            using MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat);
            return ms.ToArray();
        }


        /// <summary>
        /// Get default values from appsettings.json and set them on the properties on this very same service
        /// </summary>
        private void GetDefaultValues()
        {
            Width = _configuration.GetValue<int>("ImageGenerationSettings:DefaultWidth");
            Height = _configuration.GetValue<int>("ImageGenerationSettings:DefaultHeight");
            MaxSideSize = _configuration.GetValue<int>("ImageGenerationSettings:MaxSideSize");
            MaxTextLength = _configuration.GetValue<int>("ImageGenerationSettings:MaxTextLength");
            ImageType = _configuration.GetValue<string>("ImageGenerationSettings:DefaultImageType");
            ImageFormat = VerifyImageType(_configuration.GetValue<string>("ImageGenerationSettings:DefaultImageType"));
            BackgroundColor = GetColor(_configuration.GetValue<string>("ImageGenerationSettings:DefaultBackgroundColor"));
            TextColor = GetColor(_configuration.GetValue<string>("ImageGenerationSettings:DefaultTextColor"));
        }


#warning Verify color before getting it
        /// <summary>
        /// Enter color name (Ex: Red, Blue, etc) and returns the Hex version of it (internally calls "ColorToHex" Method)
        /// </summary>
        /// <param name="name">Color Name (Red, Blue, etc)</param>
        /// <returns>Color in hex value</returns>
        private string GetColor(string name)
        {
            return ColorToHex(Color.FromName(name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color">Color object</param>
        /// <returns>Hex value of the provided color</returns>
        private string ColorToHex(Color color)
        {
            return $"#{color.R.ToString("X2")}{color.G.ToString("X2")}{color.B.ToString("X2")}";
        }

#warning Debug This and see arrary values 
        private bool VerifyColor(string name)
        {
            var colors = Enum.GetValues(typeof(KnownColor));
            foreach (var color in colors)
            {
                color.Equals(name);
            }
            return true;
        }

        /// <summary>
        /// Verify if the provided string contains none or 1 'x' or '.' characters
        /// </summary>
        /// <param name="format">string to verify</param>
        /// <returns>True if its valid</returns>
        private bool IsProvidedFormatValid(string format)
        {

            //Checks if there is a space or white space
            //alternative syntax, but may not be as readable: 
            //if(format.Any(character=> Char.IsWhiteSpace(character)))
            if (format.Any(Char.IsWhiteSpace))
            {
                return false;
            }

            //Change Capital letters to "normal" letters, so if the user
            //provides a capital "X", it will change to "x"
            //this way we can detect the apropiate delimiter on the next method
            format = format.ToLower();

            //The character 'x' is used to separate width and heigth values
            //example: 600x400 , so if we get more than 1 'x'
            //it means our desired format is incorrect
            if (format.Count(character => character == 'x') > 1)
            {
                return false;
            }
            //The character '.' is used to give a format to the image
            //example: ".jpg", full example "/600x400.jpg", if we get 
            //more than 1 it means our desired format is incorrect
            if (format.Count(character => character == '.') > 1)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Checks if the provided size in string is made up of numbers only,
        /// this method is intended to be used after verification made by method
        /// IsProvidedFormatValid(string format)
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private bool VerifyAndSetSizeAndFormat(string format)
        {

            //Change Capital letters to "normal" letters, so if the user
            //provides a capital "X", it will change to "x"
            //this way we can detect the apropiate delimiter on the next method
            format = format.ToLower();

            string[] _baseSplittedString = new string[2];

            string _heigth = string.Empty;
            string _width = string.Empty;
            string _imageFormat = string.Empty;

            //We will use this as a flag for conditionals
            bool _formatContainsImageExtension = false;

            //check if the string contains a format or not
            //by verifying if there is a '.' in the string
            if (format.Any(character => character == '.'))
            {
                //if there is a '.'
                //split string in 2 pieces
                //[0] = sides ("640x480" or "640")
                //[1] = format (".jpg")
                _baseSplittedString = format.Split('.');
                _formatContainsImageExtension = true;
                _imageFormat = _baseSplittedString[1];
            }

            //then check if we should use the "_baseSplittedString" variable 
            //or the "format" parameter
            if (_formatContainsImageExtension)
            {
                //check if we have width and height
                //we can know if there is a delimiter 'x' (example: "640x480")
                if (format.Any(chararcter => chararcter == 'x'))
                {
                    //_formatContainsImageExtension is true here
                    //so it means that the string was splitted previously
                    //we should use [0] here in order to get width and height
                    var _sides = _baseSplittedString[0].Split('x');

                    //set the values
                    _width = _sides[0];
                    _heigth = _sides[1];
                }
                else
                {
                    //here we don't have two sides only one
                    //this means the figure is a square so we need
                    //the same length in both sides
                    _width = _heigth = _baseSplittedString[0];
                }
            }
            else
            {

                //check if we have width and height
                //we can know if there is a delimiter 'x' (example: "640x480")
                if (format.Any(character => character == 'x'))
                {
                    //Split this in half
                    var _sides = format.Split('x');
                    // set values
                    _width = _sides[0];
                    _heigth = _sides[1];
                }
                else
                {
                    //here we don't have two sides only one
                    //this means the figure is a square so we need
                    //the same length in both sides
                    _width = _heigth = format;
                }
            }

            //Check if there is a letter in any of the sizes
            if (_heigth.Any(character => !Char.IsDigit(character)) || _width.Any(character => !Char.IsDigit(character)))
            {
                //if there is, then the format is invalid
                return false;
            }

            Height = Convert.ToInt32(_heigth);
            Width = Convert.ToInt32(_width);
            if (_formatContainsImageExtension)
            {
                ImageFormat = VerifyImageType(_imageFormat);
            }

            return true;
        }
        /// <summary>
        /// Verifies the image type and set the value acording to MIME Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private ImageFormat VerifyImageType(string type)
        {
            switch (type)
            {
                case "png":
                    ImageType = "image/png";
                    return ImageFormat.Png;
                    ;
                case "gif":
                    ImageType = "image/gif";
                    return ImageFormat.Gif;

                case "jpg":
                case "jpeg":
                default:
                    ImageType = "image/jpeg";
                    return ImageFormat.Jpeg;

            }
        }


        #endregion
    }
}
