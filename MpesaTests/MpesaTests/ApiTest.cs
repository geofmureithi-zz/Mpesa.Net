using Xunit;
using Safaricom.Mpesa;
using static Safaricom.Mpesa.Api;
using Safaricom.Mpesa.Helpers;

namespace MpesaTests
{
    public class ApiTest
    {
        private static readonly string FakeURL = "https://example.co.ke/url";
        private static readonly long msisdn = 254712345678;
        private static readonly ExtraConfig configs = new ExtraConfig
        {
            ShortCode = 600111,
            Initiator = "testapi111",
            LNMShortCode = 174379,
            LNMPassWord = "bfb279f9aa9bdbcf158e97dd71a467cd2e0c893059b10f78e6b72ada1ed2c919",
            SecurityCredential = "Safaricom111!",
            CertPath = "~/c-sharp-mpesa-lib/Mpesa/cert.cer"
        };
        Api mpesa = new Api(Env.Sandbox, "OT13kmfq1I8GcD2D4JIcyrHO7C3IAM81", "nxKU4f4Zq6h1urLD", configs);

        [Fact]
        public void AuthTest()
        {
            Assert.NotEmpty(mpesa.Auth().Data.AccessToken);
        }

        [Fact]
        public async void AsyncAuthTest()
        {
            var res = await mpesa.AuthAsync();
            Assert.NotEmpty(res.Data.AccessToken);
        }

        [Fact]
        public async void AccountBalance()
        {
            var party = new IdentityParty(configs.ShortCode, IdentityParty.IdentifierType.SHORTCODE);
            var res = await mpesa.AccountBalance(party, FakeURL, FakeURL);
            Assert.Equal(0, res.Data.ResponseCode);
        }
        [Fact]
        public async void C2BRegister()
        {
            var res = await mpesa.C2BRegister(FakeURL, FakeURL);
            Assert.NotNull(res.Data.ConversationId);
        }
        [Fact]
        public async void B2B()
        {
            var receiverParty = new IdentityParty(600111, IdentityParty.IdentifierType.SHORTCODE);
            var res = await mpesa.B2B(receiverParty, 100, FakeURL, FakeURL);
            Assert.NotNull(res.Data.ConversationId);
        }

        [Fact]
        public async void C2BSimulate()
        {
            var res = await mpesa.C2BSimulate(msisdn, 100, "Sample Ref");
            Assert.NotNull(res.Data.ConversationId);
        }

        [Fact]
        public async void B2C()
        {
            var res = await mpesa.B2C(msisdn, 100, FakeURL, FakeURL);
            Assert.NotNull(res.Data.ConversationId);
        }

        [Fact]
        public async void LipaNaMpesa()
        {
            var res = await mpesa.LipaNaMpesaOnline(msisdn, 100, FakeURL, "Some Ref");
            Assert.NotNull(res.Data.CheckoutRequestID);
            var requestDetails = await mpesa.LipaNaMpesaQuery(res.Data.CheckoutRequestID);
            Assert.NotNull(requestDetails.Data.ResponseCode);
        }

        [Fact]
        public async void ReversalRequest()
        {
            var res = await mpesa.ReversalRequest("LKXXXX1234", 100, FakeURL, FakeURL);
            Assert.Equal(0, res.Data.ResponseCode);

        }

        [Fact]
        public async void TransactionStatus()
        {
            var res = await mpesa.TransactionStatus("LKXXXX1234", FakeURL, FakeURL);
            Assert.Equal(0, res.Data.ResponseCode);
        }
    }
}
