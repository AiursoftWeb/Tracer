using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Genji.Attributes;
using System.Net.WebSockets;
using Genji.Models;

namespace Genji.Controllers
{
    public class HomeController : Controller
    {
        private IPusher<WebSocket> _pusher;
        public HomeController()
        {
            _pusher = new WebSocketPusher();
        }

        public IActionResult Index()
        {
            return View();
        }

        [ForceWS]
        public async Task<IActionResult> Pushing()
        {
            await _pusher.Accept(HttpContext);
            while (_pusher.Connected)
            {
                try
                {
                    await _pusher.SendMessage(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffffff"));
                    await Task.Delay(10);
                }
                catch
                {
                    break;
                }
            }
            return null;
        }

        public IActionResult Ping()
        {
            return Json(new { message = "ok" });
        }

        public IActionResult Download()
        {
            int length = 1024 * 1024 * 3;
            var file = new byte[length];
            for (int i = 0; i < length; i++)
            {
                file[i] = 1;
            }
            HttpContext.Response.Headers.Add("Content-Length", length.ToString());
            HttpContext.Response.Headers.Add("cache-control", "no-cache");
            return new FileContentResult(file, "application/octet-stream");
        }

        public IActionResult Address()
        {
            return Json(new
            {
                localIpAddress = HttpContext.Connection.LocalIpAddress,
                remoteIpAddress = HttpContext.Connection.RemoteIpAddress
            });
        }
    }
}
