using Placeholder_Image_Generator_API.Models;
using System.Drawing;
using System.Drawing.Imaging;

namespace Placeholder_Image_Generator_API.Services.PlaceholderImageService
{
    /// <summary>
    /// Service for Generating placegolder images based on provided parameters
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
        /// Defines the background color of the image
        /// </summary>
        public string BackgroundColor { get; set; }
        /// <summary>
        /// Defines the text color that will be placed on the image (if requested)
        /// </summary>
        public string TextColor { get; set; }
        /// <summary>
        /// Defines the MIME type for the image
        /// </summary>
        public string ImageType { get; set; }
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
