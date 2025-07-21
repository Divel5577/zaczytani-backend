using AutoMapper;
using MediatR;
using Zaczytani.Domain.Repositories;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Filters;

namespace Zaczytani.Application.Client.Queries;
public record GetBookShelfQuery(Guid Id) : IRequest<BookShelfDto>, IUserIdAssignable
{
    public Guid UserId { get; private set; }

    public void SetUserId(Guid userId) => UserId = userId;

    private class Handler(IBookShelfRepository repository, IMapper mapper) : IRequestHandler<GetBookShelfQuery, BookShelfDto>
    {
        private readonly IBookShelfRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<BookShelfDto> Handle(GetBookShelfQuery request, CancellationToken cancellationToken)
        {
            var bookshelf = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (bookshelf == null || bookshelf.UserId != request.UserId)
                throw new UnauthorizedAccessException("You do not have access to view this shelf.");

            return _mapper.Map<BookShelfDto>(bookshelf);
        }
    }
}
