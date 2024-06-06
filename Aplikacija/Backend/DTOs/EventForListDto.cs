namespace Backend.DTOs;

public class EventForListDto
{

    [JsonPropertyName("title")]
    public required string Naziv { get; set; }

    [JsonPropertyName("img")]
    public required string Slika { get; set; }

    [JsonPropertyName("date")]
    public required string Datum { get; set; }
}