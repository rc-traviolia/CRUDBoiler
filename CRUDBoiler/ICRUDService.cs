using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;

namespace CRUDBoiler
{
    /// <summary>
    /// Interface for a class that needs to implement logic for performing CRUD operations on a <see cref="TDataModel"/>. 
    /// This includes accepting <see cref="TRequestModel"/> for PUT/PATCH operations,
    /// as well as an <seealso href="int?"/> for GET, PATCH, and DELETE operations
    /// </summary>
    /// <param name="request">The <see cref="HttpRequestData"/> for this response.</param>
    /// <param name="statusCode">The response status code.</param>
    /// <returns>The response data.</returns>
    public interface ICRUDService<TDataModel, TRequestModel>
    {
        CRUDResponse<TDataModel> Create(TRequestModel? requestModel);
        CRUDResponse<TDataModel> Read(int? id);
        CRUDResponse<TDataModel> Update(int? id, TRequestModel? requestModel);
        CRUDResponse Delete(int? id);
    }
}