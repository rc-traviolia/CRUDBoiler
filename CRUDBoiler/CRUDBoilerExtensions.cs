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
        /// <summary>
        /// Creates a response for the the provided <see cref="HttpRequestData"/> that includes a message.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestData"/> for this response.</param>
        /// <param name="statusCode">The response status code.</param>
        /// <returns>The response data.</returns>
        public static HttpResponseData CreateResponse(this HttpRequestData httpRequest, HttpStatusCode httpStatusCode, string message)
        {
            var response = httpRequest.CreateResponse(httpStatusCode);
            response.Body = new StringContent(message).ReadAsStream();
            return response;
        }
    }
}
