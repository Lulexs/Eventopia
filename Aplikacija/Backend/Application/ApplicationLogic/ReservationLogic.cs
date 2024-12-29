using Backend.ApplicationLogic.Exceptions;

namespace Backend.ApplicationLogic;

public class ReservationLogic
{
    public Context Context { get; set; }

    public ReservationLogic(Context context)
    {
        Context = context;
    }

    public async Task<SpaceDto?> GetSpacePlan(int eventId, Korisnik korisnik)
    {
        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            throw new UnauthorizedException($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}");
        }

        SpaceDto? spacePlan = await Context.Dogadjaji.Include(x => x.PlanProstora)
                                               .ThenInclude(x => x!.DraggableItems!)
                                               .ThenInclude(x => x.Rezervacija)
                                               .ThenInclude(x => x!.Korisnik)
                                               .Include(x => x.PlanProstora)
                                               .ThenInclude(x => x!.Lines)
                                               .Include(x => x.PlanProstora)
                                               .ThenInclude(x => x!.SurfaceDimension)
                                               .Where(x => x.ID == eventId)
                                               .Select(x => new SpaceDto
                                               {
                                                   ID = x.PlanProstora!.ID,
                                                   DraggableItems = x.PlanProstora!.DraggableItems!.Select(y => new DraggableItemDto
                                                   {
                                                       ID = y.ID,
                                                       FrontID = y.FrontID,
                                                       Tip = y.Tip.ToString().ToLower(),
                                                       Top = y.Top,
                                                       Left = y.Left,
                                                       Height = y.Height,
                                                       HeightFactor = y.HeightFactor,
                                                       BrojMesta = y.BrojMesta,
                                                       Reserved = y.Reserved,
                                                       Price = y.Price,
                                                       ReservationId = y.Rezervacija!.Korisnik == korisnik ? y.Rezervacija.ID : -1,
                                                       ReservedSeats = y.Rezervacija!.BrojMesta
                                                   }).ToList(),
                                                   Lines = x.PlanProstora.Lines!.Select(y => new LineDto
                                                   {
                                                       X1 = y.X1,
                                                       Y1 = y.Y1,
                                                       X2 = y.X2,
                                                       Y2 = y.Y2
                                                   }).ToList(),
                                                   SurfaceDimension = new SurfaceDimensionDto
                                                   {
                                                       Width = x.PlanProstora!.SurfaceDimension!.Width,
                                                       Height = x.PlanProstora!.SurfaceDimension!.Height
                                                   }
                                               })
                                               .FirstOrDefaultAsync();

        return spacePlan;
    }

    public async Task<EventDetailsDto?> GetEventDetails(int eventId, Korisnik korisnik)
    {
        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            throw new UnauthorizedException($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}");
        }

        EventDetailsDto? eventDetails = await Context.Dogadjaji.Include(x => x.PlanProstora)
                                               .ThenInclude(x => x!.Prostor)
                                               .Where(x => x.ID == eventId)
                                               .Select(x => new EventDetailsDto
                                               {
                                                   Opis = x.Opis,
                                                   Latitude = x.PlanProstora!.Prostor!.Latitude,
                                                   Longitude = x.PlanProstora!.Prostor!.Longitude
                                               })
                                               .FirstOrDefaultAsync();

        return eventDetails;
    }

    public async Task<int> MakeReservation(int tableId, int numberOfSeats, Korisnik korisnik)
    {
        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            throw new UnauthorizedException($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}.");
        }

        var table = await Context.DraggableItems.Include(x => x.PlanProstora)
                                                .ThenInclude(x => x!.Dogadjaj)
                                                .ThenInclude(x => x!.Rezervacije)
                                                .FirstOrDefaultAsync(x => x.ID == tableId);

        if (table == null)
            throw new TableNotFoundException("Table not found.");

        if (table.Tip != TipItema.Table)
            throw new NotTableException("Selected item is not a table.");

        if (table.BrojMesta < numberOfSeats)
            throw new MaximumSeatsForTableException($"Table only has {table.BrojMesta} seats available.");

        if (numberOfSeats < 0.75 * table.BrojMesta)
            throw new MinumumTablesForEventException("You can only reserve at least 75% of the table seats.");

        if (table.Reserved == true)
            throw new TableReservedException("Table is already reserved.");

        var dogadjaj = table.PlanProstora!.Dogadjaj;

        if (dogadjaj == null)
            throw new EventNotFoundException("Event not found.");

        if (dogadjaj.Vreme < DateTime.Now)
            throw new EventInPastException("Event has already passed.");

        int count = dogadjaj.Rezervacije!.Where(x => x.Korisnik == korisnik).Count();

        if (count >= 2)
            throw new MaximumTablesForEventException("You have already reserved maximum number of tables for this event.");

        var reservation = new Rezervacija
        {
            BrojMesta = numberOfSeats,
            Sto = table,
            Dogadjaj = dogadjaj,
            Korisnik = korisnik
        };

        Context.Rezervacije.Add(reservation);
        table.Reserved = true;

        await Context.SaveChangesAsync();
        return reservation.ID;

    }
}