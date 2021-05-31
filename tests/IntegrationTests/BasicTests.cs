﻿using Aiursoft.XelNaga.Tools;
using AngleSharp.Html.Dom;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tracer.Tests.Tools;
using static Aiursoft.WebTools.Extends;

namespace Tracer.Tests.IntegrationTests
{
    [TestClass]
    public class BasicTests
    {
        private readonly string _endpointUrl;
        private readonly int _port;
        private IHost _server;
        private HttpClient _http;

        public BasicTests()
        {
            _port = Network.GetAvailablePort();
            _endpointUrl = $"http://localhost:{_port}";
        }

        [TestInitialize]
        public async Task CreateServer()
        {
            _server = App<TestStartup>(port: _port);
            _http = new HttpClient();
            await _server.StartAsync();
        }

        [TestCleanup]
        public async Task CleanServer()
        {
            await _server.StopAsync();
            _server.Dispose();
        }

        [TestMethod]
        [DataRow("/")]
        [DataRow("/hOmE?aaaaaa=bbbbbb")]
        [DataRow("/hOmE/InDeX")]
        public async Task GetHome(string url)
        {
            var response = await _http.GetAsync(_endpointUrl + url);
            var doc = await HtmlHelpers.GetDocumentAsync(response);

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.AreEqual("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            var p = (IHtmlElement)doc.QuerySelector("p.lead");
            Assert.AreEqual("Aiursoft Tracer is a simple network quality testing app. Helps testing the connection speed between you and Aiursoft services.", p.InnerHtml);
        }

        [TestMethod]
        public async Task GetDownload()
        {
            var url = _endpointUrl + "/Home/Download";
            var response = await _http.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.AreEqual("application/octet-stream", response.Content.Headers.ContentType?.ToString());
            Assert.AreEqual(1024 * 1024, content.Length);
        }

        [TestMethod]
        public async Task Ping()
        {
            var url = _endpointUrl + "/pINg";
            var response = await _http.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            Assert.AreEqual("[]", content);
        }

        private static int _messageCount;
        private static async Task Monitor(ClientWebSocket socket)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            while (true)
            {
                var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    _messageCount++;
                    string message = Encoding.UTF8.GetString(
                        buffer.Skip(buffer.Offset).Take(buffer.Count).ToArray())
                        .Trim('\0')
                        .Trim();
                    Console.WriteLine(message);
                }
                else
                {
                    Console.WriteLine($"[WebSocket Event] Remote wrong message. [{result.MessageType}].");
                    break;
                }
            }
        }

        [TestMethod]
        public async Task TestConnect()
        {
            using (var socket = new ClientWebSocket())
            {
                await socket.ConnectAsync(new Uri(_endpointUrl.Replace("http", "ws") + "/Home/Pushing"), CancellationToken.None);
                Console.WriteLine("Websocket connected!");
                await Task.Factory.StartNew(async () => await Monitor(socket));
                await Task.Delay(500);
                await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }

            await Task.Delay(10);
            Assert.IsTrue(_messageCount > 3);
            Assert.IsTrue(_messageCount < 7);
            Console.WriteLine($"Total messages: {_messageCount}");
        }
    }
}
