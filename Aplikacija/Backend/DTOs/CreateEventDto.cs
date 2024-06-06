namespace Backend.DTOs
{
    public class CreateEventDto
    {
        [Required]
        [JsonPropertyName("title")]
        public required string Naziv { get; set; }

        [Required]
        [JsonPropertyName("description")]
        public required string Opis { get; set; }

        [Required]
        [JsonPropertyName("date")]
        public required DateTime Datum { get; set; }

        [Required]
        [JsonPropertyName("time")]
        public required string Vreme { get; set; }

        [Required]
        [JsonPropertyName("location")]
        public required string Location { get; set; }

        [JsonPropertyName("tags")]
        public List<string>? Tags { get; set; }

        [Required]
        [JsonPropertyName("img")]
        public required string Slika { get; set; }

        [JsonPropertyName("video")]
        public string? Video { get; set; }

        // mora da se vidi za prostor kako se prenosi
    }
}