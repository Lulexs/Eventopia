namespace Backend.DTOs;

public class SpaceDto
{

    [JsonPropertyName("id")]
    public int ID { get; set; }

    [JsonPropertyName("items")]
    public List<DraggableItemDto>? DraggableItems { get; set; }

    [JsonPropertyName("lines")]
    public List<LineDto>? Lines { get; set; }

    [JsonPropertyName("surfaceDimension")]
    public SurfaceDimensionDto? SurfaceDimension { get; set; }

    [Required]
    [JsonPropertyName("description")]
    public required string Opis { get; set; }

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
    [Range(-90, 90)]
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [Required]
    [Range(-180, 180)]
    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

}