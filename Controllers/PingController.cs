using Aiursoft.Pylon.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tracer.Controllers
{
    public class PingController : Controller
    {
        [AiurNoCache]
        public JsonResult Index()
        {
            return Json(new List<object>());
        }
    }
}
