namespace Mashal.Helpers.Errors
{
    public class ApiException : ApiResponse
    {
        public ApiException(int statuscode, string? statusmessage = null, String? Details = null) : base(statuscode, statusmessage)
        {
        }

        public string? Details { get; set; }
    }
}

