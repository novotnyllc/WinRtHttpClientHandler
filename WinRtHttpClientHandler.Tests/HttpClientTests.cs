﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace WinRtHttpClientHandler.Tests
{
    [TestClass]
    public class HttpClientTests
    {
        [TestMethod]
        public async Task TestClient()
        {
            var handler = new WinRtHttpClientHandler();
            var client = new HttpClient(handler);

            var html = await client.GetStringAsync("http://www.microsoft.com");

            Assert.IsNotNull(html);
        }
    }
}
