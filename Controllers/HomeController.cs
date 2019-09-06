using Aiursoft.Pylon.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Tracer.Models;

namespace Tracer.Controllers
{
    public class HomeController : Controller
    {
        private static byte[] _data;
        private const int _length = 1024 * 1024 * 1;
        private static byte[] GetData()
        {
            if (_data == null)
            {
                _data = new byte[_length];
                for (int i = 0; i < _length; i++)
                {
                    _data[i] = 1;
                }
            }
            return _data;
        }

        private IPusher<WebSocket> _pusher;
        public HomeController()
        {
            _pusher = new WebSocketPusher();
        }


        public IActionResult Index()
        {
            return View();
        }

        [AiurNoCache]
        [AiurForceWebSocket]
        public async Task<IActionResult> Pushing()
        {
            await _pusher.Accept(HttpContext);
            for (int i = 0; i < 36000 && _pusher.Connected; i++)
            {
                try
                {
                    _pusher.SendMessage(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffffff")).GetAwaiter();
                    await Task.Delay(100);
                }
                catch
                {
                    break;
                }
            }
            return null;
        }

        [AiurNoCache]
        public IActionResult Download()
        {
            HttpContext.Response.Headers.Add("Content-Length", _length.ToString());
            return new FileContentResult(GetData(), "application/octet-stream");
        }
    }
}
