using System;
using RestSharp.Deserializers;
namespace Safaricom.Mpesa.Responses
{
    public class AccountBalanceResponse : GenericResponse
    {
        [DeserializeAs(Name = "ResponseCode")]
        public int ResponseCode { get; set; }

    }
}
