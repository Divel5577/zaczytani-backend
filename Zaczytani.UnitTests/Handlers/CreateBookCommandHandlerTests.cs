using Moq;
using Zaczytani.Application.Admin.Commands;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Repositories;
using static Zaczytani.Application.Admin.Commands.CreateBookCommand;

namespace Zaczytani.UnitTests.Handlers;

public class CreateBookCommandHandlerTests
{
    private readonly Mock<IBookRepository> bookRepositoryMock;
    private readonly Mock<IAuthorRepository> authorRepositoryMock;
    private readonly Mock<IPublishingHouseRepository> publishingHouseRepositoryMock;
    private readonly CreateBookCommandHandler handler;
    public CreateBookCommandHandlerTests()
    {
        bookRepositoryMock = new Mock<IBookRepository>();
        authorRepositoryMock = new Mock<IAuthorRepository>();
        publishingHouseRepositoryMock = new Mock<IPublishingHouseRepository>();
        handler = new CreateBookCommandHandler(bookRepositoryMock.Object,authorRepositoryMock.Object,publishingHouseRepositoryMock.Object);
    }
    [Fact]
    public async Task Handle_ShouldCreateBookAndReturnId()
    {
        var authorName = "Jan Kowalski";
        var publishingHouseName = "Wydawnictwo test";
        var userId = Guid.NewGuid();
        var command = new CreateBookCommand
        {
            Title = "Testowy tytuł",
            Isbn = "1234567890123",
            Description = "Testowy opis",
            PageNumber = 100,
            ReleaseDate = new DateOnly(2020, 1, 1),
            FileName = "cover.jpg",
            Authors = [authorName],
            PublishingHouse = publishingHouseName,
            Genre = [BookGenre.Fantasy],
            Series = "Testowa seria"
        };
        command.SetUserId(userId);

        var author = new Author { Id = Guid.NewGuid(), Name = authorName };
        var publishingHouse = new PublishingHouse { Id = Guid.NewGuid(), Name = publishingHouseName };

        authorRepositoryMock.Setup(r => r.GetByNameAsync(authorName)).ReturnsAsync((Author?)null);
        publishingHouseRepositoryMock.Setup(r => r.GetByNameAsync(publishingHouseName)).ReturnsAsync((PublishingHouse?)null);

        bookRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);
        bookRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await handler.Handle(command, CancellationToken.None);

        bookRepositoryMock.Verify(r => r.AddAsync(It.Is<Book>(b =>
            b.Title == command.Title &&
            b.Description == command.Description &&
            b.Isbn == command.Isbn &&
            b.PageNumber == command.PageNumber &&
            b.Image == command.FileName &&
            b.UserId == userId &&
            b.Genre.SequenceEqual(command.Genre) &&
            b.Series == command.Series &&
            b.ReleaseDate == command.ReleaseDate &&
            b.Authors.Any(a => a.Name == authorName) &&
            b.PublishingHouse.Name == publishingHouseName
        )), Times.Once);

        bookRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.NotEqual(Guid.Empty, result);
    }
}
