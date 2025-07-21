using AutoMapper;
using MediatR;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Queries;

public record GetRecommendedBooksQuery(int PageSize, BookGenre? BookGenre, string? AuthorName) : IRequest<IEnumerable<BookDto>>, IUserIdAssignable
{
    private Guid UserId { get; set; }
    public void SetUserId(Guid userId) => UserId = userId;

    private class GetRecommendedBooksQueryHandler(IBookRepository bookRepository, IFileStorageRepository fileStorageRepository, IBookShelfRepository bookShelfRepository, IMapper mapper) : IRequestHandler<GetRecommendedBooksQuery, IEnumerable<BookDto>>
    {
        private readonly IBookRepository _bookRepository = bookRepository;
        private readonly IFileStorageRepository _fileStorageRepository = fileStorageRepository;
        private readonly IBookShelfRepository _bookShelfRepository = bookShelfRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<BookDto>> Handle(GetRecommendedBooksQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Book> books = [];

            if(request.BookGenre == null && string.IsNullOrEmpty(request.AuthorName))
            {
                books = await _bookRepository.GetUserRecommendedBooksAsync(request.UserId, request.PageSize, cancellationToken);
            }
            else
            {
                books = await _bookRepository.GetUserRecommendedBooksAsync(request.UserId, request.BookGenre, request.AuthorName, request.PageSize, cancellationToken);
            }

            var bookDtos = books.Select(b =>
            {
                var bookDto = _mapper.Map<BookDto>(b);
                bookDto.ImageUrl = _fileStorageRepository.GetFileUrl(b.Image);
                bookDto.Readers = _bookShelfRepository.GetBookCountOnReadShelf(b.Id);
                return bookDto;
            });

            return bookDtos;
        }
    }
}
