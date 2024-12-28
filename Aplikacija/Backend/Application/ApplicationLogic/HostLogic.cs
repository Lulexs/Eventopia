using Backend.ApplicationLogic.Exceptions;

namespace Backend.ApplicationLogic;

public class HostLogic
{
    private readonly UserManager<Korisnik> _userManager;
    public Context Context { get; set; }

    public HostLogic(UserManager<Korisnik> userManager, Context context)
    {
        _userManager = userManager;
        Context = context;
    }

    public async Task<int> CreateEvent(CreateEventDto createEventDto, Korisnik korisnik)
    {
        string dateTimeString = $"{createEventDto.Datum} {createEventDto.Vreme}";
        DateTime dateTime;
        if (!DateTime.TryParseExact(dateTimeString, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
        {
            throw new InvalidDateTimeFormat("Invalid date and time format.");
        }

        if (dateTime < DateTime.Now)
        {
            throw new EventInPastException("Event date and time must be in the future.");
        }

        var prostor = await Context.Prostori.Include(x => x.PlanoviProstora)
                                            .FirstOrDefaultAsync(x => x.ID == createEventDto.ProstorId);

        if (prostor == null)
        {
            throw new SpaceNotFoundException("Space not found.");
        }

        List<DraggableItem> draggableItems = new List<DraggableItem>();

        int capacity = 0;

        PlanProstora planProstora = new PlanProstora
        {
            Prostor = prostor
        };

        foreach (DraggableItemDto draggableItemDto in createEventDto.Items!)
        {

            if (draggableItemDto.Tip.ToEnum<TipItema>() == TipItema.Table && draggableItemDto.BrojMesta == 0)
                draggableItemDto.BrojMesta = 4;


            if (draggableItemDto.Tip.ToEnum<TipItema>() == TipItema.Table)
                capacity += draggableItemDto.BrojMesta ?? 0;

            DraggableItem draggableItem = new DraggableItem
            {
                FrontID = draggableItemDto.FrontID,
                Tip = draggableItemDto.Tip.ToEnum<TipItema>(),
                Top = draggableItemDto.Top,
                Left = draggableItemDto.Left,
                Height = draggableItemDto.Height,
                HeightFactor = draggableItemDto.HeightFactor,
                BrojMesta = draggableItemDto.BrojMesta,
                Reserved = draggableItemDto.Reserved,
                Price = draggableItemDto.Price,
                PlanProstora = planProstora
            };

            draggableItems.Add(draggableItem);
        }

        planProstora.DraggableItems = draggableItems;
        planProstora.Kapacitet = capacity;

        List<Line> lines = new List<Line>();

        foreach (LineDto lineDto in createEventDto.Lines!)
        {
            Line line = new Line
            {
                X1 = lineDto.X1,
                Y1 = lineDto.Y1,
                X2 = lineDto.X2,
                Y2 = lineDto.Y2,
                PlanProstora = planProstora
            };
            lines.Add(line);
        }

        planProstora.Lines = lines;

        SurfaceDimension surfaceDimension = new SurfaceDimension
        {
            Width = createEventDto.SurfaceDimension!.Width,
            Height = createEventDto.SurfaceDimension!.Height,
            PlanProstora = planProstora
        };

        await Context.SurfaceDimensions.AddAsync(surfaceDimension);
        await Context.PlanoviProstora.AddAsync(planProstora);

        var rezervacijaProstora = new RezervacijaProstora
        {
            VremeOd = dateTime.AddHours(-12),
            VremeDo = dateTime.AddHours(12),
            Status = StatusRezervacije.WaitingConfirmation,
            Prostor = prostor
        };

        await Context.RezervacijeProstora.AddAsync(rezervacijaProstora);

        var dogadjaj = new Dogadjaj
        {
            Naziv = createEventDto.Naziv,
            Opis = createEventDto.Opis,
            Vreme = dateTime,
            Organizator = korisnik,
            VideoLink = createEventDto.Video,
            Status = StatusDogadjaja.WaitingForSpaceApproval,
            Slika = "",
            PlanProstora = planProstora,
        };

        await Context.Dogadjaji.AddAsync(dogadjaj);

        dogadjaj.RezervacijaProstora = rezervacijaProstora;
        rezervacijaProstora.Dogadjaj = dogadjaj;

        List<Tag> tags = new List<Tag>();
        foreach (var tag in createEventDto.Tags!)
        {
            var existingTag = await Context.Tagovi.Include(x => x.Dogadjaji).FirstOrDefaultAsync(x => x.TagName == tag);
            if (existingTag != null)
            {
                tags.Add(existingTag);
                existingTag.Dogadjaji!.Add(dogadjaj);
            }
            else
            {
                Tag newTag = new Tag
                {
                    TagName = tag,
                    Dogadjaji = new List<Dogadjaj> { dogadjaj }
                };
                await Context.Tagovi.AddAsync(newTag);
                tags.Add(newTag);
            }
        }

        await Context.SaveChangesAsync();
        return dogadjaj.ID;
    }
}