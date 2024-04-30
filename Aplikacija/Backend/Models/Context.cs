namespace Backend.Models;

public class Context : DbContext
{

    public Context(DbContextOptions options) : base(options)
    {

    }
}