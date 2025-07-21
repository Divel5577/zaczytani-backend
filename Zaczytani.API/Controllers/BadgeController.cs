using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zaczytani.Application.Admin.Commands;
using Zaczytani.Application.Admin.Queries;
using Zaczytani.Application.Client.Queries;
using Zaczytani.Application.Dtos;
using Zaczytani.Domain.Constants;
using Zaczytani.Domain.Enums;

namespace Zaczytani.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BadgeController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [Authorize(Roles = UserRoles.User)]
    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<BadgeDto>>> GetUserBadges([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetUserBadgesQuery(userId), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = UserRoles.User)]
    [HttpGet("Definitions")]
    public async Task<ActionResult<IEnumerable<BadgeDto>>> GetAllBadgeDefinitions([FromQuery] BadgeType? type, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBadgeDefinitionsQuery(type), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost("{badgeId}/AddImage")]
    public async Task<ActionResult<Guid>> AddBadgeImage([FromRoute] Guid badgeId, [FromBody] string fileName)
    {
        var command = new AddBadgeImageCommand
        {
            BadgeId = badgeId,
            FileName = fileName
        };

        await _mediator.Send(command);
        return NoContent();
    }

}
