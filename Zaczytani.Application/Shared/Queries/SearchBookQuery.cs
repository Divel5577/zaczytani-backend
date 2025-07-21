using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zaczytani.Application.Dtos;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Shared.Queries;

public class SearchBookQuery : IRequest<IEnumerable<SearchBookDto>>
{
    public string SearchPhrase { get; set; } = string.Empty;

    private class SearchBookQueryHandler(IBookRepository bookRepository, IBookShelfRepository bookShelfRepository, IFileStorageRepository fileStorageRepository, IMapper mapper) : IRequestHandler<SearchBookQuery, IEnumerable<SearchBookDto>>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IBookRepository _bookRepository = bookRepository;
        private readonly IFileStorageRepository _fileStorageRepository = fileStorageRepository;
        private readonly IBookShelfRepository _bookShelfRepository = bookShelfRepository;

        public async Task<IEnumerable<SearchBookDto>> Handle(SearchBookQuery request, CancellationToken cancellationToken)
        {
            var books = await _bookRepository.GetBySearchPhraseWithAuthors(request.SearchPhrase)
                .Include(b => b.Authors)
                .Include(b => b.PublishingHouse)
                .Include(b => b.Reviews)
                .ToListAsync(cancellationToken);

            var result = books
                .SelectMany(b => b.Authors.Select(a => new { Author = a, Book = b }))
                .GroupBy(x => x.Author)
                .Select(g => new SearchBookDto(
                    g.Key.Id,
                    g.Key.Name,
                    g.Key.Image is not null ? _fileStorageRepository.GetFileUrl(g.Key.Image) : null,
                    g.Select(x =>
                    {
                        var bookDto = _mapper.Map<BookSearchDto>(x.Book);
                        bookDto.ImageUrl = _fileStorageRepository.GetFileUrl(x.Book.Image);
                        bookDto.Readers = _bookShelfRepository.GetBookCountOnReadShelf(x.Book.Id);
                        return bookDto;
                    })
                ));

            return result;
        }
    }
}
