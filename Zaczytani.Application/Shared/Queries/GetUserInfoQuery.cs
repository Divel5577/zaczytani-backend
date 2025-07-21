using MediatR;
using Microsoft.AspNetCore.Identity;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Filters;
using Zaczytani.Application.Services;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Shared.Queries;

public class GetUserInfoQuery : IRequest<UserInfoDto>, IUserIdAssignable
{
    private Guid UserId { get; set; }
    public void SetUserId(Guid userId) => UserId = userId;

    public class GetUserInfoQueryHandler(UserManager<User> userManager, IFileStorageRepository fileStorageRepository, IBadgeAssignerService badgeAssignerService ) : IRequestHandler<GetUserInfoQuery, UserInfoDto>
    {
        private readonly IBadgeAssignerService _badgeAssignerService = badgeAssignerService;
        public async Task<UserInfoDto> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId.ToString())
                ?? throw new NotFoundException("User not found");

            var userImage = fileStorageRepository.GetFileUrl(user.Image);

            _badgeAssignerService.SetUserId(request.UserId);
            await _badgeAssignerService.CheckAccountAgeBadgesAsync(cancellationToken);

            return new UserInfoDto(request.UserId, userImage);

        }
    }
}
