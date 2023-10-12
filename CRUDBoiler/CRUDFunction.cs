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

        public CRUDFunction()
        {
        }

        public abstract HttpResponseData IngestionPoint([HttpTrigger(AuthorizationLevel.Function, "put", "get", "patch", "delete", Route = "ENTITY/{id:int?}")] HttpRequestData httpRequest, int? id);
        public virtual HttpResponseData RouteRequest([HttpTrigger(AuthorizationLevel.Function, "put", "get", "patch", "delete")] HttpRequestData httpRequest, int? id)
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
            var requestModel = httpRequest.ReadFromJsonAsync<TRequestModel>();
            if (!requestModel.IsCompletedSuccessfully)
            {
                /*TODO: This does not fail when submitting an incorrect object. It will only fail, if I had to guess, when something other than JSON is sent*/
                return httpRequest.CreateResponse(HttpStatusCode.BadRequest, $"Failed to deserialize request body. {requestModel.Result}");
            }
            var createResponse = Create(requestModel.Result);

            return httpRequest.CreateResponse(createResponse.StatusCode);
        }
        public virtual HttpResponseData Read([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData httpRequest, int? id)
        {
            if (!id.HasValue)
            {
                return httpRequest.CreateResponse(HttpStatusCode.BadRequest, $"You must include an id. Example: myapiservice.com/api/{{route}}/{{id}}");
            }

            var createResponse = Read(id);
            return httpRequest.CreateResponse(createResponse.StatusCode);
        }
        public virtual HttpResponseData Update([HttpTrigger(AuthorizationLevel.Function, "patch")] HttpRequestData httpRequest, int? id)
        {
            if (!id.HasValue)
            {
                return httpRequest.CreateResponse(HttpStatusCode.BadRequest, $"You must include an id. Example: myapiservice.com/api/{{route}}/{{id}}");
            }
            var requestModel = httpRequest.ReadFromJsonAsync<TRequestModel>();

            if (!requestModel.IsCompletedSuccessfully)
            {
                /*TODO: This does not fail when submitting an incorrect object. It will only fail, if I had to guess, when something other than JSON is sent*/
                return httpRequest.CreateResponse(HttpStatusCode.BadRequest, $"Failed to deserialize request body. {requestModel.Result}");
            }
            var createResponse = Update(id, requestModel.Result);
            return httpRequest.CreateResponse(createResponse.StatusCode);
        }
        public virtual HttpResponseData Delete([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequestData httpRequest, int? id)
        {
            if (!id.HasValue)
            {
                return httpRequest.CreateResponse(HttpStatusCode.BadRequest, $"You must include an id. Example: myapiservice.com/api/{{route}}/{{id}}");
            }
            var createResponse = Delete(id);
            return httpRequest.CreateResponse(createResponse.StatusCode);
        }

        public abstract CRUDResponse<TDataModel> Create(TRequestModel? requestModel);
        public abstract CRUDResponse<TDataModel> Read(int? id);
        public abstract CRUDResponse<TDataModel> Update(int? id, TRequestModel? requestModel);
        public abstract CRUDResponse Delete(int? id);
    }
}
