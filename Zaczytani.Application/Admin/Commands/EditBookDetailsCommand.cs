using MediatR;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Admin.Commands;

public record EditBookDetailsCommand : IRequest
{
    private Guid Id { get; set; }
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
    public void SetId(Guid id)
    {
        Id = id;
    }

    public class EditBookDetailsCommandHandler(IBookRepository bookRepository, IAuthorRepository authorRepository, IPublishingHouseRepository publishingHouseRepository) : IRequestHandler<EditBookDetailsCommand>
    {
        private readonly IBookRepository _bookRepository = bookRepository;
        private readonly IAuthorRepository _authorRepository = authorRepository;
        private readonly IPublishingHouseRepository _publishingHouseRepository = publishingHouseRepository;

        public async Task Handle(EditBookDetailsCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException($"Book {request.Id} not found.");

            book.Title = request.Title;
            book.Isbn = request.Isbn;
            book.Description = request.Description;
            book.PageNumber = request.PageNumber;
            book.ReleaseDate = request.ReleaseDate;
            book.Image = request.FileName;
            book.Genre = request.Genre;
            book.Series = request.Series;
            book.Authors = [];

            foreach (var authorName in request.Authors)
            {
                var author = await _authorRepository.GetByNameAsync(authorName);

                author ??= new Author()
                {
                    Name = authorName
                };

                book.Authors.Add(author);
            }

            var publishingHouse = await _publishingHouseRepository.GetByNameAsync(request.PublishingHouse);

            if (publishingHouse == null)
            {
                publishingHouse = new PublishingHouse
                {
                    Name = request.PublishingHouse
                };

                await _publishingHouseRepository.AddAsync(publishingHouse);
            }

            book.PublishingHouse = publishingHouse;

            await _bookRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
