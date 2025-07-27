namespace TimescaleApi.Domain.Entities;

public class Value
{
    public long Id { get; set; }
    public required string FileName { get; set; }
    public DateTime Date { get; set; }
    public double ExecutionTime { get; set; }
    public double MeasuredValue { get; set; }
}