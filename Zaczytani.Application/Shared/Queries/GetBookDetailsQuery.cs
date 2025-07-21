using AutoMapper;
using MediatR;
using Zaczytani.Application.Dtos;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Shared.Queries;

public record GetBookDetailsQuery(Guid BookId) : IRequest<BookDto>
{
    public class GetBookDetailsQueryHandler(IBookRepository bookRepository, IBookShelfRepository bookShelfRepository, IFileStorageRepository fileStorageRepository, IMapper mapper)
        : IRequestHandler<GetBookDetailsQuery, BookDto>
    {
        private readonly IBookShelfRepository _bookShelfRepository = bookShelfRepository;
        private readonly IBookRepository _bookRepository = bookRepository;
        private readonly IFileStorageRepository _fileStorageRepository = fileStorageRepository;
        private readonly IMapper _mapper = mapper;
        public async Task<BookDto> Handle(GetBookDetailsQuery request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken)
                ?? throw new NotFoundException($"Book with ID {request.BookId} was not found.");

            var bookDto = _mapper.Map<BookDto>(book);
            bookDto.ImageUrl = _fileStorageRepository.GetFileUrl(book.Image);
            bookDto.Readers = await _bookShelfRepository.GetBookCountOnReadShelfAsync(book.Id, cancellationToken);

            return bookDto;
        }
    }
}
