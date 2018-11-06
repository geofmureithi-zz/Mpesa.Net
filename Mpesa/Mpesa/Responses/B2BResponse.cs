using System;
using RestSharp.Deserializers;
namespace Safaricom.Mpesa.Responses
{
    public class B2CResponse : GenericResponse
    {

        [DeserializeAs(Name = "ResponseCode")]
        public int ResponseCode { get; set; }
    }
}
