using MediatR;
using Moq;
using Zaczytani.Application.Admin.Commands;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.UnitTests.Handlers;

public class EditBookDetailsCommandHandlerTests
{
    private readonly Mock<IBookRepository> bookRepositoryMock;
    private readonly Mock<IAuthorRepository> authorRepositoryMock;
    private readonly Mock<IPublishingHouseRepository> publishingHouseRepositoryMock;
    private readonly IRequestHandler<EditBookDetailsCommand> handler;

    public EditBookDetailsCommandHandlerTests()
    {
        bookRepositoryMock = new Mock<IBookRepository>();
        authorRepositoryMock = new Mock<IAuthorRepository>();
        publishingHouseRepositoryMock = new Mock<IPublishingHouseRepository>();
        handler = new EditBookDetailsCommand.EditBookDetailsCommandHandler(bookRepositoryMock.Object, authorRepositoryMock.Object, publishingHouseRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldEditBookCorrectly()
    {
        var bookId = Guid.NewGuid();
        var authorName = "Michał Nowak";
        var publishingHouseName = "Nowe wydawnictwo";

        var existingBook = new Book
        {
            Id = bookId,
            Title = "Stary tytuł",
            Isbn = "0000000000000",
            Description = "Stary opis",
            PageNumber = 50,
            ReleaseDate = new DateOnly(2000, 1, 1),
            Image = null,
            Genre = [BookGenre.Horror],
            Series = "Stara seria",
            Authors = [new Author { Name = "Ktoś nikt" }],
            PublishingHouse = new PublishingHouse { Name = "Stare wydawnictwo" }
        };

        var command = new EditBookDetailsCommand
        {
            Title = "Nowy tytuł",
            Isbn = "1234567890123",
            Description = "Zaktualizowany opis",
            PageNumber = 200,
            ReleaseDate = new DateOnly(2023, 12, 31),
            FileName = "new_cover.jpg",
            Authors = [authorName],
            PublishingHouse = publishingHouseName,
            Genre = [BookGenre.Fantasy],
            Series = "Nowa seria"
        };
        command.SetId(bookId);

        authorRepositoryMock.Setup(r => r.GetByNameAsync(authorName)).ReturnsAsync((Author?)null);
        publishingHouseRepositoryMock.Setup(r => r.GetByNameAsync(publishingHouseName)).ReturnsAsync((PublishingHouse?)null);
        publishingHouseRepositoryMock.Setup(r => r.AddAsync(It.IsAny<PublishingHouse>())).Returns(Task.CompletedTask);
        bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>())).ReturnsAsync(existingBook);
        bookRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal(command.Title, existingBook.Title);
        Assert.Equal(command.Isbn, existingBook.Isbn);
        Assert.Equal(command.Description, existingBook.Description);
        Assert.Equal(command.PageNumber, existingBook.PageNumber);
        Assert.Equal(command.ReleaseDate, existingBook.ReleaseDate);
        Assert.Equal(command.FileName, existingBook.Image);
        Assert.Equal(command.Genre, existingBook.Genre);
        Assert.Equal(command.Series, existingBook.Series);
        Assert.Contains(existingBook.Authors, a => a.Name == authorName);
        Assert.Equal(publishingHouseName, existingBook.PublishingHouse.Name);

        publishingHouseRepositoryMock.Verify(r => r.AddAsync(It.IsAny<PublishingHouse>()), Times.Once);
        bookRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
