namespace Backend;

public static class Seed
{
    public static async Task SeedUsers(UserManager<Korisnik> userManager, RoleManager<AppRole> roleManager)
    {
        if (!userManager.Users.Any())
        {
            var korisnici = new List<Korisnik> {
                new Korisnik{Ime = "Luka", Prezime = "Velickovic", Email = "lulee@elfak.rs", UserName = "lulee@elfak.rs", SlikaProfila = "https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-1.png", Telefon = "065/123-456", DatumRodjenja = new DateTime(1999, 12, 12)},
            };

            var roles = new List<AppRole> {
                new AppRole{Name = "Visitor"},
                new AppRole{Name = "Space Owner"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Host"}
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach (var korisnik in korisnici)
            {
                var result = await userManager.CreateAsync(korisnik, "PrejaK@s1fra");
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join("\n", result.Errors.Select(x => x.Description)));
                }
                if (korisnik.Ime == "Luka")
                    await userManager.AddToRoleAsync(korisnik, "Admin");
            }
        }

    }
}