using AutoMapper;
using MediatR;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Queries;

public record GetBooksOnShelfQuery(Guid Id) : IRequest<IEnumerable<BookDto>>, IUserIdAssignable
{
    public Guid UserId { get; private set; }

    public void SetUserId(Guid userId) => UserId = userId;

    private class Handler(IBookShelfRepository bookshelfRepository, IMapper mapper, IFileStorageRepository fileStorageRepository) : IRequestHandler<GetBooksOnShelfQuery, IEnumerable<BookDto>>
    {
        private readonly IBookShelfRepository _bookshelfRepository = bookshelfRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IFileStorageRepository _fileStorageRepository = fileStorageRepository;

        public async Task<IEnumerable<BookDto>> Handle(GetBooksOnShelfQuery request, CancellationToken cancellationToken)
        {
            var bookshelf = await _bookshelfRepository.GetByIdWithBooksAsync(request.Id, request.UserId, cancellationToken)
                ?? throw new NotFoundException("Bookshelf not found or access denied.");
            var bookDtos = bookshelf.Books.Select(book =>
            {
                var bookDto = _mapper.Map<BookDto>(book);
                if (bookshelf.Type == BookShelfType.Read)
                {
                    bookDto.Rating = book.Reviews
                        .Where(r => r.IsFinal 
                            && r.UserId == request.UserId 
                            && r.Rating is not null)
                        .Average(r => r.Rating);
                }
                bookDto.ImageUrl = _fileStorageRepository.GetFileUrl(book.Image);
                bookDto.Readers = _bookshelfRepository.GetBookCountOnReadShelf(book.Id);
                return bookDto;
            });

            return bookDtos;
        }
    }
}
