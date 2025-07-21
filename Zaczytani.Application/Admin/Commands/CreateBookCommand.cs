using MediatR;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Admin.Commands;

public class CreateBookCommand : IRequest<Guid>, IUserIdAssignable
{
    public string Title { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PageNumber { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public string? FileName { get; set; } = string.Empty;
    public List<string> Authors { get; set; } = [];
    public string PublishingHouse { get; set; } = string.Empty;
    public List<BookGenre> Genre { get; set; } = [];
    public string? Series { get; set; } = string.Empty;

    private Guid UserId { get; set; }

    public void SetUserId(Guid userId)
    {
        UserId = userId;
    }

    public class CreateBookCommandHandler(IBookRepository bookRepository, IAuthorRepository authorRepository, IPublishingHouseRepository publishingHouseRepository) : IRequestHandler<CreateBookCommand, Guid>
    {
        private readonly IBookRepository _bookRepository = bookRepository;
        private readonly IAuthorRepository _authorRepository = authorRepository;
        private readonly IPublishingHouseRepository _publishingHouseRepository = publishingHouseRepository;
        public async Task<Guid> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var book = new Book
            {
                Title = request.Title,
                Description = request.Description,
                Isbn = request.Isbn,
                PageNumber = request.PageNumber,
                Image = request.FileName,
                UserId = request.UserId,
                Genre = request.Genre,
                Series = request.Series,
                ReleaseDate = request.ReleaseDate,
            };

            foreach (var authorName in request.Authors)
            {
                var author = await _authorRepository.GetByNameAsync(authorName);

                author ??= new Author
                {
                    Name = authorName
                };

                book.Authors.Add(author);
            }

            var publishingHouse = await _publishingHouseRepository.GetByNameAsync(request.PublishingHouse);

            publishingHouse ??= new PublishingHouse
            {
                Name = request.PublishingHouse
            };

            book.PublishingHouse = publishingHouse;

            await _bookRepository.AddAsync(book);
            await _bookRepository.SaveChangesAsync(cancellationToken);

            return book.Id;
        }
    }
}
