using MediatR;
using System;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;
public record DeleteBookShelfCommand(Guid ShelfId) : IRequest, IUserIdAssignable
{
    public Guid UserId { get; private set; }

    public void SetUserId(Guid userId)
    {
        UserId = userId;
    }

    private class Handler(IBookShelfRepository repository) : IRequestHandler<DeleteBookShelfCommand>
    {
        private readonly IBookShelfRepository _repository = repository;

        public async Task Handle(DeleteBookShelfCommand request, CancellationToken cancellationToken)
        {
            var bookshelf = await _repository.GetByIdAsync(request.ShelfId, cancellationToken);

            if (bookshelf == null || bookshelf.UserId != request.UserId)
                throw new KeyNotFoundException("Bookshelf not found or you do not have access to it.");

            if (bookshelf.IsDefault)
            {
                throw new InvalidOperationException("Cannot delete a default bookshelf.");
            }

            await _repository.DeleteAsync(request.ShelfId, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
        }
    }
}
