using System;
using RestSharp.Deserializers;
namespace Safaricom.Mpesa.Responses
{
    public class AuthResponse
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>The access token.</value>
        [DeserializeAs(Name = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the expiration time of token.
        /// </summary>
        /// <value>The expiration time of token.</value>
        [DeserializeAs(Name = "expires_in")]
        public int Expiration { get; set; }
    }
}
