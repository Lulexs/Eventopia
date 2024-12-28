namespace Backend.DTOs;

public class SpaceBasicDto
{
    [Required]
    [JsonPropertyName("id")]
    public int ID { get; set; }

    [Required]
    [JsonPropertyName("city")]
    public required string Grad { get; set; }

    [Required]
    [JsonPropertyName("country")]
    public required string Drzava { get; set; }

    [Required]
    [JsonPropertyName("address")]
    public required string Adresa { get; set; }

    [Required]
    [JsonPropertyName("capacity")]
    public required int Kapacitet { get; set; }
}

public class SpaceBasicDtoComparer : IEqualityComparer<SpaceBasicDto>
{
    public bool Equals(SpaceBasicDto? x, SpaceBasicDto? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;

        return x.ID == y.ID &&
               x.Grad == y.Grad &&
               x.Drzava == y.Drzava &&
               x.Adresa == y.Adresa &&
               x.Kapacitet == y.Kapacitet;
    }

    public int GetHashCode(SpaceBasicDto obj)
    {
        return base.GetHashCode();
    }
}