namespace Backend.Dtos;

public class FullDogadjajDto
{
    public string? EventName { get; set; }
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    public string? Video { get; set; }
    public int Capacity { get; set; }
    public string? Location { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public int ReservedTables { get; set; }
    public int MaxTables { get; set; }
    public int? TotalEarnings { get; set; }
}
