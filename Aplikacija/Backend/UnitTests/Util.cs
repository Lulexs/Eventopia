using System.Text.RegularExpressions;

namespace UnitTests;


public static class Utils
{
    public static bool IsBase64String(string input)
    {
        if (input.Length % 4 != 0)
            return false;

        string pattern = @"^[a-zA-Z0-9+/]*={0,2}$";
        return Regex.IsMatch(input, pattern);
    }

    public readonly static List<SpaceBasicDto> InitialSpaces = [
            new SpaceBasicDto() {
                ID = 1,
                Grad = "Beograd",
                Drzava = "Serbia",
                Adresa = "Zorana Zunkovica",
                Kapacitet = 88
            },
            new SpaceBasicDto() {
                ID = 2,
                Grad = "Beograd",
                Drzava = "Serbia",
                Adresa = "Uciteljska 14",
                Kapacitet = 100
            },
            new SpaceBasicDto() {
                ID = 3,
                Grad = "Sarajevo",
                Drzava = "Bosnia",
                Adresa = "Karadjordjeva",
                Kapacitet = 148
            },
        ];

    public readonly static List<KorisnikSaZabranamaDto> InitialUsers =
        [
            new KorisnikSaZabranamaDto
            {
                KorisnikId = "cb2f456a-0e50-43c9-e98a-08dd25cb4a83",
                ZabranaId = 0,
                Ime = "Event",
                Prezime = "Organizer1",
                Avatar = "https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-1.png",
                Role = "Host",
                DatumOd = DateTime.Parse("0001-01-01T00:00:00"),
                DatumDo = DateTime.Parse("0001-01-01T00:00:00"),
                Razlog = null
            },
            new KorisnikSaZabranamaDto
            {
                KorisnikId = "634a7ec8-1a5f-43a5-e98b-08dd25cb4a83",
                ZabranaId = 0,
                Ime = "Event",
                Prezime = "Organizer2",
                Avatar = "https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-1.png",
                Role = "Host",
                DatumOd = DateTime.Parse("0001-01-01T00:00:00"),
                DatumDo = DateTime.Parse("0001-01-01T00:00:00"),
                Razlog = null
            },
            new KorisnikSaZabranamaDto
            {
                KorisnikId = "b8ced5ee-f7c8-47b1-e98c-08dd25cb4a83",
                ZabranaId = 0,
                Ime = "Event ",
                Prezime = "Visitor1",
                Avatar = "https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-2.png",
                Role = "Visitor",
                DatumOd = DateTime.Parse("0001-01-01T00:00:00"),
                DatumDo = DateTime.Parse("0001-01-01T00:00:00"),
                Razlog = null
            },
            new KorisnikSaZabranamaDto
            {
                KorisnikId = "04840f1e-2fb6-4657-e98d-08dd25cb4a83",
                ZabranaId = 0,
                Ime = "Event",
                Prezime = "Visitor2",
                Avatar = "https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-9.png",
                Role = "Visitor",
                DatumOd = DateTime.Parse("0001-01-01T00:00:00"),
                DatumDo = DateTime.Parse("0001-01-01T00:00:00"),
                Razlog = null
            },
            new KorisnikSaZabranamaDto
            {
                KorisnikId = "f1542b08-a1b1-4630-e988-08dd25cb4a83",
                ZabranaId = 0,
                Ime = "Space",
                Prezime = "Owner1",
                Avatar = "https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-1.png",
                Role = "Space owner",
                DatumOd = DateTime.Parse("0001-01-01T00:00:00"),
                DatumDo = DateTime.Parse("0001-01-01T00:00:00"),
                Razlog = null
            },
            new KorisnikSaZabranamaDto
            {
                KorisnikId = "8cfbb93e-8a7b-44ad-e989-08dd25cb4a83",
                ZabranaId = 0,
                Ime = "Space",
                Prezime = "Owner2",
                Avatar = "https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-1.png",
                Role = "Space owner",
                DatumOd = DateTime.Parse("0001-01-01T00:00:00"),
                DatumDo = DateTime.Parse("0001-01-01T00:00:00"),
                Razlog = null
            }
        ];
}