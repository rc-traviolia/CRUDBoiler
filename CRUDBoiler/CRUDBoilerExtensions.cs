using System.Net;
using System.Text.Json;
using Azure.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;


namespace CRUDBoiler
{
    public static class CRUDBoilerExtensions
    {
        public static HttpResponseData CreateResponse(this HttpRequestData httpRequest, HttpStatusCode httpStatusCode, string message)
        {
            var response = httpRequest.CreateResponse(httpStatusCode);
            response.Body = new StringContent(message).ReadAsStream();
            return response;
        }
    }
}
