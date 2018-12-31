using System;
using RestSharp.Deserializers;
namespace Safaricom.Mpesa.Responses
{
    /// <summary>
    /// B2C Response.
    /// </summary>
    public class B2CResponse : GenericResponse
    {
        /// <summary>
        /// Gets or sets the response code.
        /// </summary>
        /// <value>The response code.</value>
        [DeserializeAs(Name = "ResponseCode")]
        public int ResponseCode { get; set; }
    }
}
