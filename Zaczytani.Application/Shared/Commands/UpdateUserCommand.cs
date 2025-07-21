using MediatR;
using Microsoft.AspNetCore.Identity;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Exceptions;

namespace Zaczytani.Application.Shared.Commands;

public class UpdateUserCommand : IRequest, IUserIdAssignable
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? FileName { get; set; }

    private Guid UserId { get; set; }
    public void SetUserId(Guid userId) => UserId = userId;

    private class UpdateUserCommandHandler(UserManager<User> userManager) : IRequestHandler<UpdateUserCommand>
    {
        private readonly UserManager<User> _userManager = userManager;

        public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString())
                ?? throw new NotFoundException("User not found.");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Image = request.FileName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new BadRequestException(result.Errors.First().Description);
            }
        }
    }
}
