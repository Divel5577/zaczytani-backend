using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Zaczytani.Application.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SetUserIdAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            context.HttpContext.Items["UserId"] = userId;
        }
        else
        {
            context.Result = new UnauthorizedResult();
        }

        base.OnActionExecuting(context);
    }
}