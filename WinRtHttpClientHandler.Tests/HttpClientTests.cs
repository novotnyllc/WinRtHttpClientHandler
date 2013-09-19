using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace WinRtHttpClientHandler.Tests
{
    [TestClass]
    public class HttpClientTests
    {
        [TestMethod]
        public async Task TestClient()
        {
            var filter = new HttpBaseProtocolFilter();

         //   filter.IgnorableServerCertificateErrors.Add()

            using (var client = new HttpClient(new WinRtHttpClientHandler(filter)))
            {

                var html = await client.GetStringAsync("http://www.microsoft.com");

                Assert.IsNotNull(html);
            }
        }
    }
}
