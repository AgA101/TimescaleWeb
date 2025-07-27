using TimescaleApi.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace TimescaleApi.Domain.Interfaces;

public interface IValueRepository
{
    Task AddRangeAsync(IEnumerable<Value> values);
    Task DeleteByFileNameAsync(string fileName);
    Task<List<Value>> GetLast10ByFileNameAsync(string fileName);
    Task<IDbContextTransaction> BeginTransactionAsync();
}