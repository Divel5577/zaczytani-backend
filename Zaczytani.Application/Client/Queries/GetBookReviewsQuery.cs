using AutoMapper;
using MediatR;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Queries;

public record GetBookReviewsQuery(Guid BookId) : IRequest<IEnumerable<BookReviewDto>>, IUserIdAssignable
{
    private Guid UserId { get; set; }
    public void SetUserId(Guid userId) => UserId = userId;

    private class GetBookReviewsQueryHandler(IReviewRepository reviewRepository, IMapper mapper) : IRequestHandler<GetBookReviewsQuery, IEnumerable<BookReviewDto>>
    {
        private readonly IReviewRepository _reviewRepository = reviewRepository;
        private readonly IMapper _mapper = mapper;
        public async Task<IEnumerable<BookReviewDto>> Handle(GetBookReviewsQuery request, CancellationToken cancellationToken)
        {
            var finalReviews = await _reviewRepository.GetFinalReviewsByBookId(request.BookId, cancellationToken);
            var reviewDtos = new List<BookReviewDto>();

            foreach (var finalReview in finalReviews)
            {
                var reviewDto = _mapper.Map<BookReviewDto>(finalReview);
                var notes = await _reviewRepository.GetReviewsByBookIdAndUserId(request.BookId, reviewDto.User.Id, cancellationToken);

                reviewDto.NotesCount = notes.Count();
                reviewDto.IsLiked = finalReview.Likes.Any(l => l == request.UserId);
                reviewDtos.Add(reviewDto);
            }

            return reviewDtos;
        }
    }
}
