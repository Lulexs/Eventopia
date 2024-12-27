namespace UnitTests;

[TestFixture]
public class AdministratorLogicTests
{
    private UserManager<Korisnik> _userManager = null!;
    private Context _context = null!;
    private AdministratorLogic _adminLogic = null!;

    [OneTimeSetUp]
    public void BeforeAllSetup()
    {
        (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        _adminLogic = new AdministratorLogic(_userManager, _context);
    }

    [OneTimeTearDown]
    public void AfterAllTests()
    {
        _context.Dispose();
    }

    [Test]
    public async Task BanUser_BansUser()
    {
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

        var zabrana = await _context.Zabrane.Where(x => x.Korisnik!.Id.ToString() == userId).FirstOrDefaultAsync();
        if (zabrana != null)
        {
            _context.Zabrane.Remove(zabrana);
            await _context.SaveChangesAsync();
        }
    }

    [Test]
    public async Task BanningBannedUser_ThrowsException()
    {
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

        var zabrana = await _context.Zabrane.Where(x => x.Korisnik!.Id.ToString() == userId).FirstOrDefaultAsync();
        if (zabrana != null)
        {
            _context.Zabrane.Remove(zabrana);
            await _context.SaveChangesAsync();
        }
        var bannedUser = await _context.Zabrane.Where(x => x.Korisnik!.Id.ToString() == userId && x.DatumDo > DateTime.Now)
                                               .Select(x => new BannedDto
                                               {
                                                   Razlog = x.Razlog!,
                                                   DatumDo = x.DatumDo
                                               })
                                                .FirstOrDefaultAsync();
        Assert.That(bannedUser, Is.Null);
    }

    [Test]
    public void BanningNonExistantUser_ThrowsException()
    {
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
    }

    [Test]
    public async Task UnbanningBannedUser_UnbansUser()
    {
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
    }

    [Test]
    public void UnbanningUserNotBanned_ThrowsException()
    {
        int zabranaId = 123456;

        var exception = Assert.ThrowsAsync<BanNotFoundException>(async () =>
        {
            await _adminLogic.UnbanUser(zabranaId);
        });
        Assert.That(exception?.Message, Is.EqualTo("Ban does not exist"));
    }

    [Test]
    public async Task UnbanningAlreadyUnbannedUser_ThrowsException()
    {
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
    }

    [Test]
    public async Task DeletingEvent_DeletesEvent()
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        int eventId = 3;
        await _adminLogic.DeleteEvent(eventId);
        var deletedEvent = await _context.Dogadjaji.Where(x => x.ID == eventId).FirstOrDefaultAsync();

        Assert.That(deletedEvent, Is.Null);

        await transaction.RollbackAsync();
    }

    [Test]
    public void DeletingNoExistingEvent_ThrowsException()
    {
        int eventId = 124356;

        var exception = Assert.ThrowsAsync<EventNotFoundException>(async () =>
        {
            await _adminLogic.DeleteEvent(eventId);
        });
        Assert.That(exception?.Message, Is.EqualTo("Event does not exist"));
    }

    [Test]
    public async Task DeletingDeletedEvent_ThrowsException()
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        int eventId = 3;
        await _adminLogic.DeleteEvent(eventId);
        var deletedEvent = await _context.Dogadjaji.Where(x => x.ID == eventId).FirstOrDefaultAsync();

        var exception = Assert.ThrowsAsync<EventNotFoundException>(async () =>
        {
            await _adminLogic.DeleteEvent(eventId);
        });
        Assert.That(exception?.Message, Is.EqualTo("Event does not exist"));

        await transaction.RollbackAsync();
    }

    [Test]
    public async Task DeletingComment_DeletesComment()
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        int commentId = 1;
        await _adminLogic.DeleteComment(commentId);
        var deletedComment = await _context.Ocene.Where(x => x.ID == commentId).FirstOrDefaultAsync();

        Assert.That(deletedComment, Is.Null);

        await transaction.RollbackAsync();
    }

    [Test]
    public void DeletingNonExistingComment_ThrowsException()
    {
        int commentId = 123456;

        var exception = Assert.ThrowsAsync<CommentNotFoundException>(async () =>
        {
            await _adminLogic.DeleteComment(commentId);
        });
        Assert.That(exception?.Message, Is.EqualTo("Comment does not exist"));
    }

    [Test]
    public async Task DeletingDeletedComment_ThrowsException()
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        int commentId = 1;
        await _adminLogic.DeleteComment(commentId);
        var deletedComment = await _context.Ocene.Where(x => x.ID == commentId).FirstOrDefaultAsync();

        var exception = Assert.ThrowsAsync<CommentNotFoundException>(async () =>
        {
            await _adminLogic.DeleteComment(commentId);
        });
        Assert.That(exception?.Message, Is.EqualTo("Comment does not exist"));

        await transaction.RollbackAsync();
    }


}