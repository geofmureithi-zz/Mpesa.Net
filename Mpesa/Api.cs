using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.X509;
using RestSharp;
using Safaricom.Mpesa.Responses;

namespace Safaricom.Mpesa
{
    /*
     * The Main Mpesa class
     */
    /// <summary>
    /// Contains the main api calls and extra utils
    /// </summary>
    public class Api
    {
        private Env Environment;
        private string ConsumerKey;
        private string ConsumerSecret;
        private RestClient client;
        private ExtraConfig Config;
        /// <summary>
        /// Used to set extra config needed for advanced calls such as LNM
        /// </summary>
        public class ExtraConfig
        {
            public int ShortCode { get; set; }
            public string Initiator { get; set; }
            public int LNMShortCode { get; set; }
            public string LNMPassWord { get; set; }
            public string SecurityCredential { get; set; }
        }

        /// <summary>
        /// Enum type class for available environments
        /// </summary>
        public class Env
        {
            public string Endpoint { get; set; }
            private Env(string env) { Endpoint = env; }
            public static Env Sandbox { get { return new Env("https://sandbox.safaricom.co.ke"); } }
            public static Env Production { get { return new Env("https://api.safaricom.co.ke"); } }
        }

        /// <summary>
        /// Identifier types as provided by Daraja
        /// </summary>
        /// <see cref="https://developer.safaricom.co.ke/docs#identifier-types"/>
        public enum IdentifierType{
            MSISDN = 1,
            TILL = 2,
            SHORTCODE = 4
        }

        public Api(Env env, string consumerKey, string consumerSecret, ExtraConfig config = null)
        {
            Environment = env;
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
            client = new RestClient(env.Endpoint)
            {
                Proxy = new WebProxy("127.0.0.1", 8008)
            };
            if (config != null)
                Config = config;

        }

        protected String getSecurityCredential()
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(Config.SecurityCredential);

            PemReader pr = new PemReader(
                (StreamReader)File.OpenText("./cert.cer")
            );
            X509Certificate certificate = (X509Certificate)pr.ReadObject();

            //PKCS1 v1.5 paddings
            Pkcs1Encoding eng = new Pkcs1Encoding(new RsaEngine());
            eng.Init(true, certificate.GetPublicKey());

            int length = plainTextBytes.Length;
            int blockSize = eng.GetInputBlockSize();
            List<byte> cipherTextBytes = new List<byte>();
            for (int chunkPosition = 0;
                chunkPosition < length;
                chunkPosition += blockSize)
            {
                int chunkSize = Math.Min(blockSize, length - chunkPosition);
                cipherTextBytes.AddRange(eng.ProcessBlock(
                    plainTextBytes, chunkPosition, chunkSize
                ));
            }
            return Convert.ToBase64String(cipherTextBytes.ToArray());
        }

        public IRestResponse<AuthResponse> Auth()
        {
            var request = new RestRequest("oauth/v1/generate?grant_type=client_credentials", Method.GET);
            byte[] creds = Encoding.UTF8.GetBytes(ConsumerKey + ":" + ConsumerSecret);
            String encoded = System.Convert.ToBase64String(creds);
            request.AddHeader("Authorization", "Basic " + encoded);
            IRestResponse<AuthResponse> response = client.Execute<AuthResponse>(request);
            return response;
        }

        public IRestResponse<AccountBalanceResponse> AccountBalance(int shortCode, int idType, string queueUrl, string resultUrl, string remarks = "Checking account balance", string initiator = null, string commandId = "AccountBalance")
        {
            var request = new RestRequest("mpesa/accountbalance/v1/query", Method.POST);
            AuthResponse authResponse = this.Auth().Data;
            request.AddHeader("Authorization", "Bearer " + authResponse.AccessToken);
            request.AddJsonBody(new
            {
                Config.Initiator,
                SecurityCredential = getSecurityCredential(),
                CommandID = commandId,
                PartyA = shortCode,
                IdentifierType = idType,
                Remarks = remarks,
                QueueTimeOutURL = queueUrl,
                ResultURL = resultUrl
            });
            return client.Execute<AccountBalanceResponse>(request);
        }

        public IRestResponse<C2BRegisterResponse> C2BRegister(string confirmationURL, string validationURL)
        {
            var request = new RestRequest("mpesa/c2b/v1/registerurl", Method.POST);
            AuthResponse authResponse = this.Auth().Data;
            request.AddHeader("Authorization", "Bearer " + authResponse.AccessToken);
            request.AddJsonBody(new
            {
                Config.ShortCode,
                ResponseType = "Completed",
                ConfirmationURL = confirmationURL,
                ValidationURL = validationURL
            });
            return client.Execute<C2BRegisterResponse>(request);
        }

        public IRestResponse<B2BResponse> B2B(string receiverParty, int amount, string queueUrl, string resultUrl, IdentifierType senderType = IdentifierType.SHORTCODE, IdentifierType receiverType = IdentifierType.SHORTCODE, string commandId = "BusinessToBusinessTransfer", string remarks = "B2B Payment", string accountRef = null)
        {
            var request = new RestRequest("mpesa/b2b/v1/paymentrequest", Method.POST);
            AuthResponse authResponse = this.Auth().Data;
            request.AddHeader("Authorization", "Bearer " + authResponse.AccessToken);
            request.AddJsonBody(new
            {
                Config.Initiator,
                SecurityCredential = getSecurityCredential(),
                CommandID = commandId,
                SenderIdentifierType = senderType,
                RecieverIdentifierType = receiverType,
                Amount = amount,
                PartyA = Config.ShortCode,
                PartyB = receiverParty,
                AccountReference = accountRef,
                Remarks = remarks,
                QueueTimeOutURL = queueUrl,
                ResultURL = resultUrl
            });
            return client.Execute<B2BResponse>(request);
        }

        public IRestResponse<C2BSimulateResponse> C2BSimulate(long msisdn, int amount, string billRefNumber, string commandId = "CustomerPayBillOnline")
        {
            var request = new RestRequest("mpesa/c2b/v1/simulate", Method.POST);
            AuthResponse authResponse = this.Auth().Data;
            request.AddHeader("Authorization", "Bearer " + authResponse.AccessToken);
            request.AddJsonBody(new
            {
                Config.ShortCode,
                CommandID = commandId,
                Amount = amount,
                Msisdn = msisdn,
                BillRefNumber = billRefNumber
            });
            return client.Execute<C2BSimulateResponse>(request);
        }

        public IRestResponse<B2CResponse> B2C(long msisdn, int amount, string queueUrl, string resultUrl, string commandId = "BusinessPayment", string remarks = "B2C Payment", string occasion="None")
        {
            var request = new RestRequest("mpesa/b2c/v1/paymentrequest", Method.POST);
            AuthResponse authResponse = this.Auth().Data;
            request.AddHeader("Authorization", "Bearer " + authResponse.AccessToken);
            request.AddJsonBody(new
            {
                InitiatorName =  Config.Initiator,
                SecurityCredential = getSecurityCredential(),
                CommandID = commandId,
                Amount = amount,
                PartyA = Config.ShortCode,
                PartyB = msisdn,
                Remarks = remarks,
                QueueTimeOutURL = queueUrl,
                ResultURL = resultUrl,
                Occasion = occasion
            });
            return client.Execute<B2CResponse>(request);
        }

        public static void Main()
        {
            var configs = new ExtraConfig
            {
                ShortCode = 600111,
                Initiator = "testapi111",
                LNMShortCode = 174379,
                LNMPassWord = "bfb279f9aa9bdbcf158e97dd71a467cd2e0c893059b10f78e6b72ada1ed2c919",
                SecurityCredential = "Safaricom111!"
            };
            var mpesa = new Api(Env.Sandbox, "OT13kmfq1I8GcD2D4JIcyrHO7C3IAM81", "nxKU4f4Zq6h1urLD", configs);
            Console.WriteLine(mpesa.B2C(254708374149, 100, "https://test.co.ke/queue", "https://test.co.ke/result").Data.ConversationId);
        }
    }
}
