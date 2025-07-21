using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net.NetworkInformation;
using Zaczytani.API.Controllers;
using Zaczytani.Application.Admin.Commands;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Shared.Queries;
using Zaczytani.Domain.Enums;

namespace Zaczytani.UnitTests.Controllers;

public class BookControllersTests
{
    private readonly Mock<IMediator> mediatorMock;
    private readonly BookController controller;

    public BookControllersTests()
    {
        mediatorMock = new Mock<IMediator>();
        controller = new BookController(mediatorMock.Object);
    }
    [Fact]
    public async Task GetBookDetails_ReturnsOkResult_WithBook()
    {
        var bookId = Guid.NewGuid();
        var expectedBook = new BookDto { Id = bookId, Title = "Test Book" };

        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetBookDetailsQuery>(), default))
            .ReturnsAsync(expectedBook);

        var result = await controller.GetBookDetails(bookId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBook = Assert.IsType<BookDto>(okResult.Value);
        Assert.Equal(expectedBook.Id, returnedBook.Id);
        Assert.Equal(expectedBook.Title, returnedBook.Title);
    }

    [Fact]
    public async Task CreateBook_ReturnsCreatedAtActionResult()
    {
        var bookId = Guid.NewGuid();
        var command = new CreateBookCommand
        {
            Title = "Test Title",
            Isbn = "1234567890123",
            Description = "This is a test description.",
            PageNumber = 300,
            ReleaseDate = DateOnly.Parse("2025-01-01"),
            Authors = new List<string> { "Author 1", "Author 3" },
            PublishingHouse = "Test Publishing House",
            Genre = new List<BookGenre> { BookGenre.Fiction },
            Series = "Test Series"
        };
        mediatorMock
        .Setup(m => m.Send(command, default))
            .ReturnsAsync(bookId);

        var result = await controller.CreateBook(command);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(BookController.GetBookDetails), createdResult.ActionName);
        var routeValues = (IDictionary<string, object>)createdResult.RouteValues;
        Assert.Equal(bookId, routeValues["id"]);
    }

    [Fact]
    public async Task DeleteBook_ReturnsNoContent_WhenBookIsDeleted()
    {
        var bookId = Guid.NewGuid();

        mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteBookCommand>(), default))
            .Returns(Task.CompletedTask);

        var result = await controller.DeleteBook(bookId);

        var noContentResult = Assert.IsType<NoContentResult>(result);
        mediatorMock.Verify(m => m.Send(It.Is<DeleteBookCommand>(cmd => cmd.Id == bookId), default), Times.Once);
    }
}


