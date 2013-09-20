using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Newtonsoft.Json;

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


        [TestMethod]
        public async Task TestPost()
        {
            var filter = new HttpBaseProtocolFilter();

            //   filter.IgnorableServerCertificateErrors.Add()

            using (var client = new HttpClient(new WinRtHttpClientHandler(filter)))
            {
                var uri = new Uri("http://www.microsoft.com/");

                var resp = await client.PostAsJsonAsync("https://www.googleapis.com/urlshortener/v1/url", new UrlShortener { LongUrl = uri });

                resp.EnsureSuccessStatusCode();

                var shortened = await resp.Content.ReadAsAsync<UrlShortenResponse>();

                Assert.AreEqual(shortened.LongUrl, uri);
            }
        }


        private class UrlShortener
        {
            [JsonProperty("longUrl")]
            public Uri LongUrl { get; set; }
        }

        private class UrlShortenResponse
        {
            public string Kind { get; set; }
            public Uri Id { get; set; }
            public Uri LongUrl { get; set; }
        }
    }
}
