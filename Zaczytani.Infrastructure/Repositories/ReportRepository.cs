using Microsoft.EntityFrameworkCore;
using Zaczytani.Domain.Entities;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Repositories;
using Zaczytani.Infrastructure.Persistance;

namespace Zaczytani.Infrastructure.Repositories;

internal class ReportRepository(BookDbContext dbContext) : IReportRepository
{
    private readonly BookDbContext _dbContext = dbContext;

    public IQueryable<Report> GetPendingReports() => _dbContext.Reports.Where(r => r.Status == ReportStatus.Pending);

    public IQueryable<Report> GetReportById(Guid id) => _dbContext.Reports.Where(x => x.Id == id);

    public async Task AddAsync(Report report, CancellationToken cancellationToken)
    {
        await _dbContext.Reports.AddAsync(report, cancellationToken);
    }

    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}
