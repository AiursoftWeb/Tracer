using Aiursoft.SDK.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Tracer.Models;

namespace Tracer.Controllers
{
    public class HomeController : Controller
    {
        private static byte[]? _data;
        private const int Length = 1024 * 1024 * 1;
        private static byte[] GetData()
        {
            if (_data == null)
            {
                _data = new byte[Length];
                for (int i = 0; i < Length; i++)
                {
                    _data[i] = 1;
                }
            }
            return _data;
        }

        private readonly IPusher _pusher;
        public HomeController()
        {
            _pusher = new WebSocketPusher();
        }

        public IActionResult Index()
        {
            return View();
        }

        [AiurNoCache]
        public async Task<IActionResult> Pushing()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest) 
            {
                return Json(new { });
            }
            await _pusher.Accept(HttpContext);
            for (int i = 0; i < 36000 && _pusher.Connected; i++)
            {
                try
                {
                    _pusher.SendMessage(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffffff") + $"|{i + 1}").GetAwaiter();
                    await Task.Delay(100);
                }
                catch
                {
                    break;
                }
            }
            return Json(new { });
        }

        [AiurNoCache]
        public IActionResult Download()
        {
            HttpContext.Response.Headers.Add("Content-Length", Length.ToString());
            return new FileContentResult(GetData(), "application/octet-stream");
        }
    }
}
