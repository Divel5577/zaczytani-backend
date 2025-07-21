using MediatR;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Admin.Commands;

public record DeleteBookCommand(Guid Id) : IRequest
{
    public class DeleteBookCommandHandler(IBookRepository bookRepository) : IRequestHandler<DeleteBookCommand>
    {
        private readonly IBookRepository _bookRepository = bookRepository;
        public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException($"Book {request.Id} not found.");

            _bookRepository.Delete(book);
            await _bookRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
