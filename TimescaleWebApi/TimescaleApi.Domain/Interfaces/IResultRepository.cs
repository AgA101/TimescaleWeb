using TimescaleApi.Domain.Entities;

namespace TimescaleApi.Domain.Interfaces;

public interface IResultRepository
{
    Task AddAsync(Result result);
    Task DeleteByFileNameAsync(string fileName);
    Task<List<Result>> GetFilteredResultsAsync(string fileName = null, DateTime? minDate = null, DateTime? maxDate = null, double? minAvgValue = null, double? maxAvgValue = null, double? minAvgExecutionTime = null, double? maxAvgExecutionTime = null);
    
}