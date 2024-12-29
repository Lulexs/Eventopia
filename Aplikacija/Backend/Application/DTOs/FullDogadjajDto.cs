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

    public override string ToString()
    {
        var tags = Tags != null ? string.Join(", ", Tags) : "None";
        return $"EventName: {EventName}\n" +
                $"Description: {Description}\n" +
                $"Tags: {tags}\n" +
                $"Video: {Video}\n" +
                $"Capacity: {Capacity}\n" +
                $"Location: {Location}\n" +
                $"Address: {Address}\n" +
                $"PhoneNumber: {PhoneNumber}\n" +
                $"ReservedTables: {ReservedTables}\n" +
                $"MaxTables: {MaxTables}\n" +
                $"TotalEarnings: {TotalEarnings}";
    }
}

public class FullDogadjajDtoComparer : IEqualityComparer<FullDogadjajDto>
{
    public bool Equals(FullDogadjajDto? x, FullDogadjajDto? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;

        return x.EventName == y.EventName &&
               x.Tags == null && y.Tags == null || x.Tags!.Count == y.Tags!.Count && x.Tags.All(y.Tags.Contains) &&
               x.Capacity == y.Capacity &&
               x.Location == y.Location &&
               x.Address == y.Address &&
               x.PhoneNumber == y.PhoneNumber &&
               x.ReservedTables == y.ReservedTables &&
               x.MaxTables == y.MaxTables &&
               x.TotalEarnings == y.TotalEarnings;
    }

    public int GetHashCode(FullDogadjajDto obj)
    {
        return base.GetHashCode();
    }
}