using System;
using System.Collections.Generic;
using RestSharp.Deserializers;

namespace Safaricom.Mpesa.Responses
{
    /// <summary>
    /// Lipa na mpesa online response.
    /// </summary>
    public class LNMPaymentResponse : GenericResponse
    {
        /// <summary>
        /// Gets or sets the response code.
        /// </summary>
        /// <value>The response code.</value>
        [DeserializeAs(Name = "ResponseCode")]
        public int ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets the checkout request identifier.
        /// </summary>
        /// <value>The checkout request identifier.</value>
        public string CheckoutRequestID { get; set; }

        /// <summary>
        /// Gets or sets the customer message.
        /// </summary>
        /// <value>The customer message.</value>
        public string CustomerMessage { get; set; }

        /// <summary>
        /// Gets or sets the merchant request identifier.
        /// </summary>
        /// <value>The merchant request identifier.</value>
        public string MerchantRequestID { get; set; }
    }
}