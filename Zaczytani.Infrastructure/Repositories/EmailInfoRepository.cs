using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Repositories;
using Zaczytani.Infrastructure.Persistance;

namespace Zaczytani.Infrastructure.Repositories;

internal class EmailInfoRepository(BookDbContext dbContext) : IEmailInfoRepository
{
    private readonly BookDbContext _dbContext = dbContext;

    public async Task AddAsync(EmailInfo entity, CancellationToken cancellationToken) => await _dbContext.AddAsync(entity, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
