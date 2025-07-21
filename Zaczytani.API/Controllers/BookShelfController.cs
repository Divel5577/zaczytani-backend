using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zaczytani.Application.Client.Commands;
using Zaczytani.Application.Client.Queries;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Constants;

namespace Zaczytani.API.Controllers;

[ApiController]
[Authorize(Roles = UserRoles.User)]
[SetUserId]
[Route("api/[controller]")]
public class BookShelfController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("{shelfId}/{bookId}/Attach")]
    public async Task<ActionResult> AttachBook([FromRoute] Guid shelfId, Guid bookId)
    {
        var command = new AttachBookCommand(shelfId, bookId);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{shelfId}/{bookId}/Detach")]
    public async Task<ActionResult> DetachBook([FromRoute] Guid shelfId, Guid bookId)
    {
        var command = new DetachBookCommand(shelfId, bookId);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("Create")]
    public async Task<ActionResult> CreateShelf([FromBody] CreateBookShelfCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetShelf), new { id }, new { Id = id });
    }

    [HttpPut("Update")]
    public async Task<ActionResult> UpdateShelf([FromBody] UpdateBookShelfCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("Delete")]
    public async Task<ActionResult> DeleteShelf([FromBody] DeleteBookShelfCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet("GetBookshelf/{id:guid}")]
    public async Task<ActionResult<BookShelfDto>> GetShelf(Guid id)
    {
        var query = new GetBookShelfQuery(id);
        var shelf = await _mediator.Send(query);
        return shelf == null ? NotFound() : Ok(shelf);
    }

    [HttpGet("GetAllBookShelves")]
    public async Task<ActionResult<IEnumerable<BookShelfDto>>> GetAllShelves()
    {
        var query = new GetAllBookshelvesQuery();
        var shelves = await _mediator.Send(query);
        return Ok(shelves);
    }

    [HttpGet("{id}/Books")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksOnShelf(Guid id)
    {
        var query = new GetBooksOnShelfQuery(id);
        var books = await _mediator.Send(query);
        return Ok(books);
    }

    [HttpGet("CurrentlyReadingShelfId")]
    public async Task<ActionResult<ReadingBookShelfIdDto>> GetCurrentlyReadingShelfId()
    {
        var query = new GetCurrentlyReadingShelfIdQuery();
        var shelfId = await _mediator.Send(query);
        return Ok(shelfId);
    }

}