namespace Backend.DTOs;

public class KorisnikSaZabranamaDto
{
    [JsonPropertyName("userId")]
    public string? KorisnikId { get; set; }
    [JsonPropertyName("banId")]
    public int ZabranaId { get; set; }
    [JsonPropertyName("firstName")]
    public string? Ime { get; set; }
    [JsonPropertyName("lastName")]
    public string? Prezime { get; set; }
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }
    [JsonPropertyName("role")]
    public string? Role { get; set; }
    [JsonPropertyName("timeFrom")]
    public DateTime DatumOd { get; set; }
    [JsonPropertyName("timeTo")]
    public DateTime DatumDo { get; set; }
    [JsonPropertyName("reason")]
    public string? Razlog { get; set; }

    public override string ToString()
    {
        return $"KorisnikId: {KorisnikId}, ZabranaId: {ZabranaId}, Ime: {Ime}, Prezime: {Prezime}, Avatar: {Avatar}, Role: {Role}, DatumOd: {DatumOd}, DatumDo: {DatumDo}, Razlog: {Razlog}\n";
    }
}

public class KorisnikBezZabranaDto
{
    public Guid Id { get; set; }
    public string? Ime { get; set; }
    public string? Prezime { get; set; }
    public string? Email { get; set; }
    public AppUserRole? Role { get; set; }
}

public class KorisnikSaZabranamaDtoComparer : IEqualityComparer<KorisnikSaZabranamaDto>
{
    public bool Equals(KorisnikSaZabranamaDto? x, KorisnikSaZabranamaDto? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;

        return x.KorisnikId == y.KorisnikId &&
               x.ZabranaId == y.ZabranaId &&
               x.Ime == y.Ime &&
               x.Prezime == y.Prezime &&
               x.Avatar == y.Avatar &&
               x.Role == y.Role &&
               x.DatumOd == y.DatumOd &&
               x.DatumDo == y.DatumDo &&
               x.Razlog == y.Razlog;
    }

    public int GetHashCode(KorisnikSaZabranamaDto obj)
    {
        return base.GetHashCode();
    }
}
