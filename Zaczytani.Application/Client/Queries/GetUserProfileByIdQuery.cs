using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Zaczytani.Application.Dtos;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Helpers;
using Zaczytani.Domain.Repositories;
using Zaczytani.Application.Filters;

namespace Zaczytani.Application.Client.Queries;

public record GetUserProfileByIdQuery(Guid UserId) : IRequest<UserProfileDto>, IUserIdAssignable
{
    public Guid CurrentUserId { get; private set; }
    public void SetUserId(Guid userId) => CurrentUserId = userId;

    internal class Handler(
        UserManager<User> userManager,
        IUserRepository userRepository,
        IBookShelfRepository bookShelfRepository,
        IFileStorageRepository fileStorageRepository,
        IChallengeRepository challengeRepository,
        IBadgeRepository badgeRepository,
        IMapper mapper
    ) : IRequestHandler<GetUserProfileByIdQuery, UserProfileDto>
    {
        public async Task<UserProfileDto> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId.ToString())
                ?? throw new NotFoundException("User not found");

            var userImage = fileStorageRepository.GetFileUrl(user.Image);

            var favoriteGenres = await userRepository.GetFavoriteGenresAsync(request.UserId, cancellationToken);
            var bookShelves = await bookShelfRepository.GetAllByUserIdAsync(request.UserId, cancellationToken);

            var readBooksShelf = bookShelves.FirstOrDefault(bs => bs.Type == BookShelfType.Read);
            var currentlyReadingShelf = bookShelves.FirstOrDefault(bs => bs.Type == BookShelfType.Reading);

            var readBookDtos = readBooksShelf?.Books.Select(b =>
            {
                var book = mapper.Map<BookDto>(b);
                book.ImageUrl = fileStorageRepository.GetFileUrl(b.Image);
                return book;
            });

            var currentlyReading = currentlyReadingShelf?.Books.Select(b =>
            {
                var book = mapper.Map<BookDto>(b);
                book.ImageUrl = fileStorageRepository.GetFileUrl(b.Image);
                return book;
            });

            var challengeProgresses = (await challengeRepository
                .GetChallengesWithProgressByUserId(request.UserId, cancellationToken))
                .Take(2)
                .Select(cp =>
                {
                    var dto = mapper.Map<ChallengeProgressDto>(cp);
                    if (dto.Criteria == ChallengeType.Genre &&
                        Enum.TryParse(dto.CriteriaValue, false, out BookGenre bookGenre))
                    {
                        dto.CriteriaValue = EnumHelper.GetEnumDescription(bookGenre);
                    }
                    return dto;
                });

            var followersCount = await userRepository.GetFollowersCountAsync(request.UserId, cancellationToken);
            var followingCount = await userRepository.GetFollowingCountAsync(request.UserId, cancellationToken);
            var isFollowed = await userRepository.IsFollowingAsync(request.CurrentUserId, request.UserId, cancellationToken);

            var badges = await badgeRepository.GetUserBadgesAsync(user.Id, cancellationToken);
            var badgeDtos = badges.Select(ub => new BadgeDto
            {
                Id = ub.Badge.Id,
                Name = ub.Badge.Name,
                IconUrl = ub.Badge.IconPath,
                Type = ub.Badge.Type.ToString()
            }).ToList();

            return new UserProfileDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                ImageUrl = userImage,
                TotalBooksRead = readBooksShelf?.Books.Count ?? 0,
                FavoriteGenres = favoriteGenres.Select(g => g.ToString()).ToList(),
                ReadBooks = readBookDtos ?? [],
                CurrentlyReading = currentlyReading ?? [],
                Badges = badgeDtos,
                ChallengeProgresses = challengeProgresses,
                FollowersCount = followersCount,
                FollowingCount = followingCount,
                IsFollowed = isFollowed
            };
        }
    }
}
