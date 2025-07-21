using MediatR;
using Zaczytani.Domain.Exceptions;
using Zaczytani.Domain.Repositories;

namespace Zaczytani.Application.Admin.Commands;

public class AddBadgeImageCommand : IRequest<Guid>
{
    public Guid BadgeId { get; set; }
    public string FileName { get; set; } = string.Empty;

    private class AddBadgeImageCommandHandler(IBadgeRepository badgeRepository) : IRequestHandler<AddBadgeImageCommand, Guid>
    {
        private readonly IBadgeRepository _badgeRepository = badgeRepository;

        public async Task<Guid> Handle(AddBadgeImageCommand request, CancellationToken cancellationToken)
        {
            var badge = await _badgeRepository.GetBadgeDefinitionByIdAsync(request.BadgeId, cancellationToken);

            if (badge == null)
                throw new NotFoundException($"Badge with ID {request.BadgeId} not found.");

            badge.IconPath = request.FileName;

            await _badgeRepository.SaveChangesAsync(cancellationToken);

            return badge.Id;
        }
    }
}
