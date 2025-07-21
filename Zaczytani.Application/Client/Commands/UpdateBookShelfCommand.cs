using MediatR;
using AutoMapper;
using Zaczytani.Domain.Repositories;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Entities;

namespace Zaczytani.Application.Client.Commands;
public record UpdateBookShelfCommand(Guid ShelfId, string Name, string Description) : IRequest, IUserIdAssignable
{
    public Guid UserId { get; private set; }

    public void SetUserId(Guid userId)
    {
        UserId = userId;
    }

    private class Handler(IBookShelfRepository repository, IMapper mapper) : IRequestHandler<UpdateBookShelfCommand>
    {
        private readonly IBookShelfRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task Handle(UpdateBookShelfCommand request, CancellationToken cancellationToken)
        {
            var bookshelf = await _repository.GetByIdAsync(request.ShelfId, cancellationToken);

            if (bookshelf == null || bookshelf.UserId != request.UserId)
                throw new KeyNotFoundException("Bookshelf not found or you do not have access to it.");

            if (bookshelf.IsDefault)
            {
                if (bookshelf.Name != request.Name)
                {
                    throw new InvalidOperationException("Default bookshelves cannot have their name changed.");
                }
            }
            else
            {
                bookshelf.Name = request.Name;
            }

            await _repository.UpdateAsync(bookshelf, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
        }
    }
}
