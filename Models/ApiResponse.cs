using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace MinimalAPI.Models
{
    public class ApiResponse
    {
        public ApiResponse() 
        {
            ErrorMessages = new List<string>();
        }
        public bool IsSuccessful {  get; set; }
        public object Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> ErrorMessages { get; set; } 
    }
}
