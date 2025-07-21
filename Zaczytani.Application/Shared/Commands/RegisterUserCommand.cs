using System.Text;
using System.Text.Encodings.Web;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Zaczytani.Application.Services;
using Zaczytani.Domain.Constants;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Shared.Commands;

public class RegisterUserCommand : IRequest
{
    public string Login { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }

    public class RegisterUserCommandHandler(UserManager<User> userManager, IUserStore<User> userStore, IEmailSender<User> emailSender, IConfiguration configuration, IBookShelfRepository bookShelfRepository, IBadgeAssignerService badgeAssignerService) : IRequestHandler<RegisterUserCommand>
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IUserStore<User> _userStore = userStore;
        private readonly IUserEmailStore<User> _emailStore = (IUserEmailStore<User>)userStore;
        private readonly IEmailSender<User> _emailSender = emailSender;
        private readonly IBookShelfRepository _bookShelfRepository = bookShelfRepository;
        private readonly IBadgeAssignerService _badgeAssignerService = badgeAssignerService;
        private readonly string _frontendUrl = configuration.GetSection("FrontendUrl").Value
            ?? throw new InvalidOperationException("Frontend URL is not configured. Please set 'FrontendUrl' in appsettings.json.");

        public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
            };
            await _userStore.SetUserNameAsync(user, request.Login, cancellationToken);
            await _emailStore.SetEmailAsync(user, request.Email, cancellationToken);
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                throw new BadRequestException(result.Errors.First().Description);
            }

            await _userManager.AddToRoleAsync(user, UserRoles.User);

            await CreateDefaultBookShelves(user.Id, cancellationToken);

            _badgeAssignerService.SetUserId(user.Id);
            await _badgeAssignerService.CheckAccountAgeBadgesAsync(cancellationToken);

            await SendConfirmationEmailAsync(user, request.Email);
        }

        private async Task CreateDefaultBookShelves(Guid userId, CancellationToken cancellationToken)
        {
            var reading = BookShelf.CreateCurrentlyReading(userId);
            var read = BookShelf.CreateRead(userId);
            var toRead = BookShelf.CreateToRead(userId);

            await _bookShelfRepository.AddAsync(reading, cancellationToken);
            await _bookShelfRepository.AddAsync(read, cancellationToken);
            await _bookShelfRepository.AddAsync(toRead, cancellationToken);
            await _bookShelfRepository.SaveChangesAsync(cancellationToken);
        }

        private async Task SendConfirmationEmailAsync(User user, string email)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var routeValues = new RouteValueDictionary()
            {
                ["userId"] = user.Id,
                ["code"] = code,
            };

            var confirmEmailUrl = BuildEmailConfirmationUrl(routeValues);

            await _emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(confirmEmailUrl));
        }

        private string BuildEmailConfirmationUrl(RouteValueDictionary routeValues)
        {
            var queryString = string.Join("&", routeValues.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value.ToString())}"));
            return $"{_frontendUrl}/auth/confirmEmail?{queryString}";
        }
    }
}
