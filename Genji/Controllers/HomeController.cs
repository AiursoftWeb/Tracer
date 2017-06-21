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
    }
}
