using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Shared.Queries;

namespace Zaczytani.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SearchController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SearchDto>>> Search([FromQuery] SearchQuery command, CancellationToken cancellationToken)
    {
        var books = await _mediator.Send(command, cancellationToken);

        return Ok(books);
    }
}
