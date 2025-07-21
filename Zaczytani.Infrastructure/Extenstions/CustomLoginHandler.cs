using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zaczytani.Domain.Constants;
using Zaczytani.Domain.Entities;

namespace Zaczytani.Infrastructure.Extenstions;
public class CustomLoginHandler : SignInManager<User>
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger<SignInManager<User>> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IUserClaimsPrincipalFactory<User> _claimsFactory;

    public CustomLoginHandler(UserManager<User> userManager, IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<User> claimsFactory, IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<User>> logger, IAuthenticationSchemeProvider schemes,
        IUserConfirmation<User> confirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
        _contextAccessor = contextAccessor;
        _logger = logger;
    }

    public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        var user = await UserManager.FindByNameAsync(userName) ?? await UserManager.FindByEmailAsync(userName);
        if (user == null)
        {
            return SignInResult.Failed;
        }
        var roles = await UserManager.GetRolesAsync(user);

        var context = _contextAccessor.HttpContext;

        if (roles.Contains(UserRoles.Admin) && (!context.Request.Headers["User-Agent"].ToString().Contains("Mobile") && !context.Request.Headers["User-Agent"].ToString().Contains("okhttp")))
        {
            _logger.LogWarning($"[CustomLoginHandler] Użytkownik {userName} jest adminem i próbuje zalogować się na platformie web.");
            return SignInResult.Failed;
        }

        if (roles.Contains(UserRoles.User) && (context.Request.Headers["User-Agent"].ToString().Contains("Mobile") || context.Request.Headers["User-Agent"].ToString().Contains("okhttp")))
        {
            _logger.LogWarning($"[CustomLoginHandler] Użytkownik {userName} jest userem i próbuje zalogować się na mobilce.");
            return SignInResult.Failed;
        }
        return await base.PasswordSignInAsync(user.UserName, password, isPersistent, lockoutOnFailure);
    }
}