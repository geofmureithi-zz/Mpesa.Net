using System;
using Xunit;
using Nancy;
using Nancy.Testing;
namespace MpesaTests
{
    public delegate void EventHandler();
    public class IntergrationTests
    {
        protected class CallBacksMock : NancyModule
        {
            public CallBacksMock()
            {
                Get["/"] = _ => "Hello From Mpesa C# Tests";
                Get["/mock/callback"] = _ =>
                {
                    IntergrationTests.C2BCallback?.Invoke();
                    return "Success";
                };

            }
        }
        public static event EventHandler C2BCallback = new EventHandler(OnC2B);
        public static event EventHandler B2BCallback;
        CallBacksMock mock;
        public IntergrationTests()
        {
            // Given
            mock = new CallBacksMock();
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);
            // When
            var result = browser.Get("/mock/callback", with =>
            {
                with.HttpRequest();
            });
        }
    }
}