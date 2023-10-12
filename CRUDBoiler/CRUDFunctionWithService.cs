using System.Net;
using System.Text.Json;
using Azure.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using CRUDBoiler;

namespace CRUDBoiler
{
    public abstract class CRUDFunctionWithService<TDataModel, TRequestModel>
    {
        private readonly ICRUDService<TDataModel, TRequestModel> _iCRUDService;

        public CRUDFunctionWithService(ICRUDService<TDataModel, TRequestModel> iCRUDService)
        { 
            _iCRUDService = iCRUDService;
        }

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

            var createResponse = _iCRUDService.Create(requestBody);
            return httpRequest.CreateResponse(createResponse.StatusCode);
        }
        public virtual HttpResponseData Read([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData httpRequest, int? id)
        {
            if (!id.HasValue)
            {
                return httpRequest.CreateResponse(HttpStatusCode.BadRequest, $"You must include an id. Example: myapiservice.com/api/{{route}}/{{id}}");
            }

            var createResponse = _iCRUDService.Read(id);
            return httpRequest.CreateResponse(createResponse.StatusCode);
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
            var createResponse = _iCRUDService.Update(id, requestBody.Result);
            return httpRequest.CreateResponse(createResponse.StatusCode);
        }
        public virtual HttpResponseData Delete([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequestData httpRequest, int? id)
        {
            if (!id.HasValue)
            {
                return httpRequest.CreateResponse(HttpStatusCode.BadRequest, $"You must include an id. Example: myapiservice.com/api/{{route}}/{{id}}");
            }
            var createResponse = _iCRUDService.Delete(id);
            return httpRequest.CreateResponse(createResponse.StatusCode);
        }

        
    }
}
