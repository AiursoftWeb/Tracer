using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genji.Attributes
{
    public class ForceWS : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (!context.HttpContext.WebSockets.IsWebSocketRequest)
            {
                throw new Exception();
            }
        }
    }
}
