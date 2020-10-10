namespace API.Errors
{
    public class ApiExceptions
    {   
        public int httpStatusCode { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public ApiExceptions(int httpStatusCode, string message = null, string details =null)
        {
            this.httpStatusCode = httpStatusCode;
            this.message = message;
            this.details = details;

        }
    }
}