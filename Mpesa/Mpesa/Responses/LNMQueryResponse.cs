using System;
using RestSharp.Deserializers;

namespace Safaricom.Mpesa.Responses
{
    /// <summary>
    /// LNMQuery response.
    /// </summary>
    public class LNMQueryResponse
    {
        [DeserializeAs(Name = "ResponseCode")]
        public string ResponseCode { get; set; }
        [DeserializeAs(Name = "ResponseDescription")]
        public string ResponseDescription { get; set; }
        [DeserializeAs(Name = "MerchantRequestID")]
        public string MerchantRequestID { get; set; }
        [DeserializeAs(Name = "CheckoutRequestID")]
        public string CheckoutRequestID { get; set; }
        [DeserializeAs(Name = "ResultCode")]
        public string ResultCode { get; set; }
        [DeserializeAs(Name = "ResultDesc")]
        public string ResultDesc { get; set; }
    }
}
