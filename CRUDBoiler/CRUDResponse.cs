using System.Net;

namespace CRUDBoiler
{
    public class CRUDResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }
    }
    public class CRUDResponse<TDataModel> : CRUDResponse
    {

    }
}