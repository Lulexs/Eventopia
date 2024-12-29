namespace Backend.DTOs;

public class OcenaZaHostaDto
{
    public string? Avatar { get; set; }
    public string? Name { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string? Time { get; set; }

    public override string ToString()
    {
        return $"{{ Avatar: {Avatar ?? "null"}, Name: {Name ?? "null"}, Rating: {Rating}, Comment: {Comment ?? "null"} }}\n";
    }
}

public class OcenaZaHostaDtoComparer : IEqualityComparer<OcenaZaHostaDto>
{
    public bool Equals(OcenaZaHostaDto? x, OcenaZaHostaDto? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;

        return string.Equals(x.Avatar, y.Avatar) &&
                string.Equals(x.Name, y.Name) &&
                x.Rating == y.Rating &&
                string.Equals(x.Comment, y.Comment);
    }

    public int GetHashCode(OcenaZaHostaDto obj)
    {
        return base.GetHashCode();
    }
}