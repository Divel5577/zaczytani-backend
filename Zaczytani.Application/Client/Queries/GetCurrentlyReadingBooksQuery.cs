using AutoMapper;
using MediatR;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Queries;

public class GetCurrentlyReadingBooksQuery : IRequest<IEnumerable<ReadingBookDto>>, IUserIdAssignable
{
    public Guid UserId { get; private set; }

    public void SetUserId(Guid userId) => UserId = userId;

    private class Handler(IBookShelfRepository bookShelfRepository, IReviewRepository reviewRepository, IFileStorageRepository fileStorageRepository, IMapper mapper) : IRequestHandler<GetCurrentlyReadingBooksQuery, IEnumerable<ReadingBookDto>>
    {
        private readonly IBookShelfRepository _bookShelfRepository = bookShelfRepository;
        private readonly IReviewRepository _reviewRepository = reviewRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IFileStorageRepository _fileStorageRepository = fileStorageRepository;

        public async Task<IEnumerable<ReadingBookDto>> Handle(GetCurrentlyReadingBooksQuery request, CancellationToken cancellationToken)
        {
            var readingBookShelf = await _bookShelfRepository.GetBookShelfByTypeAsync(BookShelfType.Reading, request.UserId, cancellationToken)
                ?? throw new NotFoundException("Reading BookShelf not found");

            var currentlyReadingBooks = _mapper.Map<IEnumerable<ReadingBookDto>>(readingBookShelf.Books);

            foreach (var (dto, book) in currentlyReadingBooks.Zip(readingBookShelf.Books, (dto, book) => (dto, book)))
            {
                dto.ImageUrl = _fileStorageRepository.GetFileUrl(book.Image);
                var review = await _reviewRepository.GetLatestReviewByBookIdAsync(dto.Id, request.UserId, cancellationToken);
                dto.Progress = review is not null ? review.Progress : 0;
            }
        
            return currentlyReadingBooks;
        }
    }
}
