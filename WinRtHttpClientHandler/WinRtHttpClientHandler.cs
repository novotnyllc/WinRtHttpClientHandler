using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;
using rt = Windows.Web.Http;


namespace WinRtHttpClientHandler
{
    /// <summary>
    /// An HttpMessageHandler that lets you use the Windows Runtime IHttpFilter types
    /// </summary>
    public class WinRtHttpClientHandler : HttpMessageHandler
    {
        private readonly rt.HttpClient _client;
        private bool _disposed = false;

        public WinRtHttpClientHandler(IHttpFilter httpFilter)
        {
            _client = new rt.HttpClient(httpFilter);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CheckDisposed();

            var rtMessage = await ConvertHttpRequestMessaageToRt(request, cancellationToken).ConfigureAwait(false);

            var resp = await _client.SendRequestAsync(rtMessage).AsTask(cancellationToken).ConfigureAwait(false);

            var netResp = await ConvertRtResponseMessageToNet(resp, cancellationToken).ConfigureAwait(false);

            return netResp;
        }

        internal static async Task<rt.HttpRequestMessage> ConvertHttpRequestMessaageToRt(HttpRequestMessage message, CancellationToken token)
        {
            var rt = new rt.HttpRequestMessage()
            {
              Method  =  new rt.HttpMethod(message.Method.Method),
              Content =  await GetContentFromNet(message.Content).ConfigureAwait(false),
              RequestUri = message.RequestUri,
              
            };

            CopyHeaders(message.Headers, rt.Headers);

            foreach(var prop in message.Properties)
                rt.Properties.Add(prop);

            return rt;
        }

        internal static async Task<HttpRequestMessage> ConvertRtRequestMessageToNet(rt.HttpRequestMessage message, CancellationToken token)
        {
            var req = new HttpRequestMessage()
            {
                Method = new HttpMethod(message.Method.Method),
                RequestUri = message.RequestUri,
                Content = await GetNetContentFromRt(message.Content, token).ConfigureAwait(false),
            };

            foreach (var header in message.Headers)
                req.Headers.TryAddWithoutValidation(header.Key, header.Value);

            foreach (var prop in message.Properties)
                req.Properties.Add(prop);
            
            return req;
        }

        internal static void CopyHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> source, IDictionary<string, string> destination)
        {
            var headers = from kvp in source
                          from val in kvp.Value
                          select new KeyValuePair<string, string>(kvp.Key, val);
            foreach(var header in headers)
                destination.Add(header);
        }

        internal static async Task<rt.IHttpContent> GetContentFromNet(HttpContent content)
        {
            if (content == null)
                return null;

            var stream = await content.ReadAsStreamAsync().ConfigureAwait(false);
            var c = new rt.HttpStreamContent(stream.AsInputStream());

            CopyHeaders(content.Headers, c.Headers);
            
            return c;
        }

        internal static async Task<HttpContent> GetNetContentFromRt(rt.IHttpContent content, CancellationToken token)
        {
            if(content == null)
                return null;

            var str = await content.ReadAsInputStreamAsync().AsTask(token).ConfigureAwait(false);

            var c = new StreamContent(str.AsStreamForRead());

            foreach (var header in content.Headers)
                c.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return c;
        }


        internal static async Task<HttpResponseMessage> ConvertRtResponseMessageToNet(rt.HttpResponseMessage message, CancellationToken token)
        {
            var resp = new HttpResponseMessage((HttpStatusCode)(int)message.StatusCode)
            {
                ReasonPhrase = message.ReasonPhrase,
                RequestMessage = await ConvertRtRequestMessageToNet(message.RequestMessage, token).ConfigureAwait(false),
                Content = await GetNetContentFromRt(message.Content, token).ConfigureAwait(false),
                
          //      Version = message.Source
            };

            foreach (var header in message.Headers)
                resp.Headers.TryAddWithoutValidation(header.Key, header.Value);
            
            return resp;
        }

        private void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("WinRtHttpClientHandler");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _client.Dispose();

                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
