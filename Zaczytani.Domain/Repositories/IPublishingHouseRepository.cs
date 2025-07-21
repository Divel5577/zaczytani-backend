using Zaczytani.Domain.Entities;

namespace Zaczytani.Domain.Repositories;

public interface IPublishingHouseRepository
{
    IQueryable<PublishingHouse> GetAll();
    Task AddAsync(PublishingHouse entity);
    Task<PublishingHouse?> GetByIdAsync(Guid id);
    Task<PublishingHouse?> GetByNameAsync(string name);
    Task SaveChangesAsync();
}
