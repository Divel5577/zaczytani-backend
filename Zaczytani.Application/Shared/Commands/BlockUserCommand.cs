using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Zaczytani.Application.Client.Commands;
using Zaczytani.Application.Configuration;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;
namespace Zaczytani.Application.Shared.Commands;

public record BlockUserCommand(Guid UserId,Guid ReportId) : IRequest
{
    private class BlockUserCommandHandler(UserManager<User> userManager, IOptions<UserManagementSettings> settings,IMediator mediator) : IRequestHandler<BlockUserCommand>
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly UserManagementSettings _settings = settings.Value;
        private readonly IMediator _mediator = mediator;

        public async Task Handle(BlockUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString())
                 ?? throw new NotFoundException("User with given ID not found");
           
            user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(_settings.BlockDurationInMinutes);

            await _userManager.UpdateAsync(user);

            await _mediator.Send(new UserBlockedCommand(request.ReportId), cancellationToken);

        }
    }
}
