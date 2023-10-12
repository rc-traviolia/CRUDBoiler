using System.Net;

namespace CRUDBoiler
{
    public class CRUDResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? Message { get; set; }
    }
    public class CRUDResponse<TDataModel> : CRUDResponse
    {
        public TDataModel? Value { get; set; }
    }
}