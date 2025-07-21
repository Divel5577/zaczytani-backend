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
public class ChallengeController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult> CreateChallenge([FromBody] CreateChallengeCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("{challengeId}/Join")]
    public async Task<ActionResult> JoinChallenge([FromRoute] Guid challengeId, CancellationToken cancellationToken)
    {
        var command = new JoinChallengeCommand(challengeId);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChallengeDto>>> GetAllChallenges(CancellationToken cancellationToken)
    {
        var query = new GetOtherUsersChallengesQuery();
        var challenges = await _mediator.Send(query, cancellationToken);

        return Ok(challenges);
    }

    [HttpGet("Progress")]
    public async Task<ActionResult<IEnumerable<ChallengeProgressDto>>> GetChallengeProgresses(CancellationToken cancellationToken)
    {
        var query = new GetChallengeProgressesQuery();
        var challenges = await _mediator.Send(query, cancellationToken);

        return Ok(challenges);
    }

    [HttpGet("MyChallenges")]
    public async Task<ActionResult<IEnumerable<ChallengeDto>>> GetMyChallenges(CancellationToken cancellationToken)
    {
        var query = new GetMyChallengesQuery();
        var challenges = await _mediator.Send(query, cancellationToken);
        return Ok(challenges);
    }

    [HttpDelete("{challengeId}")]
    public async Task<IActionResult> DeleteChallenge([FromRoute] Guid challengeId)
    {
        var command = new DeleteChallengeCommand(challengeId);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{challengeId}/Detach")]
    public async Task<IActionResult> DetachFromChallenge([FromRoute] Guid challengeId)
    {
        var command = new DetachFromChallengeCommand(challengeId);
        await _mediator.Send(command);
        return NoContent();
    }
}
