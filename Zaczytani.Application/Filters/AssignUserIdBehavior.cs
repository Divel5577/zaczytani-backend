using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Zaczytani.Application.Filters;

internal class AssignUserIdBehavior<TRequest, TResponse>(IHttpContextAccessor httpContextAccessor) : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IUserIdAssignable userRequest)
        {
            var user = _httpContextAccessor?.HttpContext?.User
                ?? throw new InvalidOperationException("User context is not present");

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                var userId = Guid.Parse(user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
                userRequest.SetUserId(userId);
            }
        }

        return await next();
    }
}
