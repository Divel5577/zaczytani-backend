using AutoMapper;
using MediatR;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;

public class CreateBookShelfCommand : IRequest<Guid>, IUserIdAssignable
{
    public string Name { get; set; } = string.Empty;

    private Guid UserId { get; set; }

    public void SetUserId(Guid userId)
    {
        UserId = userId;
    }

    private class CreateBookShelfCommandHandler(IBookShelfRepository bookShelfRepository, IMapper mapper) : IRequestHandler<CreateBookShelfCommand, Guid>
    {
        private readonly IBookShelfRepository _bookShelfRepository = bookShelfRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<Guid> Handle(CreateBookShelfCommand request, CancellationToken cancellationToken)
        {
            var bookshelf = _mapper.Map<BookShelf>(request);
            bookshelf.UserId = request.UserId;

            await _bookShelfRepository.AddAsync(bookshelf, cancellationToken);
            await _bookShelfRepository.SaveChangesAsync(cancellationToken);

            return bookshelf.Id;
        }
    }
}
