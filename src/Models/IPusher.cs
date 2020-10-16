using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Tracer.Models
{
    public interface IPusher
    {
        bool Connected { get; }
        Task Accept(HttpContext context);
        Task SendMessage(string message);
    }
}
