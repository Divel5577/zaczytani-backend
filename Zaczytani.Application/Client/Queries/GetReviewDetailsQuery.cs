using AutoMapper;
using MediatR;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Queries;

public record GetReviewDetailsQuery(Guid Id) : IRequest<ReviewDetailsDto>, IUserIdAssignable
{
    private Guid UserId { get; set; }
    public void SetUserId(Guid userId) => UserId = userId;

    private class GetReviewDetailsQueryHandler(IReviewRepository reviewRepository, IFileStorageRepository fileStorageRepository, IMapper mapper) : IRequestHandler<GetReviewDetailsQuery, ReviewDetailsDto>
    {
        private readonly IFileStorageRepository _fileStorageRepository = fileStorageRepository;
        private readonly IReviewRepository _reviewRepository = reviewRepository;
        private readonly IMapper _mapper = mapper;
        public async Task<ReviewDetailsDto> Handle(GetReviewDetailsQuery request, CancellationToken cancellationToken)
        {
            var finalReview = await _reviewRepository.GetFinalReviewByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Review with given ID not found");

            var reviewDto = _mapper.Map<ReviewDetailsDto>(finalReview);
            var reviews = await _reviewRepository.GetReviewsByBookIdAndUserId(reviewDto.Book.Id, reviewDto.User.Id, cancellationToken);

            reviewDto.Book.ImageUrl = _fileStorageRepository.GetFileUrl(finalReview.Book.Image);
            reviewDto.Notes = _mapper.Map<IEnumerable<NoteDto>>(reviews);
            reviewDto.IsLiked = finalReview.Likes.Any(l => l == request.UserId);

            return reviewDto;
        }
    }
}
