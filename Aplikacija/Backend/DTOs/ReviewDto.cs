namespace Backend.DTOs;

    public class ReviewDto
    {
        [Range(1, 10)]
        public int Vrednost { get; set; }

        [MaxLength(200)]
        public string? Komentar { get; set; }

        public DateTime VremeKomentara { get; set; }
        
        public string Korisnik { get; set; }
    }
