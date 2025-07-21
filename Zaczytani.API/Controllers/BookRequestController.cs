using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zaczytani.Application.Admin.Commands;
using Zaczytani.Application.Admin.Queries;
using Zaczytani.Application.Client.Commands;
using Zaczytani.Application.Client.Queries;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Filters;
using Zaczytani.Domain.Constants;

namespace Zaczytani.API.Controllers;

[ApiController]
[Authorize]
[SetUserId]
[Route("api/[controller]")]
public class BookRequestController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [Authorize(Roles =UserRoles.User)]
    [HttpPost]
    public async Task<IActionResult> CreateBookRequest([FromBody] CreateBookRequestCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id });
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost("Accept/{id}")]
    public async Task<IActionResult> AcceptBookRequest([FromRoute] Guid id, [FromBody] AcceptBookRequestCommand command)
    {
        command.SetId(id);
        await _mediator.Send(command);
        return Ok();
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPatch("Reject/{id}")]
    public async Task<IActionResult> RejectBookRequest([FromRoute] Guid id)
    {
        var command = new RejectBookRequestCommand();
        command.SetId(id);
        await _mediator.Send(command);
        return Ok();
    }

    [Authorize(Roles = UserRoles.User)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserBookRequestDto>>> GetUsersBookRequests()
    {
        var bookRequests = await _mediator.Send(new GetUsersBookRequestsQuery());
        return Ok(bookRequests);
    }
    [Authorize(Roles =UserRoles.Admin)]
    [HttpGet("Pending")]
    public async Task<ActionResult<IEnumerable<BookRequestDto>>> GetPendingBookRequests()
    {
        var bookRequests = await _mediator.Send(new GetBookRequestsQuery());
        return Ok(bookRequests);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet("GeneratedBookDetails")]
    public async Task<ActionResult<IEnumerable<GeneratedBookDto>>> GetGeneratedBookDetails([FromQuery] GetGeneratedBookDetailsQuery query)
    {
        var bookDetails = await _mediator.Send(query);
        return Ok(bookDetails);
    }
}
