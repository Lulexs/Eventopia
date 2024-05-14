using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Backend.Models;

public class Context : IdentityDbContext<Korisnik>
{

    public Context(DbContextOptions<Context> options) : base(options)
    {

    }
}

