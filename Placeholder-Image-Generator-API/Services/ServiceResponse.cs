namespace Placeholder_Image_Generator_API.Services
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; } = false;
        public bool IsInternalError { get; set; } = false;
        public string Message { get; set; }  = string.Empty;
    }
}
