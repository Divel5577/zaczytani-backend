using AutoMapper;
using MediatR;
using Zaczytani.Application.Filters;
using Zaczytani.Application.Services;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;

public class CreateReviewCommand : IRequest<Guid>, IUserIdAssignable
{
    public string? Content { get; set; }

    public int? Rating { get; set; }

    public int Progress { get; set; }

    public bool IsFinal { get; set; } = false;

    public bool? ContainsSpoilers { get; set; }

    protected Guid BookId { get; set; }

    public void SetBookId(Guid bookId) => BookId = bookId;

    private Guid UserId { get; set; }

    public void SetUserId(Guid userId) => UserId = userId;

    private class CreateReviewCommandHandler(IBookRepository bookRepository, IReviewRepository reviewRepository, IBadgeAssignerService badgeAssignerService, IMapper mapper, IMediator mediator) : IRequestHandler<CreateReviewCommand, Guid>
    {
        private readonly IBookRepository _bookRepository = bookRepository;
        private readonly IReviewRepository _reviewRepository = reviewRepository;
        private readonly IBadgeAssignerService _badgeAssignerService = badgeAssignerService;
        private readonly IMapper _mapper = mapper;
        private readonly IMediator _mediator = mediator;

        public async Task<Guid> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken)
                ?? throw new NotFoundException($"Book with ID {request.BookId} was not found.");

            if (request.Progress > book.PageNumber)
                throw new BadRequestException("Progress cannot be greater than book page number");

            var review = _mapper.Map<Review>(request);

            review.BookId = request.BookId;
            review.UserId = request.UserId;

            await _reviewRepository.AddAsync(review, cancellationToken);
            await _reviewRepository.SaveChangesAsync(cancellationToken);

            _badgeAssignerService.SetUserId(request.UserId);

            if (request.IsFinal || request.Progress == book.PageNumber)
            {
                var command = new ReadBookCommand(request.BookId);
                command.SetUserId(request.UserId);
                await _mediator.Send(command, cancellationToken);
            }

            var count = await _reviewRepository.CountUserReviewsAsync(request.UserId, cancellationToken);
            await _badgeAssignerService.CheckReviewBadgesAsync(count, cancellationToken);

            return review.Id;
        }
    }
}
