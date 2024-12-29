namespace UnitTests;

[TestFixture]
public class ReservationTests
{
    [Test]
    // [Ignore("Temp")]
    public async Task MakeReservation_MakesReservation()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _reservationLogic = new ReservationLogic(_context);

        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Visitor1").FirstAsync();
        int tableId = 190;
        int numOfSeats = 3;

        int resId = await _reservationLogic.MakeReservation(tableId, numOfSeats, korisnik);

        var reservation = await _context.Rezervacije.Include(x => x.Dogadjaj).Where(x => x.ID == resId).FirstOrDefaultAsync();

        Assert.Multiple(() =>
        {
            Assert.That(reservation, Is.Not.Null);
            Assert.That(reservation!.BrojMesta, Is.EqualTo(3));
            Assert.That(reservation.Dogadjaj!.ID, Is.EqualTo(4));
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task MakeReservationInvalidNumOfSeats_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _reservationLogic = new ReservationLogic(_context);

        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Visitor1").FirstAsync();
        int tableId = 190;
        int numOfSeats = 1;

        var exception = Assert.ThrowsAsync<MinumumTablesForEventException>(async () =>
        {
            int resId = await _reservationLogic.MakeReservation(tableId, numOfSeats, korisnik);
        });
        Assert.That(exception!.Message, Is.EqualTo("You can only reserve at least 75% of the table seats."));

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task MakeReservationInvalidTable_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _reservationLogic = new ReservationLogic(_context);

        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Visitor1").FirstAsync();
        int tableId = 19901;
        int numOfSeats = 4;

        var exception = Assert.ThrowsAsync<TableNotFoundException>(async () =>
        {
            int resId = await _reservationLogic.MakeReservation(tableId, numOfSeats, korisnik);
        });
        Assert.That(exception!.Message, Is.EqualTo("Table not found."));

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task MakeReservationNotTable_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _reservationLogic = new ReservationLogic(_context);

        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Visitor1").FirstAsync();
        int tableId = 175;
        int numOfSeats = 4;

        var exception = Assert.ThrowsAsync<NotTableException>(async () =>
        {
            int resId = await _reservationLogic.MakeReservation(tableId, numOfSeats, korisnik);
        });
        Assert.That(exception!.Message, Is.EqualTo("Selected item is not a table."));

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task MakeReservationMoreSeatsThanAvailable_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _reservationLogic = new ReservationLogic(_context);

        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Visitor1").FirstAsync();
        int tableId = 190;
        int numOfSeats = 5;

        var exception = Assert.ThrowsAsync<MaximumSeatsForTableException>(async () =>
        {
            int resId = await _reservationLogic.MakeReservation(tableId, numOfSeats, korisnik);
        });
        Assert.That(exception!.Message, Is.EqualTo("Table only has 4 seats available."));

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task MakeReservationTableAlreadyReserved_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _reservationLogic = new ReservationLogic(_context);

        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Visitor1").FirstAsync();
        int tableId = 190;
        int numOfSeats = 4;
        int resId = await _reservationLogic.MakeReservation(tableId, numOfSeats, korisnik);

        var exception = Assert.ThrowsAsync<TableReservedException>(async () =>
        {
            int resId = await _reservationLogic.MakeReservation(tableId, numOfSeats, korisnik);
        });
        Assert.That(exception!.Message, Is.EqualTo("Table is already reserved."));

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetEventDetails_GetsEventDetails()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _reservationLogic = new ReservationLogic(_context);
        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Visitor1").FirstAsync();
        int eventId = 5;

        var eventDetails = await _reservationLogic.GetEventDetails(eventId, korisnik);

        var expected = new EventDetailsDto()
        {
            Opis = "Dvadeset godina blistave karijere, muzička zvezda Jelena Tomašević obeležiće koncertom u Sava Centru, koji će izvesti u pratnji Simfonijskog orkestra Radio - televizije Srbije.  ",
            Latitude = 43.825652195651216,
            Longitude = 18.366999318693814
        };

        Assert.Multiple(() =>
        {
            Assert.That(eventDetails, Is.Not.Null);
            Assert.That(eventDetails!.Opis, Is.EqualTo(expected.Opis));
            Assert.That(eventDetails!.Latitude, Is.EqualTo(expected.Latitude));
            Assert.That(eventDetails!.Longitude, Is.EqualTo(expected.Longitude));
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetEventDetailsWithBannedUser_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _reservationLogic = new ReservationLogic(_context);
        var _adminLogic = new AdministratorLogic(_userManager, _context);
        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Visitor1").FirstAsync();
        int eventId = 5;
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
            var eventDetails = await _reservationLogic.GetEventDetails(eventId, korisnik);
        });
        StringAssert.IsMatch(@"You are banned from the platform until (?<date>.+?)\. Reason: (?<reason>.+)", exception!.Message);

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetEventDetailsAfterCancelingEvent_GetsNull()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _reservationLogic = new ReservationLogic(_context);
        var _hostLogic = new HostLogic(_context);
        await _context.Database.BeginTransactionAsync();

        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Visitor1").FirstAsync();
        int eventId = 5;
        await _hostLogic.CancelEvent(eventId, await _userManager.Users.Where(x => x.Prezime == "Organizer1").FirstAsync());

        var eventDetails = await _reservationLogic.GetEventDetails(eventId, korisnik);
        Assert.That(eventDetails, Is.Null);

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetSpacePlan_GetsSpacePlan()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _reservationLogic = new ReservationLogic(_context);
        await _context.Database.BeginTransactionAsync();

        DateTime now = DateTime.Now;
        Korisnik korisnik = await _userManager.Users.Where(x => x.UserRole!.Role!.Name == "Host").FirstAsync();

        var spacePlan = await _reservationLogic.GetSpacePlan(5, korisnik);
        Assert.Multiple(() =>
        {
            Assert.That(spacePlan!.Grad, Is.Null);
            Assert.That(spacePlan!.Drzava, Is.Null);
            Assert.That(spacePlan!.Adresa, Is.Null);
            Assert.That(spacePlan!.SurfaceDimension!.Height, Is.EqualTo(661.5));
            Assert.That(spacePlan!.SurfaceDimension!.Width, Is.EqualTo(1712.6875));
            Assert.That(spacePlan!.Lines, Has.Count.EqualTo(4));
            Assert.That(spacePlan!.DraggableItems, Has.Count.EqualTo(41));
        });

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task GetSpacePlanByBannedUser_ThrowsException()
    {
        var (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        var _reservationLogic = new ReservationLogic(_context);
        var _adminLogic = new AdministratorLogic(_userManager, _context);
        await _context.Database.BeginTransactionAsync();

        DateTime now = DateTime.Now;
        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Visitor2").FirstAsync();
        await _adminLogic.BanUser(new BanUserDto()
        {
            KorisnikId = korisnik.Id.ToString(),
            DatumOd = now,
            DatumDo = now.AddDays(10).ToString(),
            Razlog = "Testiranje"
        });

        var exception = Assert.ThrowsAsync<UnauthorizedException>(async () =>
        {
            await _reservationLogic.GetSpacePlan(1, korisnik);
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
        var _reservationLogic = new ReservationLogic(_context);
        await _context.Database.BeginTransactionAsync();

        DateTime now = DateTime.Now;
        Korisnik korisnik = await _userManager.Users.Where(x => x.Prezime == "Visitor2").FirstAsync();

        var spacePlan = await _reservationLogic.GetSpacePlan(1234, korisnik);
        Assert.That(spacePlan, Is.Null);

        await _context.Database.RollbackTransactionAsync();
        _userManager.Dispose();
        await _context.DisposeAsync();
    }
}