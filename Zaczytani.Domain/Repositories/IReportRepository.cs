using Zaczytani.Domain.Entities;

namespace Zaczytani.Domain.Repositories;

public interface IReportRepository
{
    IQueryable<Report> GetPendingReports();
    IQueryable<Report> GetReportById(Guid id);
    Task AddAsync(Report report, CancellationToken cancellationToken);
    Task SaveChangesAsync();
}
