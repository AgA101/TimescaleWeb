using TimescaleApi.Domain.Entities;
using TimescaleApi.Domain.Interfaces;
using CsvHelper;
using System.Globalization;


namespace TimescaleApi.Application.Services
{
    public class ValueService
    {
        private readonly IValueRepository _valueRepository;
        private readonly IResultRepository _resultRepository;

        public ValueService(IValueRepository valueRepository, IResultRepository resultRepository)
        {
            _valueRepository = valueRepository ?? throw new ArgumentNullException(nameof(valueRepository));
            _resultRepository = resultRepository ?? throw new ArgumentNullException(nameof(resultRepository));
        }

        public async Task<Result> ProcessCsvAsync(Stream csvStream, string fileName)
        {
            var values = new List<Value>();
            using (var reader = new StreamReader(csvStream))
            using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";" 
            }))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var value = new Value
                    {
                        FileName = fileName,
                        Date = DateTime.Parse(csv.GetField<string>("Date"), null, System.Globalization.DateTimeStyles.RoundtripKind),
                        ExecutionTime = csv.GetField<double>("ExecutionTime"),
                        MeasuredValue = csv.GetField<double>("Value")
                    };
                    values.Add(value);
                }
            }
            
            if (values.Count < 1 || values.Count > 10000)
                throw new ArgumentException("Number of rows must be between 1 and 10,000.");

            foreach (var value in values)
            {
                if (value.Date < new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc) || value.Date > DateTime.UtcNow)
                    throw new ArgumentException("Date must be between 2000-01-01 and now.");
                if (value.ExecutionTime < 0)
                    throw new ArgumentException("ExecutionTime cannot be negative.");
                if (value.MeasuredValue < 0)
                    throw new ArgumentException("MeasuredValue cannot be negative.");
            }
            
            var result = new Result
            {
                FileName = fileName,
                TimeDeltaSeconds = (values.Max(v => v.Date) - values.Min(v => v.Date)).TotalSeconds,
                MinDate = values.Min(v => v.Date),
                AvgExecutionTime = values.Average(v => v.ExecutionTime),
                AvgValue = values.Average(v => v.MeasuredValue),
                MedianValue = values.Count % 2 == 0 
                    ? values.OrderBy(v => v.MeasuredValue).Skip(values.Count / 2 - 1).Take(2).Average(v => v.MeasuredValue)
                    : values.OrderBy(v => v.MeasuredValue).ElementAt(values.Count / 2).MeasuredValue,
                MaxValue = values.Max(v => v.MeasuredValue),
                MinValue = values.Min(v => v.MeasuredValue)
            };
            
            using (var transaction = await _valueRepository.BeginTransactionAsync())
            {
                try
                {
                    await _valueRepository.DeleteByFileNameAsync(fileName);
                    await _valueRepository.AddRangeAsync(values);
                    await _resultRepository.DeleteByFileNameAsync(fileName);
                    await _resultRepository.AddAsync(result);
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw; 
                }
            }

            return result;
        }

        public async Task<List<Result>> GetFilteredResultsAsync(string fileName = null, DateTime? minDate = null, DateTime? maxDate = null, double? minAvgValue = null, double? maxAvgValue = null, double? minAvgExecutionTime = null, double? maxAvgExecutionTime = null)
        {
            return await _resultRepository.GetFilteredResultsAsync(fileName, minDate, maxDate, minAvgValue, maxAvgValue, minAvgExecutionTime, maxAvgExecutionTime);
        }

        public async Task<List<Value>> GetLast10ValuesAsync(string fileName)
        {
            return await _valueRepository.GetLast10ByFileNameAsync(fileName);
        }
    }
}