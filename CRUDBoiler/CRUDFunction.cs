using System.Net;
using System.Text.Json;
using Azure.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using CRUDBoiler;

namespace CRUDBoiler
{
    public abstract class CRUDFunction<TDataModel, TRequestModel> : ICRUDService<TDataModel, TRequestModel>
    {
        public const string EndpointName = "";
        private readonly ILogger _logger;

        public CRUDFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CRUDFunctionWithService<TDataModel, TRequestModel>>();
        }

        //[Function("Function1")]
        //public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        //{
        //    _logger.LogInformation("C# HTTP trigger function processed a request.");

        //    var response = req.CreateResponse(HttpStatusCode.OK);
        //    response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        //    response.WriteString("Welcome to Azure Functions!");
        //    var one = 1;
        //    return response;
        //}

        public virtual HttpResponseData IngestionPoint([HttpTrigger(AuthorizationLevel.Function, "put", "get", "patch", "delete")] HttpRequestData httpRequest, int? id)
        {
            switch (httpRequest.Method)
            {
                case "PUT":
                    return Create(httpRequest);
                case "GET":
                    return Read(httpRequest, id);
                case "PATCH":
                    return Update(httpRequest, id);
                case "DELETE":
                    return Delete(httpRequest, id);
                default:
                    return httpRequest.CreateResponse(HttpStatusCode.MethodNotAllowed);
            }
        }
        public virtual HttpResponseData Create([HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequestData httpRequest)
        {
            var bodyJson = new StreamReader(httpRequest.Body).ReadToEndAsync().Result;
            var requestBody = JsonSerializer.Deserialize<TRequestModel>(bodyJson);

            var createResponse = Create(requestBody);
            return httpRequest.CreateResponse(createResponse.HttpStatusCode);
        }
        public virtual HttpResponseData Read([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData httpRequest, int? id)
        {
            if (!id.HasValue)
            {
                return httpRequest.CreateResponse(HttpStatusCode.BadRequest, $"You must include an id. Example: myapiservice.com/api/{{route}}/{{id}}");
            }

            var createResponse = Read(id);
            return httpRequest.CreateResponse(createResponse.HttpStatusCode);
        }
        public virtual HttpResponseData Update([HttpTrigger(AuthorizationLevel.Function, "patch")] HttpRequestData httpRequest, int? id)
        {
            if (!id.HasValue)
            {
                return httpRequest.CreateResponse(HttpStatusCode.BadRequest, $"You must include an id. Example: myapiservice.com/api/{{route}}/{{id}}");
            }
            var requestBody = httpRequest.ReadFromJsonAsync<TRequestModel>();

            if (!requestBody.IsCompletedSuccessfully)
            {
                /*TODO: This does not fail when submitting an incorrect object. It will only fail, if I had to guess, when something other than JSON is sent*/
                return httpRequest.CreateResponse(HttpStatusCode.BadRequest, $"Failed to deserialize request body. {requestBody.Result}");
            }
            var createResponse = Update(id, requestBody.Result);
            return httpRequest.CreateResponse(createResponse.HttpStatusCode);
        }
        public virtual HttpResponseData Delete([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequestData httpRequest, int? id)
        {
            if (!id.HasValue)
            {
                return httpRequest.CreateResponse(HttpStatusCode.BadRequest, $"You must include an id. Example: myapiservice.com/api/{{route}}/{{id}}");
            }
            var createResponse = Delete(id);
            return httpRequest.CreateResponse(createResponse.HttpStatusCode);
        }

        public abstract CRUDResponse<TDataModel> Create(TRequestModel requestModel);
        public abstract CRUDResponse<TDataModel> Read(int? id);
        public abstract CRUDResponse<TDataModel> Update(int? id, TRequestModel? requestModel);
        public abstract CRUDResponse Delete(int? id);
    }
}