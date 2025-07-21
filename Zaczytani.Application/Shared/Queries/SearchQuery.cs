using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zaczytani.Application.Dtos;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Shared.Queries;

public record SearchQuery(string SearchPhrase) : IRequest<SearchDto>
{
    private class SearchQueryHandler(IBookRepository bookRepository, IUserRepository userRepository, IAuthorRepository authorRepository, IMapper mapper) : IRequestHandler<SearchQuery, SearchDto>
    {
        private readonly IBookRepository _bookRepository = bookRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IAuthorRepository _authorRepository = authorRepository;
        private readonly IMapper _mapper = mapper;
        public async Task<SearchDto> Handle(SearchQuery request, CancellationToken cancellationToken)
        {
            var bookDtos = await _bookRepository.GetBySearchPhrase(request.SearchPhrase)
                .Include(b => b.Authors)
                .Include(b => b.PublishingHouse)
                .Include(b => b.Reviews)
                .ProjectTo<BookSearchAuthorsDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            var authorDtos = await _authorRepository.GetBySearchPhrase(request.SearchPhrase)
                .Include(a => a.Books)
                .ProjectTo<AuthorSearchDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            var userDtos = await _userRepository.GetBySearchPhrase(request.SearchPhrase)
                .Include(u => u.BookShelves)
                    .ThenInclude(bs => bs.Books)
                .ProjectTo<UserSearchDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            var searchDto = new SearchDto(bookDtos, authorDtos, userDtos);

            return searchDto;
        }
    }
}
