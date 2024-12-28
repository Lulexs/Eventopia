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

    [Test]
    // [Ignore("Temp")]
    public async Task GetAvailableSpaces_AllAvailable_GetsSpaces()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);

        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.UserRole!.Role!.Name == "Host").FirstAsync();
        var allSpaces = await _hostLogic.GetAvailableSpaces("2025-03-01", "20:00", "null", -1, korisnik);

        Assert.Multiple(() =>
        {
            Assert.That(allSpaces, Has.Count.EqualTo(Utils.InitialSpaces.Count));
            Assert.That(allSpaces, Is.EquivalentTo(Utils.InitialSpaces).Using(new SpaceBasicDtoComparer()));
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetAvailableSpaces_SomeUnavailable_GetsSpaces()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);

        await _context.Database.BeginTransactionAsync();

        DateTime now = DateTime.Now;
        Korisnik korisnik = await _userManager.Users.Where(x => x.UserRole!.Role!.Name == "Host").FirstAsync();
        var allSpaces = await _hostLogic.GetAvailableSpaces(now.AddDays(4.1).ToString("yyyy-MM-dd"), "20:00", "null", -1, korisnik);

        var expected = Utils.InitialSpaces.Where(x => x.ID == 3 || x.ID == 1).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(allSpaces, Has.Count.EqualTo(expected.Count));
            Assert.That(allSpaces, Is.EquivalentTo(expected).Using(new SpaceBasicDtoComparer()));
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetAvailableSpacesWrongDateTimeFormat_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);

        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.UserRole!.Role!.Name == "Host").FirstAsync();

        var exception = Assert.ThrowsAsync<InvalidDateTimeFormat>(async () =>
        {
            var allSpaces = await _hostLogic.GetAvailableSpaces(DateTime.Now.ToString(), "20:00", "null", -1, korisnik);
        });
        Assert.That(exception!.Message, Is.EqualTo("Invalid date and time format."));

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetSpacePlanUsingBannedUser_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);
        var _adminLogic = new AdministratorLogic(_userManager, _context);
        await _context.Database.BeginTransactionAsync();

        DateTime now = DateTime.Now;
        Korisnik korisnik = await _userManager.Users.Where(x => x.UserRole!.Role!.Name == "Host").FirstAsync();
        await _adminLogic.BanUser(new BanUserDto()
        {
            KorisnikId = korisnik.Id.ToString(),
            DatumOd = now,
            DatumDo = now.AddDays(10).ToString(),
            Razlog = "Testiranje"
        });

        var exception = Assert.ThrowsAsync<UnauthorizedException>(async () =>
        {
            await _hostLogic.GetSpacePlan(1, korisnik);
        });
        StringAssert.IsMatch(@"You are banned from the platform until (?<date>.+?)\. Reason: (?<reason>.+)", exception!.Message);


        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetSpacePlanForNonExistingSpace_ReturnsNull()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);
        var _adminLogic = new AdministratorLogic(_userManager, _context);
        await _context.Database.BeginTransactionAsync();

        DateTime now = DateTime.Now;
        Korisnik korisnik = await _userManager.Users.Where(x => x.UserRole!.Role!.Name == "Host").FirstAsync();

        var spacePlan = await _hostLogic.GetSpacePlan(1234, korisnik);
        Assert.That(spacePlan, Is.Null);

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetSpacePlan_GetsSpacePlan()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);
        var _adminLogic = new AdministratorLogic(_userManager, _context);
        await _context.Database.BeginTransactionAsync();

        DateTime now = DateTime.Now;
        Korisnik korisnik = await _userManager.Users.Where(x => x.UserRole!.Role!.Name == "Host").FirstAsync();

        var spacePlan = await _hostLogic.GetSpacePlan(1, korisnik);
        Assert.Multiple(() =>
        {
            Assert.That(spacePlan!.Grad, Is.Null);
            Assert.That(spacePlan!.Drzava, Is.Null);
            Assert.That(spacePlan!.Adresa, Is.Null);
            Assert.That(spacePlan!.SurfaceDimension!.Height, Is.EqualTo(661.5));
            Assert.That(spacePlan!.SurfaceDimension!.Width, Is.EqualTo(1712.6875));
            Assert.That(spacePlan!.Lines, Has.Count.EqualTo(5));
            Assert.That(spacePlan!.DraggableItems, Has.Count.EqualTo(26));
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task CancelingEvent_CancelsEvent()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);
        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.UserRole!.Role!.Name == "Host").FirstAsync();
        int eventId = await _context.Dogadjaji.Include(x => x.Organizator)
                                              .Where(x => x.Organizator!.Id == korisnik.Id && x.Status == StatusDogadjaja.Active)
                                              .Select(x => x.ID)
                                              .FirstAsync();

        await _hostLogic.CancelEvent(eventId, korisnik);

        var canceledEvent = await _context.Dogadjaji.Where(x => x.ID == eventId)
                                                  .FirstOrDefaultAsync();

        Assert.That(canceledEvent, Is.Null);

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task CancelingCanceledEvent_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);
        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.UserRole!.Role!.Name == "Host").FirstAsync();
        int eventId = await _context.Dogadjaji.Include(x => x.Organizator)
                                              .Where(x => x.Organizator!.Id == korisnik.Id && x.Status == StatusDogadjaja.Active)
                                              .Select(x => x.ID)
                                              .FirstAsync();

        await _hostLogic.CancelEvent(eventId, korisnik);

        var exception = Assert.ThrowsAsync<EventNotFoundException>(async () =>
        {
            await _hostLogic.CancelEvent(eventId, korisnik);
        });
        Assert.That(exception!.Message, Is.EqualTo("Event not found."));

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task CancelingElsesEvent_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);
        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.UserRole!.Role!.Name == "Host").FirstAsync();
        int eventId = await _context.Dogadjaji.Include(x => x.Organizator)
                                              .Where(x => x.Organizator!.Id != korisnik.Id && x.Status == StatusDogadjaja.Active)
                                              .Select(x => x.ID)
                                              .FirstAsync();

        var exception = Assert.ThrowsAsync<EventOwnerException>(async () =>
        {
            await _hostLogic.CancelEvent(eventId, korisnik);
        });
        Assert.That(exception!.Message, Is.EqualTo("You are not the host of this event."));

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetIncomingEvents_GetsEvents()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);
        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Organizer2").FirstAsync();

        var dogadjaji = await _hostLogic.GetIncomingEvents(korisnik);

        List<string> expectedEventNames = ["Bojan Sudjic", "The Little Prince"];

        Assert.Multiple(() =>
        {
            Assert.That(dogadjaji, Has.Count.EqualTo(2));
            Assert.That(dogadjaji.Select(x => x.Naziv).ToList(), Is.EquivalentTo(expectedEventNames));
            Assert.That(dogadjaji.Select(x => x.Slika).All(x => Utils.IsBase64String(x)), Is.True);
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetIncomingEventsAfterCanceling_GetsEvents()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);
        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Organizer2").FirstAsync();
        await _hostLogic.CancelEvent(3, korisnik);
        var dogadjaji = await _hostLogic.GetIncomingEvents(korisnik);

        List<string> expectedEventNames = ["The Little Prince"];

        Assert.Multiple(() =>
        {
            Assert.That(dogadjaji, Has.Count.EqualTo(1));
            Assert.That(dogadjaji.Select(x => x.Naziv).ToList(), Is.EquivalentTo(expectedEventNames));
            Assert.That(dogadjaji.Select(x => x.Slika).All(x => Utils.IsBase64String(x)), Is.True);
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetIncomingEventsAfterScheduelingEvent_GetsEvents()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);
        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Organizer2").FirstAsync();
        DateTime now = DateTime.Now;
        CreateEventDto dto = new()
        {
            Naziv = "Test dogadjaj",
            Opis = "Test opis",
            Datum = "2030-02-05",
            Vreme = "20:00",
            Tags = ["tag1", "tag2"],
            ProstorId = 1,
            Items = [],
            Lines = [],
            SurfaceDimension = new SurfaceDimensionDto() { Width = 1280, Height = 720 }
        };
        int id = await _hostLogic.CreateEvent(dto, korisnik);

        var rezervacija = await _context.RezervacijeProstora.Include(x => x.Dogadjaj)
                                                    .ThenInclude(x => x!.Organizator)
                                                    .Include(x => x.Prostor)
                                                    .Where(x => x.Dogadjaj!.ID == id)
                                                    .FirstOrDefaultAsync();
        rezervacija!.Status = StatusRezervacije.Confirmed;
        rezervacija.Dogadjaj!.Status = StatusDogadjaja.Active;
        _context.RezervacijeProstora.Update(rezervacija);
        await _context.SaveChangesAsync();
        var dogadjaji = await _hostLogic.GetIncomingEvents(korisnik);

        List<string> expectedEventNames = ["Bojan Sudjic", "The Little Prince", "Test dogadjaj"];

        Assert.Multiple(() =>
        {
            Assert.That(dogadjaji, Has.Count.EqualTo(3));
            Assert.That(dogadjaji.Select(x => x.Naziv).ToList(), Is.EquivalentTo(expectedEventNames));
            Assert.That(dogadjaji.Select(x => x.Slika).All(x => Utils.IsBase64String(x)), Is.True);
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetPastEvents_GetsEvents()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);
        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Organizer2").FirstAsync();

        var dogadjaji = await _hostLogic.GetPastEvents(korisnik);

        List<string> expectedEventNames = ["Z++"];

        Assert.Multiple(() =>
        {
            Assert.That(dogadjaji, Has.Count.EqualTo(1));
            Assert.That(dogadjaji.Select(x => x.Naziv).ToList(), Is.EquivalentTo(expectedEventNames));
            Assert.That(dogadjaji.Select(x => x.Slika).All(x => Utils.IsBase64String(x)), Is.True);
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetPastEventsAfterEventIsPast_GetsEvents()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);
        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Organizer2").FirstAsync();
        DateTime now = DateTime.Now;
        var eventToPass = await _context.Dogadjaji.Where(x => x.ID == 3).FirstAsync();
        eventToPass.Status = StatusDogadjaja.Passed;
        eventToPass.Vreme = now.AddDays(-1);
        await _context.SaveChangesAsync();

        var dogadjaji = await _hostLogic.GetPastEvents(korisnik);

        List<string> expectedEventNames = ["Z++", "Bojan Sudjic"];

        Assert.Multiple(() =>
        {
            Assert.That(dogadjaji, Has.Count.EqualTo(2));
            Assert.That(dogadjaji.Select(x => x.Naziv).ToList(), Is.EquivalentTo(expectedEventNames));
            Assert.That(dogadjaji.Select(x => x.Slika).All(x => Utils.IsBase64String(x)), Is.True);
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetPastEventsForBannedUser_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);
        var _adminLogic = new AdministratorLogic(_userManager, _context);
        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Organizer2").FirstAsync();
        DateTime now = DateTime.Now;
        await _adminLogic.BanUser(new BanUserDto()
        {
            KorisnikId = korisnik.Id.ToString(),
            DatumOd = now,
            DatumDo = now.AddDays(10).ToString(),
            Razlog = "Testiranje"
        });

        var exception = Assert.ThrowsAsync<UnauthorizedException>(async () =>
        {
            var dogadjaji = await _hostLogic.GetPastEvents(korisnik);
        });
        StringAssert.IsMatch(@"You are banned from the platform until (?<date>.+?)\. Reason: (?<reason>.+)", exception!.Message);

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetStatistics_GetsStatistics()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);
        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Organizer2").FirstAsync();

        var statistika = await _hostLogic.GetStatistics(korisnik);

        Assert.Multiple(() =>
        {
            Assert.That(statistika.HostedEvents, Is.EqualTo(1));
            Assert.That(statistika.AverageRating, Is.EqualTo(7));
            Assert.That(statistika.Reservations, Is.EqualTo(2));
            Assert.That(statistika.EstimatedEarnings, Is.EqualTo(120));
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetStatisticsAfterEventFinished_GetsStatistics()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);
        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Organizer2").FirstAsync();
        DateTime now = DateTime.Now;
        var eventToPass = await _context.Dogadjaji.Where(x => x.ID == 3).FirstAsync();
        eventToPass.Status = StatusDogadjaja.Passed;
        eventToPass.Vreme = now.AddDays(-1);
        await _context.SaveChangesAsync();

        var statistika = await _hostLogic.GetStatistics(korisnik);

        Assert.Multiple(() =>
        {
            Assert.That(statistika.HostedEvents, Is.EqualTo(2));
            Assert.That(statistika.AverageRating, Is.EqualTo(3.5));
            Assert.That(statistika.Reservations, Is.EqualTo(2));
            Assert.That(statistika.EstimatedEarnings, Is.EqualTo(120));
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetStatisticsAfterNewComment_GetsStatistics()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _hostLogic = new HostLogic(_context);
        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Organizer2").FirstAsync();

        var dogadjaj = await _context.Dogadjaji.Where(x => x.Naziv == "Z++").FirstAsync();

        var komentar = new Ocena
        {
            Korisnik = korisnik,
            Dogadjaj = dogadjaj,
            Komentar = "Test komentar 123",
            Vrednost = 3
        };

        await _context.Ocene.AddAsync(komentar);
        await _context.SaveChangesAsync();

        var statistika = await _hostLogic.GetStatistics(korisnik);

        Assert.Multiple(() =>
        {
            Assert.That(statistika.HostedEvents, Is.EqualTo(1));
            Assert.That(statistika.AverageRating, Is.EqualTo(5));
            Assert.That(statistika.Reservations, Is.EqualTo(2));
            Assert.That(statistika.EstimatedEarnings, Is.EqualTo(120));
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }
}