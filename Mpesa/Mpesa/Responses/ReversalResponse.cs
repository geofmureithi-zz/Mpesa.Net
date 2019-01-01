using System;
using RestSharp.Deserializers;

namespace Safaricom.Mpesa.Responses
{
    /// <summary>
    /// Reversal response.
    /// </summary>
    public class ReversalResponse : GenericResponse
    {
        /// <summary>
        /// Gets or sets the response code.
        /// </summary>
        /// <value>The response code.</value>
        [DeserializeAs(Name = "ResponseCode")]
        public int ResponseCode { get; set; }
    }
}
