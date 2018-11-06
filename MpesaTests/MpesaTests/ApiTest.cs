using System;
using Xunit;
using Safaricom.Mpesa;
using static Safaricom.Mpesa.Api;

namespace MpesaTests
{
    public class ApiTest{
        Api mpesa;
        public ApiTest(){
            var configs = new ExtraConfig
            {
                ShortCode = 600111,
                Initiator = "testapi111",
                LNMShortCode = 174379,
                LNMPassWord = "bfb279f9aa9bdbcf158e97dd71a467cd2e0c893059b10f78e6b72ada1ed2c919",
                SecurityCredential = "Safaricom111!"
            };
            mpesa = new Api(Env.Sandbox, "OT13kmfq1I8GcD2D4JIcyrHO7C3IAM81", "nxKU4f4Zq6h1urLD", configs);
        }

        [Fact]
        public void AuthTest()
        {
            Assert.NotEmpty(mpesa.Auth().Data.AccessToken);
        }
    }
}
