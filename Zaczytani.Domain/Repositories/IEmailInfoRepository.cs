using Zaczytani.Domain.Entities;

namespace Zaczytani.Domain.Repositories;

public interface IEmailInfoRepository
{
    Task AddAsync(EmailInfo entity, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}