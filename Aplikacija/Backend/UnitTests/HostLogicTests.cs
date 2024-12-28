using System.Globalization;

namespace UnitTests;

public class HostLogicTests
{

    [Test]
    // [Ignore("Temp")]
    public async Task CreateEvent_CreatesEvent()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);

        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.UserRole!.Role!.Name == "Host").FirstAsync();
        DateTime now = DateTime.Now;
        CreateEventDto dto = new()
        {
            Naziv = "Test dogadjaj",
            Opis = "Test opis",
            Datum = "2030-02-05",
            Vreme = "20:00",
            Tags = ["tag1", "tag2"],
            ProstorId = 1,
            Items = await _context.DraggableItems.Include(x => x.PlanProstora).Where(x => x.PlanProstora!.ID == 1).Select(x => (DraggableItemDto)x).ToListAsync(),
            Lines = await _context.Lines.Where(x => x.PlanProstora!.ID == 1).Select(x => new LineDto { X1 = x.X1, X2 = x.X2, Y1 = x.Y1, Y2 = x.Y2 }).ToListAsync(),
            SurfaceDimension = new SurfaceDimensionDto() { Width = 1280, Height = 720 }
        };
        int id = await _hostLogic.CreateEvent(dto, korisnik);

        var rezervacija = await _context.RezervacijeProstora.Include(x => x.Dogadjaj)
                                                            .ThenInclude(y => y!.Organizator)
                                                            .ThenInclude(y => y!.Tagovi)
                                                            .Include(x => x.Prostor)
                                                            .ThenInclude(y => y!.PlanoviProstora)
                                                            .Where(x => x.Dogadjaj!.ID == id)
                                                            .FirstOrDefaultAsync();

        Assert.Multiple(() =>
        {
            Assert.That(rezervacija, Is.Not.Null);
            Assert.That(rezervacija!.Dogadjaj!.Naziv, Is.EqualTo(dto.Naziv));
            Assert.That(rezervacija!.Dogadjaj!.Opis, Is.EqualTo(dto.Opis));
            Assert.That(rezervacija.Dogadjaj.Vreme, Is.EqualTo(DateTime.ParseExact($"{dto.Datum} {dto.Vreme}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)));
            Assert.That(rezervacija.Dogadjaj!.Tagovi!.Select(x => x.TagName), Is.EquivalentTo(dto.Tags));
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task CreateEventInPast_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);

        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.UserRole!.Role!.Name == "Host").FirstAsync();
        DateTime now = DateTime.Now;
        CreateEventDto dto = new()
        {
            Naziv = "Test dogadjaj",
            Opis = "Test opis",
            Datum = "2024-02-05",
            Vreme = "20:00",
            Tags = ["tag1", "tag2"],
            ProstorId = 1,
            Items = await _context.DraggableItems.Include(x => x.PlanProstora).Where(x => x.PlanProstora!.ID == 1).Select(x => (DraggableItemDto)x).ToListAsync(),
            Lines = await _context.Lines.Where(x => x.PlanProstora!.ID == 1).Select(x => new LineDto { X1 = x.X1, X2 = x.X2, Y1 = x.Y1, Y2 = x.Y2 }).ToListAsync(),
            SurfaceDimension = new SurfaceDimensionDto() { Width = 1280, Height = 720 }
        };

        var exception = Assert.ThrowsAsync<EventInPastException>(async () =>
        {
            int id = await _hostLogic.CreateEvent(dto, korisnik);
        });
        Assert.That(exception!.Message, Is.EqualTo("Event date and time must be in the future."));

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task CreateEvenWithNonExistingSpace_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);

        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.UserRole!.Role!.Name == "Host").FirstAsync();
        DateTime now = DateTime.Now;
        CreateEventDto dto = new()
        {
            Naziv = "Test dogadjaj",
            Opis = "Test opis",
            Datum = "2030-02-05",
            Vreme = "20:00",
            Tags = ["tag1", "tag2"],
            ProstorId = 12456,
            Items = await _context.DraggableItems.Include(x => x.PlanProstora).Where(x => x.PlanProstora!.ID == 1).Select(x => (DraggableItemDto)x).ToListAsync(),
            Lines = await _context.Lines.Where(x => x.PlanProstora!.ID == 1).Select(x => new LineDto { X1 = x.X1, X2 = x.X2, Y1 = x.Y1, Y2 = x.Y2 }).ToListAsync(),
            SurfaceDimension = new SurfaceDimensionDto() { Width = 1280, Height = 720 }
        };

        var exception = Assert.ThrowsAsync<SpaceNotFoundException>(async () =>
        {
            int id = await _hostLogic.CreateEvent(dto, korisnik);
        });
        Assert.That(exception!.Message, Is.EqualTo("Space not found."));

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }


}