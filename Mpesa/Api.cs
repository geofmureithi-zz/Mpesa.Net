using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.X509;
using RestSharp;
using Safaricom.Mpesa.Helpers;
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
        private readonly Env Environment;
        private readonly string ConsumerKey;
        private readonly string ConsumerSecret;
        private RestClient client;
        private ExtraConfig Config;
        /// <summary>
        /// Used to set extra config needed for advanced calls such as LNM
        /// </summary>
        public class ExtraConfig
        {
            /// <summary>
            /// Gets or sets the short code.
            /// </summary>
            /// <value>The short code.</value>
            public int ShortCode { get; set; }
            /// <summary>
            /// Gets or sets the initiator.
            /// </summary>
            /// <value>The initiator.</value>
            public string Initiator { get; set; }
            /// <summary>
            /// Gets or sets the LNM Short code.
            /// </summary>
            /// <value>The LNM Shortcode.</value>
            public int LNMShortCode { get; set; }
            /// <summary>
            /// Gets or sets the LNM Password.
            /// </summary>
            /// <value>The LNMPassword.</value>
            public string LNMPassWord { get; set; }
            /// <summary>
            /// Gets or sets the security credential.
            /// </summary>
            /// <value>The security credential.</value>
            public string SecurityCredential { get; set; }
            /// <summary>
            /// Gets or sets the cert path.
            /// </summary>
            /// <value>Path to Mpesa Public Key</value>
            public string CertPath { get; set; }
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
        /// Initializes a new instance of the <see cref="T:Safaricom.Mpesa.Api"/> class.
        /// </summary>
        /// <param name="env">Env.</param>
        /// <param name="consumerKey">Consumer key.</param>
        /// <param name="consumerSecret">Consumer secret.</param>
        /// <param name="config">Configuration for LNM, Security credentials etc.</param>
        /// <example>
        /// <code>
        /// Api mpesa = new Api(Env.Sandbox, "Key", "Secret", null);
        /// </code>
        /// </example>
        public Api(Env env, string consumerKey, string consumerSecret, ExtraConfig config = null)
        {
            Environment = env;
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
            client = new RestClient(env.Endpoint);
            if (config != null)
                Config = config;

        }

        /// <summary>
        /// Gets the security credential.
        /// </summary>
        /// <returns>The security credential.</returns>
        protected String GetSecurityCredential()
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(Config.SecurityCredential);

            PemReader pr = new PemReader(
                (StreamReader)File.OpenText(Config.CertPath)
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

        /// <summary>
        /// Generates the auth request.
        /// </summary>
        /// <returns>The auth request.</returns>
        private RestRequest GenerateAuthRequest()
        {
            var request = new RestRequest("oauth/v1/generate?grant_type=client_credentials", Method.GET);
            byte[] creds = Encoding.UTF8.GetBytes(ConsumerKey + ":" + ConsumerSecret);
            String encoded = System.Convert.ToBase64String(creds);
            request.AddHeader("Authorization", "Basic " + encoded);
            return request;
        }

        /// <summary>
        /// OAuth.
        /// </summary>
        /// <returns>The Creds.</returns>
        /// <example>
        /// <code>
        /// Api mpesa = new Api(Env.Sandbox, "consumerKey", "consumerSecret", configs);
        /// var token = mpesa.Auth().Data.AccessToken
        /// </code>
        /// </example>
        public IRestResponse<AuthResponse> Auth()
        {
            IRestResponse<AuthResponse> response = client.Execute<AuthResponse>(GenerateAuthRequest());
            return response;
        }

        /// <summary>
        /// Async OAuth.
        /// </summary>
        /// <returns>The credentials awaitable.</returns>
        /// <example>
        /// <code>
        /// Api mpesa = new Api(Env.Sandbox, "consumerKey", "consumerSecret", configs);
        /// var res = await mpesa.AuthAsync();
        /// var token = res.Data.AccessToken
        /// </code>
        /// </example>
        public Task<IRestResponse<AuthResponse>> AuthAsync()
        {
            return client.ExecuteTaskAsync<AuthResponse>(GenerateAuthRequest());
        }

        /// <summary>
        /// Accounts the balance.
        /// </summary>
        /// <returns>The balance awaitable.</returns>
        /// <param name="partyA">The account you are checking balance for</param>
        /// <param name="queueUrl">Queue URL.</param>
        /// <param name="resultUrl">Result URL.</param>
        /// <param name="remarks">Remarks.</param>
        /// <example>
        /// <code>
        /// Api mpesa = new Api(Env.Sandbox, "consumerKey", "consumerSecret", configs);
        /// var accountyParty = new IdentityParty(configs.ShortCode, IdentityParty.IdentifierType.SHORTCODE);
        /// var res = await mpesa.AccountBalance(<paramref name="partyA"/>, <paramref name="queueUrl"/>, <paramref name="resultUrl"/>);
        /// var transactionId = res.Data.ConversationID;
        /// </code>
        /// </example>
        public Task<IRestResponse<AccountBalanceResponse>> AccountBalance(IdentityParty partyA, string queueUrl, string resultUrl, string remarks = "Checking account balance")
        {
            if (!partyA.Type.Equals(IdentityParty.IdentifierType.SHORTCODE))
                throw new Exception("Account Balance can only be done for Shortcodes");
            var request = new RestRequest("mpesa/accountbalance/v1/query", Method.POST);
            AuthResponse authResponse = this.Auth().Data;
            request.AddHeader("Authorization", "Bearer " + authResponse.AccessToken);
            request.AddJsonBody(new
            {
                Config.Initiator,
                SecurityCredential = GetSecurityCredential(),
                CommandID = CommandID.AccountBalance.ToString(),
                PartyA = partyA.Party,
                IdentifierType = partyA.Type,
                Remarks = remarks,
                QueueTimeOutURL = queueUrl,
                ResultURL = resultUrl
            });
            return client.ExecuteTaskAsync<AccountBalanceResponse>(request);
        }

        /// <summary>
        /// C2B Register URL.
        /// </summary>
        /// <returns>The acknowledgement.</returns>
        /// <param name="confirmationURL">The Confirmation URL.</param>
        /// <param name="validationURL">The Validation URL</param>
        /// <example>
        /// <code>
        /// Api mpesa = new Api(Env.Sandbox, "consumerKey", "consumerSecret", configs);
        /// var res = await mpesa.C2BRegister(<paramref name="confirmationURL"/>, <paramref name="validationURL"/>)
        /// var transactionId = res.Data.ConversationID;
        /// </code>
        /// </example>
        public Task<IRestResponse<C2BRegisterResponse>> C2BRegister(string confirmationURL, string validationURL)
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
            return client.ExecuteTaskAsync<C2BRegisterResponse>(request);
        }
        /// <summary>
        /// Business to Business Mpesa transaction
        /// </summary>
        /// <returns>The Status</returns>
        /// <param name="receiverParty">Receiver party.</param>
        /// <param name="amount">Amount.</param>
        /// <param name="queueUrl">Queue URL.</param>
        /// <param name="resultUrl">Result URL.</param>
        /// <param name="commandId">Command identifier.</param>
        /// <param name="remarks">Remarks.</param>
        /// <param name="accountRef">Account reference.</param>
        /// <example>
        /// <code>
        /// Api mpesa = new Api(Env.Sandbox, "consumerKey", "consumerSecret", configs);
        /// var receiverParty = new IdentityParty(600111, IdentityParty.IdentifierType.SHORTCODE);
        /// var res = await mpesa.B2B(<paramref name="receiverParty"/>, <paramref name="amount"/>, <paramref name="queueUrl"/>, <paramref name="resultUrl"/>);
        /// var transactionId = res.Data.ConversationID;
        /// </code>
        /// </example>
        public Task<IRestResponse<B2BResponse>> B2B(IdentityParty receiverParty, int amount, string queueUrl, string resultUrl,  CommandID commandId = null, string remarks = "B2B Payment", string accountRef = null)
        {
            if (receiverParty.Type.Equals(IdentityParty.IdentifierType.MSISDN))
                throw new Exception("MSISDNs cannot be used in B2B requests as they are not Businesses");
            var request = new RestRequest("mpesa/b2b/v1/paymentrequest", Method.POST);
            AuthResponse authResponse = this.Auth().Data;
            request.AddHeader("Authorization", "Bearer " + authResponse.AccessToken);
            request.AddJsonBody(new
            {
                Config.Initiator,
                SecurityCredential = GetSecurityCredential(),
                CommandID = (commandId ?? CommandID.BusinessToBusinessTransfer).ToString(),
                SenderIdentifierType = IdentityParty.IdentifierType.SHORTCODE,
                RecieverIdentifierType = receiverParty.Type,
                Amount = amount,
                PartyA = Config.ShortCode,
                PartyB = receiverParty.Party,
                AccountReference = accountRef,
                Remarks = remarks,
                QueueTimeOutURL = queueUrl,
                ResultURL = resultUrl
            });
            return client.ExecuteTaskAsync<B2BResponse>(request);
        }
        /// <summary>
        /// Customer to Business Transaction Simulation
        /// </summary>
        /// <remarks>Since Mpesa transactions may be triggered from the users STK menu, this is used in Sandbox to simulate that transaction</remarks>
        /// <returns>The status.</returns>
        /// <param name="msisdn">Msisdn.</param>
        /// <param name="amount">Amount.</param>
        /// <param name="billRefNumber">Bill reference number.</param>
        /// <param name="commandId">Command identifier.</param>
        /// <example>
        /// <code>
        /// Api mpesa = new Api(Env.Sandbox, "consumerKey", "consumerSecret", configs);
        /// var res = await mpesa.C2BSimulate(msisdn, 100, "Sample Ref");
        /// var transactionId = res.Data.ConversationID;
        /// </code>
        /// </example>
        public Task<IRestResponse<C2BSimulateResponse>> C2BSimulate(long msisdn, int amount, string billRefNumber, CommandID commandId = null)
        {
            if (this.Environment.Equals(Env.Production))
                throw new Exception("C2BSimulateResponse should not be called in Production");
            var request = new RestRequest("mpesa/c2b/v1/simulate", Method.POST);
            AuthResponse authResponse = this.Auth().Data;
            request.AddHeader("Authorization", "Bearer " + authResponse.AccessToken);
            request.AddJsonBody(new
            {
                Config.ShortCode,
                CommandID = (commandId ?? CommandID.CustomerPayBillOnline).ToString(),
                Amount = amount,
                Msisdn = msisdn,
                BillRefNumber = billRefNumber
            });
            return client.ExecuteTaskAsync<C2BSimulateResponse>(request);
        }
        /// <summary>
        /// Business to Customer Mpesa Transaction
        /// </summary>
        /// <returns>The transaction status</returns>
        /// <param name="msisdn">Msisdn.</param>
        /// <param name="amount">Amount.</param>
        /// <param name="queueUrl">Queue URL.</param>
        /// <param name="resultUrl">Result URL.</param>
        /// <param name="commandId">Command identifier.</param>
        /// <param name="remarks">Remarks.</param>
        /// <param name="occasion">Occasion.</param>
        /// <example>
        /// <code>
        /// Api mpesa = new Api(Env.Sandbox, "consumerKey", "consumerSecret", configs);
        /// var res = await mpesa.B2C(msisdn, 100, queueURL, resultURL);
        /// var transactionId = res.Data.ConversationID;
        /// </code>
        /// </example>
        public Task<IRestResponse<B2CResponse>> B2C(long msisdn, int amount, string queueUrl, string resultUrl, CommandID commandId = null, string remarks = "B2C Payment", string occasion = "None")
        {
            var request = new RestRequest("mpesa/b2c/v1/paymentrequest", Method.POST);
            AuthResponse authResponse = this.Auth().Data;
            request.AddHeader("Authorization", "Bearer " + authResponse.AccessToken);
            request.AddJsonBody(new
            {
                InitiatorName = Config.Initiator,
                SecurityCredential = GetSecurityCredential(),
                CommandID = (commandId ?? CommandID.BusinessPayment).ToString(),
                Amount = amount,
                PartyA = Config.ShortCode,
                PartyB = msisdn,
                Remarks = remarks,
                QueueTimeOutURL = queueUrl,
                ResultURL = resultUrl,
                Occasion = occasion
            });
            return client.ExecuteTaskAsync<B2CResponse>(request);
        }
        /// <summary>
        /// Lipa na mpesa online.
        /// </summary>
        /// <returns>The Transaction Status.</returns>
        /// <param name="senderMsisdn">Sender msisdn.</param>
        /// <param name="amount">Amount.</param>
        /// <param name="callbackUrl">Callback URL.</param>
        /// <param name="accountRef">Account reference.</param>
        /// <param name="transactionDesc">Transaction desc.</param>
        /// <example>
        /// <code>
        /// Api mpesa = new Api(Env.Sandbox, "consumerKey", "consumerSecret", configs);
        /// var res = await mpesa.LipaNaMpesaOnline(msisdn, 100, callbackURL, "Some Ref");
        /// var transactionId = res.Data.CheckoutRequestID;
        /// </code>
        /// </example>
        public Task<IRestResponse<LNMPaymentResponse>> LipaNaMpesaOnline(long senderMsisdn, int amount, string callbackUrl, string accountRef, string transactionDesc = "Lipa na mpesa online")
        {
            var request = new RestRequest("/mpesa/stkpush/v1/processrequest", Method.POST);
            AuthResponse authResponse = this.Auth().Data;
            request.AddHeader("Authorization", "Bearer " + authResponse.AccessToken);
            var timeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");
            var password = String.Concat(Config.LNMShortCode, Config.LNMPassWord, timeStamp);
            request.AddJsonBody(new
            {
                BusinessShortCode = Config.LNMShortCode,
                Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(password)),
                Timestamp = timeStamp,
                TransactionType = CommandID.CustomerPayBillOnline.ToString(), // see https://developer.safaricom.co.ke/docs#lipa-na-m-pesa-online-payment-request-parameters
                Amount = amount,
                PartyA = senderMsisdn,
                PartyB = Config.LNMShortCode,
                PhoneNumber=  senderMsisdn,
                CallBackURL = callbackUrl,
                AccountReference = accountRef,
                TransactionDesc = transactionDesc
            });
            return client.ExecuteTaskAsync<LNMPaymentResponse>(request);
        }
        /// <summary>
        /// Lipa na mpesa query.
        /// </summary>
        /// <returns>The transaction rtesult</returns>
        /// <param name="checkoutRequestId">Checkout request identifier.</param>
        /// <example>
        /// <code>
        /// Api mpesa = new Api(Env.Sandbox, "consumerKey", "consumerSecret", configs);
        /// var res = await mpesa.LipaNaMpesaQuery(checkoutId);
        /// var transactionId = res.Data.ConversationID;
        /// </code>
        /// </example>
        public Task<IRestResponse<LNMQueryResponse>> LipaNaMpesaQuery(string checkoutRequestId)
        {
            var request = new RestRequest("/mpesa/stkpushquery/v1/query", Method.POST);
            AuthResponse authResponse = this.Auth().Data;
            request.AddHeader("Authorization", "Bearer " + authResponse.AccessToken);
            var timeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");
            var password = String.Concat(Config.LNMShortCode, Config.LNMPassWord, timeStamp);
            request.AddJsonBody(new
            {
                BusinessShortCode = Config.LNMShortCode,
                Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(password)),
                Timestamp = timeStamp,
                CheckoutRequestID = checkoutRequestId
            });
            return client.ExecuteTaskAsync<LNMQueryResponse>(request);
        }
        /// <summary>
        /// Reverses a request.
        /// </summary>
        /// <returns>The request status.</returns>
        /// <param name="transactionId">Transaction identifier.</param>
        /// <param name="amount">Amount.</param>
        /// <param name="queueUrl">Queue URL.</param>
        /// <param name="resultUrl">Result URL.</param>
        /// <param name="remarks">Remarks.</param>
        /// <param name="occasion">Occasion.</param>
        /// <example>
        /// <code>
        /// Api mpesa = new Api(Env.Sandbox, "consumerKey", "consumerSecret", configs);
        /// var res = await mpesa.ReversalRequest("LKXXXX1234", 100, queueUrl, resultUrl );
        /// var transactionId = res.Data.ConversationID;
        /// </code>
        /// </example>
        public Task<IRestResponse<ReversalResponse>> ReversalRequest(string transactionId, int amount, string queueUrl, string resultUrl, string remarks = "Reversal", string occasion = "Reversal")
        {
            var request = new RestRequest("/mpesa/reversal/v1/request", Method.POST);
            AuthResponse authResponse = this.Auth().Data;
            request.AddHeader("Authorization", "Bearer " + authResponse.AccessToken);
            request.AddJsonBody(new
            {
                Config.Initiator,
                SecurityCredential = GetSecurityCredential(),
                CommandID = CommandID.TransactionReversal.ToString(),
                TransactionID = transactionId,
                Amount = amount,
                ReceiverParty = Config.ShortCode,
                RecieverIdentifierType = 11, //Please dont ask why ||  See https://developer.safaricom.co.ke/reversal/apis/post/request
                ResultURL = resultUrl,
                QueueTimeOutURL = queueUrl,
                Remarks = remarks,
                Occasion = occasion
            });
            return client.ExecuteTaskAsync<ReversalResponse>(request);
        }
        /// <summary>
        /// Gets a transaction status.
        /// </summary>
        /// <returns>The status</returns>
        /// <param name="transactionId">Transaction identifier.</param>
        /// <param name="queueUrl">Queue URL.</param>
        /// <param name="resultUrl">Result URL.</param>
        /// <param name="remarks">Remarks.</param>
        /// <param name="occasion">Occasion.</param>
        /// <example>
        /// <code>
        /// Api mpesa = new Api(Env.Sandbox, "consumerKey", "consumerSecret", configs);
        /// var res = await mpesa.TransactionStatus("LKXXXX1234", queueUrl, resultUrl );
        /// var transactionId = res.Data.ConversationID;
        /// </code>
        /// </example>
        public Task<IRestResponse<TransactionStatusResponse>> TransactionStatus(string transactionId, string queueUrl, string resultUrl, string remarks = "TransactionReversal", string occasion = "TransactionReversal")
        {
            IdentityParty receiverParty = new IdentityParty(Config.ShortCode, IdentityParty.IdentifierType.SHORTCODE);
            var request = new RestRequest("/mpesa/transactionstatus/v1/query", Method.POST);
            AuthResponse authResponse = this.Auth().Data;
            request.AddHeader("Authorization", "Bearer " + authResponse.AccessToken);
            request.AddJsonBody(new
            {
                Config.Initiator,
                SecurityCredential = GetSecurityCredential(),
                CommandID = CommandID.TransactionStatusQuery.ToString(),
                TransactionID = transactionId,
                PartyA = receiverParty.Party,
                IdentifierType = receiverParty.Type,
                ResultURL = resultUrl,
                QueueTimeOutURL = queueUrl,
                Remarks = remarks,
                Occasion = occasion
            });
            return client.ExecuteTaskAsync<TransactionStatusResponse>(request);
        }
    }
}
