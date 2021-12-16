using System.Net;

namespace AspNetCoreWebApiProjManager.Test
{
    public class ErrorModel
    {
        public HttpStatusCode ERROR_CODE { get; set; }
        public string ERROR_MSG { get; set; }
    }
}
