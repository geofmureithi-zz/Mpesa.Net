using System;
using RestSharp.Deserializers;
namespace Safaricom.Mpesa.Responses
{
    public class AuthResponse
    {
        [DeserializeAs(Name = "access_token")]
        public string AccessToken { get; set; }

        [DeserializeAs(Name = "expires_in")]
        public int Expiration { get; set; }
    }
}
