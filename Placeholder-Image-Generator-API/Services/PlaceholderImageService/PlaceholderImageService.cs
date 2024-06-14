using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Extensions;
using Placeholder_Image_Generator_API.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

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

        /// <summary>
        /// Generate image based on parameters
        /// </summary>
        /// <param name="sizeAndFormat">this parameter must have at least 1 side (ex: 600) or 2 (ex:400x600), the image format is optional, must be '.jpg' (default), '.png' or '.gif' (ex: 400x600.jpg).</param>
        /// <param name="text">The text that will be displayed on the image.</param>
        /// <returns>Service response with custom data type that includes; ImageBinaries as byte array and file MIME type.</returns>
        public async Task<ServiceResponse<GeneratedImage>> GetPlaceholderImageAsync(string sizeAndFormat, string text)
        {
            //ServiceResponse object to give information about the process
            ServiceResponse<GeneratedImage> _response = new ServiceResponse<GeneratedImage>();

            //try catch block, in case something goes wrong
            try
            {
                //Verify size and Format
                //this will also help us to know before hand if the string got
                //the required delimiters to work with it, avoiding errors in the future
                if (!IsProvidedFormatValid(sizeAndFormat))
                {
                    //if we get in here, it means the format is wrong, which means
                    //theres ir more than one 'x' or '.' in the provided string
                    //we exit early
                    _response.Message = "The provided format is not valid.";
                    return _response;
                }

                //Verify Text and Set Property Values
                if (!SetSizeAndFormat(sizeAndFormat))
                {
                    //if we get in here, it means the format is wrong, which means
                    //there is a a letter or some other thing in or between the numbers, ex; 64A instead of 640)
                    //we exit early
                    _response.Message = "The width/heigth size must be made of numbers only.";
                    return _response;
                }

                //Verify max size
                if (Width > MaxSideSize || Height > MaxSideSize)
                {
                    _response.Message = $"The width/height can't be greater than {MaxSideSize}.";
                    return _response;
                }

                //Verify min size
                if (Width <= 0 || Height <= 0)
                {
                    _response.Message = "The width/height must be at least 1px.";
                    return _response;
                }

                //Verify if user provided a text
                if (string.IsNullOrWhiteSpace(text))
                {
                    text = $"{Width}x{Height}";
                }

                //Verify text length
                if (text.Length > MaxTextLength)
                {
                    _response.Message = $"The text can't be longer than {MaxTextLength} letters.";
                    return _response;
                }

                //Generate data and place it on our _response object
                _response.Data = new GeneratedImage();
                _response.IsSuccess = true;
                _response.Data.FileType = ImageType;
                _response.Data.ImageBinaries = WriteTextOnImage(GenerateBaseImage(), text);
            }
            //We will catch an 'OverflowException' here
            //if the provided width or heigth is greater
            //than the max value of int when calling
            //'SetSizeAndFormat' method
            catch (OverflowException)
            {
                _response.Message = $"The width/height can't be greater than {MaxSideSize}.";
                return _response;
            }
            //This means we run into an issue while converting the data type
            //which means that the user provided an invalid format
            catch (FormatException)
            {
                _response.Message = "The provided format is not valid.";
                return _response;
            }
            //General Exception if something else goes wrong
            catch (Exception ex)
            {
                _response.IsInternalError = true;
                _response.Message = "There was an error generating the image, please try again.";
                return _response;
            }
            //If everything went as expected, we return the _response
            return _response;
        }

        /// <summary>
        /// Generate image based on parameters
        /// </summary>
        /// <param name="sizeAndFormat">this parameter must have at least 1 side (ex: 600) or 2 (ex:400x600), the image format is optional, must be '.jpg' (default), '.png' or '.gif' (ex: 400x600.jpg).</param>
        /// <param name="text">The text that will be displayed on the image.</param>
        /// <param name="backgroundColor">The background color must be either; a hex color (ex: #fff, the hash is optional) or a name "blue" (It's not case sensitive)</param>
        /// <returns>Service response with custom data type that includes; ImageBinaries as byte array and file MIME type.</returns>
        public async Task<ServiceResponse<GeneratedImage>> GetPlaceholderImageWithCustomBackgroundColorAsync(string sizeAndFormat, string text, string backgroundColor)
        {
            //ServiceResponse object to give information about the process
            ServiceResponse<GeneratedImage> _response = new ServiceResponse<GeneratedImage>();

            //try catch block, in case something goes wrong
            try
            {
                //Verify size and Format
                //this will also help us to know before hand if the string got
                //the required delimiters to work with it, avoiding errors in the future
                if (!IsProvidedFormatValid(sizeAndFormat))
                {
                    //if we get in here, it means the format is wrong, which means
                    //theres ir more than one 'x' or '.' in the provided string
                    //we exit early
                    _response.Message = "The provided format is not valid.";
                    return _response;
                }

                //Verify Text and Set Property Values
                if (!SetSizeAndFormat(sizeAndFormat))
                {
                    //if we get in here, it means the format is wrong, which means
                    //there is a a letter or some other thing in or between the numbers, ex; 64A instead of 640)
                    //we exit early
                    _response.Message = "The width/heigth size must be made of numbers only.";
                    return _response;
                }

                //Verify max size
                if (Width > MaxSideSize || Height > MaxSideSize)
                {
                    _response.Message = $"The width/height can't be greater than {MaxSideSize}.";
                    return _response;
                }

                //Verify min size
                if (Width <= 0 || Height <= 0)
                {
                    _response.Message = "The width/height must be at least 1px.";
                    return _response;
                }

                //Verify if user provided a text
                if (string.IsNullOrWhiteSpace(text))
                {
                    text = $"{Width}x{Height}";
                }

                //Verify text length
                if (text.Length > MaxTextLength)
                {
                    _response.Message = $"The text can't be longer than {MaxTextLength} letters.";
                    return _response;
                }

                //Verify Background Color (check if its hex or color name, then set)
                if (!SetBackgroundColor(backgroundColor))
                {
                    _response.Message = "The provided background color does not have a valid format";
                    return _response;
                }

                //Generate data and place it on our _response object
                _response.Data = new GeneratedImage();
                _response.IsSuccess = true;
                _response.Data.FileType = ImageType;
                _response.Data.ImageBinaries = WriteTextOnImage(GenerateBaseImage(), text);
            }
            //We will catch an 'OverflowException' here
            //if the provided width or heigth is greater
            //than the max value of int when calling
            //'SetSizeAndFormat' method
            catch (OverflowException)
            {
                _response.Message = $"The width/height can't be greater than {MaxSideSize}.";
                return _response;
            }
            //This means we run into an issue while converting the data type
            //which means that the user provided an invalid format
            catch (FormatException)
            {
                _response.Message = "The provided format is not valid.";
                return _response;
            }
            //General Exception if something else goes wrong
            catch (Exception ex)
            {
                _response.IsInternalError = true;
                _response.Message = "There was an error generating the image, please try again.";
                return _response;
            }
            //If everything went as expected, we return the _response
            return _response;
        }
        /// <summary>
        /// Generate image based on parameters
        /// </summary>
        /// <param name="sizeAndFormat">this parameter must have at least 1 side (ex: 600) or 2 (ex:400x600), the image format is optional, must be '.jpg' (default), '.png' or '.gif' (ex: 400x600.jpg).</param>
        /// <param name="text">The text that will be displayed on the image.</param>
        /// <param name="backgroundColor">The background color must be either; a hex color (ex: #fff, the hash is optional) or a name "blue" (It's not case sensitive)</param>
        /// <param name="fontColor">The font color must be either; a hex color (ex: #fff, the hash is optional) or a name "blue" (It's not case sensitive)</param>
        /// <returns>Service response with custom data type that includes; ImageBinaries as byte array and file MIME type.</returns>
        public async Task<ServiceResponse<GeneratedImage>> GetPlaceholderImageWithCustomBackgroundAndFontColorAsync(string sizeAndFormat, string text, string backgroundColor, string fontColor)
        {
            //ServiceResponse object to give information about the process
            ServiceResponse<GeneratedImage> _response = new ServiceResponse<GeneratedImage>();

            //try catch block, in case something goes wrong
            try
            {
                //Verify size and Format
                //this will also help us to know before hand if the string got
                //the required delimiters to work with it, avoiding errors in the future
                if (!IsProvidedFormatValid(sizeAndFormat))
                {
                    //if we get in here, it means the format is wrong, which means
                    //theres ir more than one 'x' or '.' in the provided string
                    //we exit early
                    _response.Message = "The provided format is not valid.";
                    return _response;
                }

                //Verify Text and Set Property Values
                if (!SetSizeAndFormat(sizeAndFormat))
                {
                    //if we get in here, it means the format is wrong, which means
                    //there is a a letter or some other thing in or between the numbers, ex; 64A instead of 640)
                    //we exit early
                    _response.Message = "The width/heigth size must be made of numbers only.";
                    return _response;
                }

                //Verify max size
                if (Width > MaxSideSize || Height > MaxSideSize)
                {
                    _response.Message = $"The width/height can't be greater than {MaxSideSize}.";
                    return _response;
                }

                //Verify min size
                if (Width <= 0 || Height <= 0)
                {
                    _response.Message = "The width/height must be at least 1px.";
                    return _response;
                }

                //Verify if user provided a text
                if (string.IsNullOrWhiteSpace(text))
                {
                    text = $"{Width}x{Height}";
                }

                //Verify text length
                if (text.Length > MaxTextLength)
                {
                    _response.Message = $"The text can't be longer than {MaxTextLength} letters.";
                    return _response;
                }

                //Verify Background Color (check if its hex or color name, then set)
                if (!SetBackgroundColor(backgroundColor))
                {
                    _response.Message = "The provided background color does not have a valid format";
                    return _response;
                }

                //Verify Font Color
                if(!SetFontColor(fontColor))
                {
                    _response.Message = "The provided font color does not have a valid format";
                    return _response;
                }

                //Generate data and place it on our _response object
                _response.Data = new GeneratedImage();
                _response.IsSuccess = true;
                _response.Data.FileType = ImageType;
                _response.Data.ImageBinaries = WriteTextOnImage(GenerateBaseImage(), text);
            }
            //We will catch an 'OverflowException' here
            //if the provided width or heigth is greater
            //than the max value of int when calling
            //'SetSizeAndFormat' method
            catch (OverflowException)
            {
                _response.Message = $"The width/height can't be greater than {MaxSideSize}.";
                return _response;
            }
            //This means we run into an issue while converting the data type
            //which means that the user provided an invalid format
            catch (FormatException)
            {
                _response.Message = "The provided format is not valid.";
                return _response;
            }
            //General Exception if something else goes wrong
            catch (Exception ex)
            {
                _response.IsInternalError = true;
                _response.Message = "There was an error generating the image, please try again.";
                return _response;
            }
            //If everything went as expected, we return the _response
            return _response;
        }

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
        /// Add text to an image
        /// </summary>
        /// <param name="imageBinaries">byte array containig the image data</param>
        /// <param name="text">the text that is going to be added to the image</param>
        /// <returns>image byte array data</returns>
        private byte[] WriteTextOnImage(byte[] imageBinaries, string text)
        {
            //We declare the bitmap 
            Bitmap bitmap;
            //and we create it with the provided iamge data
            using (MemoryStream ms = new MemoryStream(imageBinaries))
            {
                bitmap = new Bitmap(ms);
            }

            //we set the position in which we are going to write the text
            PointF textPosition = new PointF(bitmap.Width / 2, bitmap.Height / 2);

            //We create a Graphics object from our previously created bitmap
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                //Selecting font and size
                using (System.Drawing.Font _fontArial = new System.Drawing.Font("Arial", 12))
                {
                    //adds text to image
                    graphics.DrawString(text, _fontArial, Brushes.Black, textPosition);
                }
            }

            //we save the data and return it
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat);
                return ms.ToArray();
            }

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

        /// <summary>
        /// Verify and Set the font color from a provided value
        /// </summary>
        /// <param name="backgroundColor">font color, can either be a name "blue" or a hex value color "#fff" (with or without the hash)</param>
        /// <returns>returns true if the color format was correct and was set correctly</returns>

        private bool SetFontColor(string fontColor)
        {
            //We Verify if the fontColor string
            //contains a valid name like "red" or "blue"
            if (IsColorNameValid(fontColor))
            {
                //We set value from color name
                TextColor = GetColor(fontColor);
                return true;
            }

            //We verify if the fontColor string
            //is a hex color like "#fff", "#72962e", "000"
            bool _isColorHexNumber = int.TryParse(fontColor.StartsWith("#") ? fontColor.Substring(1) : fontColor, System.Globalization.NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out int ignoreThisVariable);

            if (_isColorHexNumber)
            {
                //We set value from Hex color code
                TextColor = fontColor.StartsWith("#") ? fontColor : fontColor.Insert(0, "#");
                return true;
            }

            //If we get in here, then the format isn't valid
            return false;
        }

        /// <summary>
        /// Verify and Set the background color from a provided value
        /// </summary>
        /// <param name="backgroundColor">Background color, can either be a name "blue" or a hex value color "#fff" (with or without the hash)</param>
        /// <returns>returns true if the color format was correct and was set correctly</returns>
        private bool SetBackgroundColor(string backgroundColor)
        {

            //We Verify if the backgroundColor string
            //contains a valid name like "red" or "blue"
            if (IsColorNameValid(backgroundColor))
            {
                //We set value from color name
                BackgroundColor = GetColor(backgroundColor);
                return true;
            }

            //We verify if the backgroundColor string
            //is a hex color like "#fff", "#72962e", "000"
            bool _isColorHexNumber = int.TryParse(backgroundColor.StartsWith("#") ? backgroundColor.Substring(1) : backgroundColor, System.Globalization.NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out int ignoreThisVariable);

            if (_isColorHexNumber)
            {
                //We set value from Hex color code
                BackgroundColor = backgroundColor.StartsWith("#") ? backgroundColor : backgroundColor.Insert(0, "#");
                return true;
            }

            //If we get in here, then the format isn't valid
            return false;
        }

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
        /// We set the hex color value from the provided Color object
        /// </summary>
        /// <param name="color">Color object</param>
        /// <returns>Hex value of the provided color</returns>
        private string ColorToHex(Color color)
        {
            return $"#{color.R.ToString("X2")}{color.G.ToString("X2")}{color.B.ToString("X2")}";
        }

        /// <summary>
        /// Verify if provided string is a valid color name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool IsColorNameValid(string name)
        {
            //Get all "Known Colors" 
            KnownColor[] _knownColors = Enum.GetValues(typeof(KnownColor)) as KnownColor[];

            //Create a new list with all the known color names
            //now normalized
            var _knownColorsNames = _knownColors.Select(asds => asds.GetDisplayName().ToLower()).ToList();

            //Verify if the name exist in the list
            if (!_knownColorsNames.Any(colorname => colorname.Equals(name.ToLower())))
            {
                return false;
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
        private bool SetSizeAndFormat(string format)
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
