using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Client.Commands;

public record GetRandomBookCommand : IRequest<BookDto>, IUserIdAssignable
{
    public Guid UserId { get; private set; }

    public void SetUserId(Guid userId)
    {
        UserId = userId;
    }

    private class Handler(IUserDrawnBookRepository userDrawnBookRepository, IBookRepository bookRepository, IFileStorageRepository fileStorageRepository, IMapper mapper) : IRequestHandler<GetRandomBookCommand, BookDto>
    {
        private readonly IUserDrawnBookRepository _userDrawnBookRepository = userDrawnBookRepository;
        private readonly IBookRepository _bookRepository = bookRepository;
        private readonly IFileStorageRepository _fileStorageRepository = fileStorageRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<BookDto> Handle(GetRandomBookCommand request, CancellationToken cancellationToken)
        {
            var existingDrawnBook = await _userDrawnBookRepository.GetDrawnBookByUserIdAndDateAsync(request.UserId, DateTime.UtcNow.Date, cancellationToken);
            if (existingDrawnBook != null)
            {
                var bookDto = _mapper.Map<BookDto>(existingDrawnBook.Book);
                bookDto.ImageUrl = _fileStorageRepository.GetFileUrl(existingDrawnBook.Book.Image);
                return bookDto;
            }

            var unseenBooksQuery = _bookRepository.GetUnseenBooks(request.UserId);

            var randomBook = await unseenBooksQuery
                .OrderBy(_ => Guid.NewGuid())
                .Include(b => b.Authors)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new InvalidOperationException("No unseen books available to draw.");

            var userDrawnBook = new UserDrawnBook
            {
                UserId = request.UserId,
                BookId = randomBook.Id,
                DrawnDate = DateTime.UtcNow.Date
            };

            await _userDrawnBookRepository.AddAsync(userDrawnBook, cancellationToken);
            await _userDrawnBookRepository.SaveChangesAsync(cancellationToken);

            var bookDtoWithImage = _mapper.Map<BookDto>(randomBook);
            bookDtoWithImage.ImageUrl = _fileStorageRepository.GetFileUrl(randomBook.Image);

            return bookDtoWithImage;
        }
    }
}