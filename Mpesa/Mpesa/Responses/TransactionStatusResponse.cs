using RestSharp.Deserializers;

namespace Safaricom.Mpesa.Responses
{
    /// <summary>
    /// Transaction status response.
    /// </summary>
    public class TransactionStatusResponse
    {
        /// <summary>
        /// Gets or sets the response code.
        /// </summary>
        /// <value>The response code.</value>
        [DeserializeAs(Name = "ResponseCode")]
        public int ResponseCode { get; set; }
    }
}
