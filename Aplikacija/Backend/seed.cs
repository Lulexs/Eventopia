//testiranje backenda baki
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Persistence;
public class Seed
{
    

    public static async Task SeedUsers(UserManager<Korisnik> userManager, RoleManager<AppRole> roleManager)
    {
        if(!userManager.Users.Any())
        {
            var korisnici = new List<Korisnik> {
                new Korisnik{Ime = "Luke", Prezime = "Velickovic", UserName = "luleee", Email = "lulee@elfaksss.rs", SlikaProfila = "slika1.jpg", Telefon = "065/123-456", DatumRodjenja = new DateTime(1999, 12, 12)},
                //new Korisnik{Ime = "Dimitrije", Prezime = "Najdanovic", UserName = "dika", Email = "dikadika@elfak.rs"},
               // new Korisnik{Ime = "Aleksandar", Prezime = "Djordjevic", UserName = "suki", Email = "sukisuki@elfak.rs"}
            };


            var roles = new List<AppRole> {
                new AppRole{Name = "ObicanKorisnik"},
                new AppRole{Name = "VlasnikProstora"},
                new AppRole{Name = "Administrator"},
                new AppRole{Name = "OrganizatorDogadjaja"}
            };

            foreach (var role in roles){
                await roleManager.CreateAsync(role);//da kreiramo uloge u bazi
            }

            foreach(var korisnik in korisnici){
                await userManager.CreateAsync(korisnik, "PrejaK@s1fra");//da kreiramo korisnika sa sifrom u bazi
                    if (korisnik.Ime == "Luke")
                        await userManager.AddToRoleAsync(korisnik, "Administrator");
            }
        }



    }
}