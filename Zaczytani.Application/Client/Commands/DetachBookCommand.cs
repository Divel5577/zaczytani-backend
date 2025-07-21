using MediatR;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;

public record DetachBookCommand(Guid BookShelfId, Guid BookId) : IRequest
{
    public class DetachBookCommanddHandler(IBookRepository bookRepository, IBookShelfRepository bookShelfRepository, IMediator mediator) : IRequestHandler<DetachBookCommand>
    {
        private readonly IBookRepository _bookRepository = bookRepository;
        private readonly IBookShelfRepository _bookShelfRepository = bookShelfRepository;
        private readonly IMediator _mediator = mediator;

        public async Task Handle(DetachBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken)
                ?? throw new NotFoundException("Book with given ID not found");
            var bookShelf = await _bookShelfRepository.GetByIdAsync(request.BookShelfId, cancellationToken)
                ?? throw new NotFoundException("Shelf with given ID not found");

            bookShelf.Books.Remove(book);
            await _bookRepository.SaveChangesAsync(cancellationToken);

            if (bookShelf.Type == BookShelfType.Read)
            {
                var updateProgressCommand = new UpdateChallengeProgressesCommand(book.Id, false);
                await _mediator.Send(updateProgressCommand, cancellationToken);
            }
        }
    }
}
