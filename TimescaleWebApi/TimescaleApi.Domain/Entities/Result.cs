namespace TimescaleApi.Domain.Entities;

public class Result
{
    public long Id { get; set; }
    public required string FileName { get; set; }
    public double TimeDeltaSeconds { get; set; }
    public DateTime MinDate { get; set; }
    public double AvgExecutionTime { get; set; }
    public double AvgValue { get; set; }
    public double MedianValue { get; set; }
    public double MaxValue { get; set; }
    public double MinValue { get; set; }
}