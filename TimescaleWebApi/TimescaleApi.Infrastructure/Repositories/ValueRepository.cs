using TimescaleApi.Domain.Entities;
using TimescaleApi.Domain.Interfaces;
using TimescaleApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace TimescaleApi.Infrastructure.Repositories
{
    public class ValueRepository : IValueRepository
    {
        private readonly AppDbContext _context;

        public ValueRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<Value> values)
        {
            await _context.Values.AddRangeAsync(values);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByFileNameAsync(string fileName)
        {
            var items = await _context.Values
                .Where(v => v.FileName == fileName)
                .ToListAsync();
            _context.Values.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Value>> GetAllAsync(string fileName)
        {
            return await _context.Values
                .Where(v => v.FileName == fileName)
                .ToListAsync();
        }

        public async Task<List<Value>> GetLast10ByFileNameAsync(string fileName)
        {
            return await _context.Values
                .Where(v => v.FileName == fileName)
                .OrderBy(v => v.Date)
                .Take(10)
                .ToListAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}