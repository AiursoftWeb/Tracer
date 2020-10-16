using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Tracer.Services;

namespace Tracer.Models
{
    public class WebSocketPusher : IPusher
    {
        private WebSocket _ws;
        public bool Connected => _ws.State == WebSocketState.Open;

        public async Task Accept(HttpContext context)
        {
            _ws = await context.WebSockets.AcceptWebSocketAsync();
        }

        public async Task SendMessage(string message)
        {
            await _ws.SendMessage(message);
        }
    }
}
