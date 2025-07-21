using AutoMapper;
using MediatR;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Queries;
public class GetAllBookshelvesQuery : IRequest<IEnumerable<BookShelfDto>>, IUserIdAssignable
{
    private Guid UserId { get; set; }

    public void SetUserId(Guid userId) => UserId = userId;

    private class Handler(IBookShelfRepository bookshelfRepository, IMapper mapper, IFileStorageRepository fileStorageRepository) : IRequestHandler<GetAllBookshelvesQuery, IEnumerable<BookShelfDto>>
    {
        private readonly IBookShelfRepository _bookshelfRepository = bookshelfRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IFileStorageRepository _fileStorageRepository = fileStorageRepository;

        public async Task<IEnumerable<BookShelfDto>> Handle(GetAllBookshelvesQuery request, CancellationToken cancellationToken)
        {
            var bookshelves = await _bookshelfRepository.GetAllByUserIdAsync(request.UserId, cancellationToken);
            var bookshelvesDtos = _mapper.Map<IEnumerable<BookShelfDto>>(bookshelves);

            foreach (var bookshelfDto in bookshelvesDtos)
            {
                var books = await _bookshelfRepository.GetTopBooksByShelfIdAsync(bookshelfDto.Id, 2, cancellationToken);

                bookshelfDto.ImageUrl = books
                    .Select(b => _fileStorageRepository.GetFileUrl(b.Image))
                    .ToList();
            }

            return bookshelvesDtos;
        }
    }
}
