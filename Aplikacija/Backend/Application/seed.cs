namespace Backend;

public static class Seed
{
    public static async Task SeedData(UserManager<Korisnik> userManager, RoleManager<AppRole> roleManager, Context context)
    {
        if (!userManager.Users.Any())
        {
            var visitorRole = new AppRole { Name = "Visitor" };
            var spaceOwnerRole = new AppRole { Name = "Space owner" };
            var adminRole = new AppRole { Name = "Admin" };
            var hostRole = new AppRole { Name = "Host" };
            var roles = new List<AppRole> {
                visitorRole,
                spaceOwnerRole,
                adminRole,
                hostRole
            };

            var korisnici = new List<Korisnik> {
                new Korisnik{Ime = "Luka",
                             Prezime = "Velickovic",
                             Email = "lulee@elfak.rs",
                             UserName = "lulee@elfak.rs",
                             SlikaProfila = "https://raw.githubusercontent.com/mantinedev/mantine/master/.demo/avatars/avatar-1.png",
                             Telefon = "065/123-456",
                             DatumRodjenja = new DateTime(1999, 12, 12),
                            }
            };

            var korisniciRoles = new List<string> {
                "Admin",
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            for (int i = 0; i < korisnici.Count; ++i)
            {
                var result = await userManager.CreateAsync(korisnici[i], "Sifra123!");
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join("\n", result.Errors.Select(x => x.Description)));
                }
                await userManager.AddToRoleAsync(korisnici[i], korisniciRoles[i]);
            }
        }

        int rankCount = context.VisitorRanks.Select(x => x.RankName).Count();
        if (rankCount != 5)
        {
            var visitorRanks = new List<VisitorRank> {
                new VisitorRank{RankName = "Newcomer", Points = 0},
                new VisitorRank{RankName = "Regular", Points = 4},
                new VisitorRank{RankName = "Enthusiast", Points = 11},
                new VisitorRank{RankName = "Master", Points = 21},
                new VisitorRank{RankName = "Legend", Points = 36}
            };

            foreach (var rank in visitorRanks)
            {
                await context.VisitorRanks.AddAsync(rank);
            }

            await context.SaveChangesAsync();
        }
    }
}