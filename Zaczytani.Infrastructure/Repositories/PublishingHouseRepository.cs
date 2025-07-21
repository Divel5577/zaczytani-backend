using Microsoft.EntityFrameworkCore;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Repositories;
using Zaczytani.Infrastructure.Persistance;

namespace Zaczytani.Infrastructure.Repositories;

internal class PublishingHouseRepository(BookDbContext dbContext) : IPublishingHouseRepository
{
    private readonly BookDbContext _dbContext = dbContext;

    public IQueryable<PublishingHouse> GetAll() => _dbContext.PublishingHouses;

    public async Task AddAsync(PublishingHouse entity) => await _dbContext.PublishingHouses.AddAsync(entity);

    public async Task<PublishingHouse?> GetByIdAsync(Guid id) => await _dbContext.PublishingHouses.FirstOrDefaultAsync(a => a.Id == id);

    public async Task<PublishingHouse?> GetByNameAsync(string name) => await _dbContext.PublishingHouses.Where(a => a.Name == name).FirstOrDefaultAsync();

    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}
