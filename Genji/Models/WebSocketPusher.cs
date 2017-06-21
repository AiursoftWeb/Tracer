using Genji.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Genji.Models
{
    public class WebSocketPusher : IPusher<WebSocket>
    {
        private WebSocket _ws;
        public bool Connected => _ws.State == WebSocketState.Open;

        public async Task Accept(HttpContext context)
        {
            _ws = await context.WebSockets.AcceptWebSocketAsync();
        }

        public async Task SendMessage(string Message)
        {
            await _ws.SendMessage(Message);
        }
    }
}
