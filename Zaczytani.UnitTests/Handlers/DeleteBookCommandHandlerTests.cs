using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaczytani.Application.Admin.Commands;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.UnitTests.Handlers;

public class DeleteBookCommandHandlerTests
{
    private readonly Mock<IBookRepository> bookRepositoryMock;
    private readonly IRequestHandler<DeleteBookCommand> handler;

    public DeleteBookCommandHandlerTests()
    {
        bookRepositoryMock = new Mock<IBookRepository>();
        handler = new DeleteBookCommand.DeleteBookCommandHandler(bookRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteBook_WhenBookExists()
    {
        var bookId = Guid.NewGuid();
        var book = new Book { Id = bookId, Title = "Ksiązka testowa" };

        bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>())).ReturnsAsync(book);
        bookRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var command = new DeleteBookCommand(bookId);

        await handler.Handle(command, CancellationToken.None);

        bookRepositoryMock.Verify(r => r.Delete(book), Times.Once);
        bookRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBookDoesNotExist()
    {
        var bookId = Guid.NewGuid();
        bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>())).ReturnsAsync((Book?)null);

        var command = new DeleteBookCommand(bookId);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        bookRepositoryMock.Verify(r => r.Delete(It.IsAny<Book>()), Times.Never);
        bookRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
