using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tracer.Models;

public interface IPusher
{
    bool Connected { get; }
    Task Accept(HttpContext context);
    Task SendMessage(string message);
}