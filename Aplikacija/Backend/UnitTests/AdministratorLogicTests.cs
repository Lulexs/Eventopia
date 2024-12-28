
namespace UnitTests;

[TestFixture]
public class AdministratorLogicTests
{
    [Test]
    // [Ignore("Temp")]
    public async Task BanUser_BansUser()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        string userId = "5F5A2EC0-A4CA-493F-E987-08DD25CB4A83";
        string razlog = "Neprikladan komentar";
        DateTime datumOd = DateTime.Now;
        string datumDo = datumOd.AddDays(7).ToString();

        await _adminLogic.BanUser(new BanUserDto
        {
            KorisnikId = userId,
            DatumOd = datumOd,
            DatumDo = datumDo,
            Razlog = razlog
        });

        var bannedUser = await _context.Zabrane.Where(x => x.Korisnik!.Id.ToString() == userId && x.DatumDo > DateTime.Now)
                                               .Select(x => new BannedDto
                                               {
                                                   Razlog = x.Razlog!,
                                                   DatumDo = x.DatumDo
                                               })
                                                .FirstOrDefaultAsync();

        Assert.Multiple(() =>
        {
            Assert.That(bannedUser, Is.Not.Null);
            Assert.That(bannedUser?.Razlog, Is.EqualTo(razlog));
            Assert.That(bannedUser?.DatumDo.ToString(), Is.EqualTo(datumDo));
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task BanningBannedUser_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        string userId = "5F5A2EC0-A4CA-493F-E987-08DD25CB4A83";
        string razlog = "Neprikladan komentar";
        DateTime datumOd = DateTime.Now;
        string datumDo = datumOd.AddDays(7).ToString();

        await _adminLogic.BanUser(new BanUserDto
        {
            KorisnikId = userId,
            DatumOd = datumOd,
            DatumDo = datumDo,
            Razlog = razlog
        });

        var exception = Assert.ThrowsAsync<AlreadyBannedException>(async () =>
        {
            await _adminLogic.BanUser(new BanUserDto
            {
                KorisnikId = userId,
                DatumOd = datumOd,
                DatumDo = datumDo,
                Razlog = razlog
            });
        });
        Assert.That(exception?.Message, Is.EqualTo("User is already banned."));

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task BanningNonExistantUser_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        string userId = Guid.Empty.ToString();
        string razlog = "Neprikladan komentar";
        DateTime datumOd = DateTime.Now;
        string datumDo = datumOd.AddDays(7).ToString();

        var exception = Assert.ThrowsAsync<UserNotFoundException>(async () =>
        {
            await _adminLogic.BanUser(new BanUserDto
            {
                KorisnikId = userId,
                DatumOd = datumOd,
                DatumDo = datumDo,
                Razlog = razlog
            });
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task UnbanningBannedUser_UnbansUser()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        string userId = "5F5A2EC0-A4CA-493F-E987-08DD25CB4A83";
        string razlog = "Neprikladan komentar";
        DateTime datumOd = DateTime.Now;
        string datumDo = datumOd.AddDays(7).ToString();

        await _adminLogic.BanUser(new BanUserDto
        {
            KorisnikId = userId,
            DatumOd = datumOd,
            DatumDo = datumDo,
            Razlog = razlog
        });

        var zabrana = await _context.Zabrane.Where(x => x.Korisnik!.Id.ToString() == userId).FirstOrDefaultAsync();

        await _adminLogic.UnbanUser(zabrana!.Id);

        zabrana = await _context.Zabrane.Where(x => x.Korisnik!.Id.ToString() == userId).FirstOrDefaultAsync();
        Assert.That(zabrana, Is.Null);

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]

    public async Task UnbanningUserNotBanned_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        int zabranaId = 123456;

        var exception = Assert.ThrowsAsync<BanNotFoundException>(async () =>
        {
            await _adminLogic.UnbanUser(zabranaId);
        });
        Assert.That(exception?.Message, Is.EqualTo("Ban does not exist"));

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]

    public async Task UnbanningAlreadyUnbannedUser_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        string userId = "5F5A2EC0-A4CA-493F-E987-08DD25CB4A83";
        string razlog = "Neprikladan komentar";
        DateTime datumOd = DateTime.Now;
        string datumDo = datumOd.AddDays(7).ToString();

        await _adminLogic.BanUser(new BanUserDto
        {
            KorisnikId = userId,
            DatumOd = datumOd,
            DatumDo = datumDo,
            Razlog = razlog
        });

        var zabrana = await _context.Zabrane.Where(x => x.Korisnik!.Id.ToString() == userId).FirstOrDefaultAsync();

        await _adminLogic.UnbanUser(zabrana!.Id);

        var exception = Assert.ThrowsAsync<BanNotFoundException>(async () =>
        {
            await _adminLogic.UnbanUser(zabrana.Id);
        });
        Assert.That(exception?.Message, Is.EqualTo("Ban does not exist"));

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task DeletingEvent_DeletesEvent()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        int eventId = 3;
        await _adminLogic.DeleteEvent(eventId);
        var deletedEvent = await _context.Dogadjaji.Where(x => x.ID == eventId).FirstOrDefaultAsync();

        Assert.That(deletedEvent, Is.Null);

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task DeletingNoExistingEvent_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        int eventId = 124356;

        var exception = Assert.ThrowsAsync<EventNotFoundException>(async () =>
        {
            await _adminLogic.DeleteEvent(eventId);
        });
        Assert.That(exception?.Message, Is.EqualTo("Event does not exist"));

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task DeletingDeletedEvent_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        int eventId = 3;
        await _adminLogic.DeleteEvent(eventId);
        var deletedEvent = await _context.Dogadjaji.Where(x => x.ID == eventId).FirstOrDefaultAsync();

        var exception = Assert.ThrowsAsync<EventNotFoundException>(async () =>
        {
            await _adminLogic.DeleteEvent(eventId);
        });
        Assert.That(exception?.Message, Is.EqualTo("Event does not exist"));

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task DeletingComment_DeletesComment()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        int commentId = 1;
        await _adminLogic.DeleteComment(commentId);
        var deletedComment = await _context.Ocene.Where(x => x.ID == commentId).FirstOrDefaultAsync();

        Assert.That(deletedComment, Is.Null);

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task DeletingNonExistingComment_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        int commentId = 123456;

        var exception = Assert.ThrowsAsync<CommentNotFoundException>(async () =>
        {
            await _adminLogic.DeleteComment(commentId);
        });
        Assert.That(exception?.Message, Is.EqualTo("Comment does not exist"));

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task DeletingDeletedComment_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        int commentId = 1;
        await _adminLogic.DeleteComment(commentId);
        var deletedComment = await _context.Ocene.Where(x => x.ID == commentId).FirstOrDefaultAsync();

        var exception = Assert.ThrowsAsync<CommentNotFoundException>(async () =>
        {
            await _adminLogic.DeleteComment(commentId);
        });
        Assert.That(exception?.Message, Is.EqualTo("Comment does not exist"));

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GettingAllEvents_GetsAllEvents()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        List<dynamic> allActiveEvents = [
            new { Id = 3, Naziv = "Bojan Sudjic"},
            new { Id = 4, Naziv = "The Little Prince"},
            new { Id = 5, Naziv = "Jelena Tomasevic"}
        ];

        var allEvents = await _adminLogic.GetAllEvents();

        Assert.Multiple(() =>
        {
            Assert.That(allEvents, Has.Count.EqualTo(allActiveEvents.Count));
            Assert.That(allEvents.All(ev => Utils.IsBase64String(ev.Slika)), Is.True);

            var actual = allEvents.Select(x => new { Id = x.ID, x.Naziv }).ToList();
            var expected = allActiveEvents.Select(x => new { x.Id, x.Naziv }).ToList();

            static bool comparer(dynamic a, dynamic b) => a.Id == b.Id && a.Naziv == b.Naziv;
            Assert.That(actual, Is.EquivalentTo(expected).Using<dynamic>(comparer));
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GettingAllEvents_GetsEmptyList()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        await _adminLogic.DeleteEvent(3);
        await _adminLogic.DeleteEvent(4);
        await _adminLogic.DeleteEvent(5);

        var allEvents = await _adminLogic.GetAllEvents();

        Assert.That(allEvents, Is.Empty);

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();

    }

    [Test]
    // [Ignore("Temp")]
    public async Task GettingAllEventsAfterInsert_GetsEvents()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);
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

        List<dynamic> allActiveEvents = [
            new { Id = 3, Naziv = "Bojan Sudjic"},
            new { Id = 4, Naziv = "The Little Prince"},
            new { Id = 5, Naziv = "Jelena Tomasevic"},
            new { Id = id, dto.Naziv }
        ];

        var allEvents = await _adminLogic.GetAllEvents();

        Assert.Multiple(() =>
        {
            Assert.That(allEvents, Has.Count.EqualTo(allActiveEvents.Count));
            Assert.That(allEvents.All(ev => Utils.IsBase64String(ev.Slika)), Is.True);

            var actual = allEvents.Select(x => new { Id = x.ID, x.Naziv }).ToList();
            var expected = allActiveEvents.Select(x => new { x.Id, x.Naziv }).ToList();

            static bool comparer(dynamic a, dynamic b) => a.Id == b.Id && a.Naziv == b.Naziv;
            Assert.That(actual, Is.EquivalentTo(expected).Using<dynamic>(comparer));
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();

    }

    [Test]
    // [Ignore("Temp")]
    public async Task GettingAllComments_GetsAllComments()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        List<ReturnOcenaDto> allComments = [
            new ReturnOcenaDto() { Id = 2, Komentar = "It was okay"},
            new ReturnOcenaDto() { Id = 1, Komentar = "Awesome experience!!"},
        ];

        var allCommentsActual = await _adminLogic.GetAllComments();

        Assert.Multiple(() =>
        {
            Assert.That(allCommentsActual, Has.Count.EqualTo(allComments.Count));

            static bool comparer(ReturnOcenaDto a, ReturnOcenaDto b) => a.Id == b.Id && a.Komentar == b.Komentar;
            Assert.That(allCommentsActual, Is.EquivalentTo(allComments).Using<ReturnOcenaDto>(comparer));
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GettingAllComments_GetsEmptyList()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        await _adminLogic.DeleteComment(1);
        await _adminLogic.DeleteComment(2);

        var allComments = await _adminLogic.GetAllComments();

        Assert.That(allComments, Is.Empty);

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();

    }

    [Test]
    // [Ignore("Temp")]
    public async Task GettingAllCommentsAfterInsert_GetsComments()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.UserRole!.Role!.Name == "Visitor").FirstAsync();
        Dogadjaj dogadjaj = await _context.Dogadjaji.Where(x => x.Status == StatusDogadjaja.Passed).FirstAsync();
        var komentar = new Ocena
        {
            Korisnik = korisnik,
            Dogadjaj = dogadjaj,
            Komentar = "Test Komentar",
            Vrednost = 5
        };

        await _context.Ocene.AddAsync(komentar);
        await _context.SaveChangesAsync();

        List<ReturnOcenaDto> allComments = [
            new ReturnOcenaDto() { Id = 2, Komentar = "It was okay"},
            new ReturnOcenaDto() { Id = 1, Komentar = "Awesome experience!!"},
            new ReturnOcenaDto() { Id = 3, Komentar = "Test Komentar"}
        ];

        var allCommentsActual = await _adminLogic.GetAllComments();

        Assert.Multiple(() =>
        {
            Assert.That(allCommentsActual, Has.Count.EqualTo(allComments.Count));

            static bool comparer(ReturnOcenaDto a, ReturnOcenaDto b) => a.Id == b.Id && a.Komentar == b.Komentar;
            Assert.That(allCommentsActual, Is.EquivalentTo(allComments).Using<ReturnOcenaDto>(comparer));
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();

    }

    [Test]
    // [Ignore("Temp")]
    public async Task GettingAllUsersWithNoBans_GetsUsers()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);
        await _context.Database.BeginTransactionAsync();

        var allUsers = await _adminLogic.GetUsersWithBans();

        Assert.Multiple(() =>
        {
            Assert.That(allUsers, Has.Count.EqualTo(Utils.InitialUsers.Count));
            Assert.That(allUsers, Is.EquivalentTo(Utils.InitialUsers).Using(new KorisnikSaZabranamaDtoComparer()));
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();

    }

    [Test]
    // [Ignore("Temp")]
    public async Task GettingAllUsersAfterBanningUsers_GetsUsers()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        var initialUsers = Utils.InitialUsers.Select(x => new KorisnikSaZabranamaDto
        {
            KorisnikId = x.KorisnikId,
            ZabranaId = x.ZabranaId,
            Ime = x.Ime,
            Prezime = x.Prezime,
            Avatar = x.Avatar,
            Role = x.Role,
            DatumOd = x.DatumOd,
            DatumDo = x.DatumDo,
            Razlog = x.Razlog
        }).ToList();

        DateTime now = DateTime.Now;
        string DatumDo = now.AddDays(7).ToString();
        int id1 = await _adminLogic.BanUser(new BanUserDto
        {
            KorisnikId = Utils.InitialUsers[0].KorisnikId!,
            DatumOd = now,
            DatumDo = DatumDo,
            Razlog = "Test razlog"
        });

        initialUsers[0].DatumOd = now;
        initialUsers[0].DatumDo = DateTime.Parse(DatumDo);
        initialUsers[0].Razlog = "Test razlog";
        initialUsers[0].ZabranaId = id1;

        int id2 = await _adminLogic.BanUser(new BanUserDto
        {
            KorisnikId = Utils.InitialUsers[1].KorisnikId!,
            DatumOd = now,
            DatumDo = DatumDo,
            Razlog = "Test razlog"
        });

        initialUsers[1].DatumOd = now;
        initialUsers[1].DatumDo = DateTime.Parse(DatumDo);
        initialUsers[1].Razlog = "Test razlog";
        initialUsers[1].ZabranaId = id2;

        var allUsers = await _adminLogic.GetUsersWithBans();

        Assert.Multiple(() =>
        {
            Assert.That(allUsers, Has.Count.EqualTo(Utils.InitialUsers.Count));
            Assert.That(allUsers, Is.EquivalentTo(initialUsers).Using(new KorisnikSaZabranamaDtoComparer()));
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GettingAllUsersAfterBanningAndUnbanningUser_GetsUsers()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _adminLogic = new AdministratorLogic(_userManager, _context);

        await _context.Database.BeginTransactionAsync();

        var initialUsers = Utils.InitialUsers.Select(x => new KorisnikSaZabranamaDto
        {
            KorisnikId = x.KorisnikId,
            ZabranaId = x.ZabranaId,
            Ime = x.Ime,
            Prezime = x.Prezime,
            Avatar = x.Avatar,
            Role = x.Role,
            DatumOd = x.DatumOd,
            DatumDo = x.DatumDo,
            Razlog = x.Razlog
        }).ToList();

        DateTime now = DateTime.Now;
        string DatumDo = now.AddDays(7).ToString();
        int id = await _adminLogic.BanUser(new BanUserDto
        {
            KorisnikId = Utils.InitialUsers[0].KorisnikId!,
            DatumOd = now,
            DatumDo = DatumDo,
            Razlog = "Test razlog"
        });

        await _adminLogic.UnbanUser(id);

        var allUsers = await _adminLogic.GetUsersWithBans();

        Assert.Multiple(() =>
        {
            Assert.That(allUsers, Has.Count.EqualTo(Utils.InitialUsers.Count));
            Assert.That(allUsers, Is.EquivalentTo(initialUsers).Using(new KorisnikSaZabranamaDtoComparer()));
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }
}