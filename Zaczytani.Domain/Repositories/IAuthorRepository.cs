using Zaczytani.Domain.Entities;

namespace Zaczytani.Domain.Repositories;

public interface IAuthorRepository
{
    IQueryable<Author> GetAll();
    IQueryable<Author> GetBySearchPhrase(string searchPhrase);
    Task<Author?> GetByIdAsync(Guid authorId);
    Task<Author?> GetByNameAsync(string name);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}