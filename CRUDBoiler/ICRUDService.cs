using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;

namespace CRUDBoiler
{
    public interface ICRUDService<TDataModel, TRequestModel>
    {
        CRUDResponse<TDataModel> Create(TRequestModel requestModel);
        CRUDResponse<TDataModel> Read(int? id);
        CRUDResponse<TDataModel> Update(int? id, TRequestModel? requestModel);
        CRUDResponse Delete(int? id);
    }
}