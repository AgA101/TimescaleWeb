using TimescaleApi.Domain.Entities;
using TimescaleApi.Domain.Interfaces;
using TimescaleApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace TimescaleApi.Infrastructure.Repositories
{
    public class ResultRepository : IResultRepository
    {
        private readonly AppDbContext _context;

        public ResultRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Result result)
        {
            await _context.Results.AddAsync(result);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByFileNameAsync(string fileName)
        {
            var item = await _context.Results.FirstOrDefaultAsync(r => r.FileName == fileName);
            if (item != null)
            {
                _context.Results.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Result> GetByFileNameAsync(string fileName)
        {
            return await _context.Results.FirstOrDefaultAsync(r => r.FileName == fileName);
        }

        public async Task<List<Result>> GetFilteredResultsAsync(string fileName = null, DateTime? minDate = null, DateTime? maxDate = null, double? minAvgValue = null, double? maxAvgValue = null, double? minAvgExecutionTime = null, double? maxAvgExecutionTime = null)
        {
            var query = _context.Results.AsQueryable();

            if (!string.IsNullOrEmpty(fileName))
            {
                query = query.Where(r => r.FileName == fileName);
            }
            if (minDate.HasValue)
            {
                query = query.Where(r => r.MinDate >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                query = query.Where(r => r.MinDate <= maxDate.Value);
            }
            if (minAvgValue.HasValue)
            {
                query = query.Where(r => r.AvgValue >= minAvgValue.Value);
            }
            if (maxAvgValue.HasValue)
            {
                query = query.Where(r => r.AvgValue <= maxAvgValue.Value);
            }
            if (minAvgExecutionTime.HasValue)
            {
                query = query.Where(r => r.AvgExecutionTime >= minAvgExecutionTime.Value);
            }
            if (maxAvgExecutionTime.HasValue)
            {
                query = query.Where(r => r.AvgExecutionTime <= maxAvgExecutionTime.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}