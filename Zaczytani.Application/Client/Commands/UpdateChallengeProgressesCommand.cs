using MediatR;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;

public record UpdateChallengeProgressesCommand(Guid BookId, bool Increment) : IRequest, IUserIdAssignable
{
    private Guid UserId { get; set; }
    public void SetUserId(Guid userId) => UserId = userId;

    private class UpdateChallengeCommandHandler(IBookRepository bookRepository, IChallengeRepository challengeRepository) : IRequestHandler<UpdateChallengeProgressesCommand>
    {
        private readonly IBookRepository _bookRepository = bookRepository;
        private readonly IChallengeRepository _challengeRepository = challengeRepository;

        public async Task Handle(UpdateChallengeProgressesCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken)
                ?? throw new NotFoundException("Book with the provided ID not found");

            var progresses = await _challengeRepository.GetChallengesWithProgressByUserId(request.UserId, cancellationToken);

            foreach (var progress in progresses)
            {
                if (ShouldUpdateProgress(progress, book))
                {
                    progress.BooksRead = Math.Max(0, progress.BooksRead + (request.Increment ? 1 : -1));
                }
            }

            await _challengeRepository.SaveChangesAsync(cancellationToken);
        }

        private static bool ShouldUpdateProgress(ChallengeProgress progress, Book book)
        {
            return progress.Challenge.Criteria switch
            {
                ChallengeType.BooksCount => true,
                ChallengeType.Genre => book.Genre.Any(g => g == Enum.Parse<BookGenre>(progress.Challenge.CriteriaValue!, true)),
                ChallengeType.Author => book.Authors.Any(a => a.Name == progress.Challenge.CriteriaValue),
                _ => false
            };
        }
    }
}
