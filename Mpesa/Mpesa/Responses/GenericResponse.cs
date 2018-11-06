using System;
using RestSharp.Deserializers;
namespace Safaricom.Mpesa.Responses
{
    public class GenericResponse
    {
        [DeserializeAs(Name = "ConversationID")]
        public string ConversationId { get; set; }

        [DeserializeAs(Name = "OriginatorCoversationID")]
        public string OriginatorCoversationId { get; set; }

        [DeserializeAs(Name = "ResponseDescription")]
        public string ResponseDescription { get; set; }
    }
}
