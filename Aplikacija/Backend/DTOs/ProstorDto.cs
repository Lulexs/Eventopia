namespace Backend.DTOs;

public class ProstorDto
{
    [Required]
    [MaxLength(50)]
    [JsonPropertyName("city")]
    public required string Grad { get; set; }

    [Required]
    [MaxLength(50)]
    [JsonPropertyName("country")]
    public required string Drzava { get; set; }

    [Required]
    [MaxLength(200)]
    [JsonPropertyName("address")]
    public required string Adresa { get; set; }

    [Required]
    [Range(-90, 90)]
    [JsonPropertyName("latitude")]
    public required double Latitude { get; set; }

    [Required]
    [Range(-180, 180)]
    [JsonPropertyName("longitude")]
    public required double Longitude { get; set; }

    [JsonPropertyName("items")]
    public List<DraggableItemDto>? DraggableItems { get; set; }

    [JsonPropertyName("lines")]
    public List<LineDto>? Lines { get; set; }

    [JsonPropertyName("surfaceDimension")]
    public SurfaceDimensionDto? SurfaceDimension { get; set; }
}