using AutoMapper;
using MediatR;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Queries;

public record GetReadingBookDetailsQuery(Guid BookId) : IRequest<ReadingBookDto>, IUserIdAssignable
{
    private Guid UserId { get; set; }
    public void SetUserId(Guid userId) => UserId = userId;

    private class GetReadingBookDetailsQueryHandler(IBookRepository bookRepository, IReviewRepository reviewRepository, IFileStorageRepository fileStorageRepository, IMapper mapper) : IRequestHandler<GetReadingBookDetailsQuery, ReadingBookDto>
    {
        private readonly IBookRepository _bookRepository = bookRepository;
        private readonly IReviewRepository _reviewRepository = reviewRepository;
        private readonly IFileStorageRepository _fileStorageRepository = fileStorageRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<ReadingBookDto> Handle(GetReadingBookDetailsQuery request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken)
                ?? throw new NotFoundException($"Book with ID {request.BookId} was not found.");

            var readingBookDto = _mapper.Map<ReadingBookDto>(book);
            readingBookDto.ImageUrl = _fileStorageRepository.GetFileUrl(book.Image);

            var latestReview = await _reviewRepository.GetLatestReviewByBookIdAsync(request.BookId, request.UserId, cancellationToken);

            if (latestReview != null)
            {
                readingBookDto.Progress = latestReview.Progress;
            }

            return readingBookDto;
        }
    }
}
