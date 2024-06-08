namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class VisitorController : ControllerBase
{
    public Context Context { get; set; }
    private readonly UserManager<Korisnik> _userManager;

    public VisitorController(UserManager<Korisnik> userManager, Context context)
    {
        Context = context;
        _userManager = userManager;
    }

    [Authorize(Policy = "RequireVisitorRole")]
    [HttpPut("updateUserAvatarAndTags")]
    public async Task<IActionResult> UpdateUserAvatarAndTags([FromBody] ChangeAvatarTagsDto changeAvatarTagsDto)
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            return Unauthorized($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}");
        }

        if (korisnik == null)
            return NotFound("User not found.");

        korisnik!.SlikaProfila = changeAvatarTagsDto.Avatar;

        await Context.KorisnikTagovi.Include(x => x.Korisnik)
                                    .Where(x => x.Korisnik == korisnik)
                                    .ExecuteDeleteAsync();

        foreach (var tag in changeAvatarTagsDto.Tags!)
        {
            var userTag = await Context.UserTags.FirstOrDefaultAsync(x => x.TagName == tag);
            if (userTag == null)
            {
                userTag = new UserTag
                {
                    TagName = tag,
                };
                await Context.UserTags.AddAsync(userTag);
            }

            korisnik.Tagovi ??= new List<KorisnikTagovi>();

            korisnik.Tagovi.Add(new KorisnikTagovi { Korisnik = korisnik, UserTag = userTag });
        }

        var result = await _userManager.UpdateAsync(korisnik);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        await Context.SaveChangesAsync();

        return Ok();

    }

    [Authorize(Policy = "RequireVisitorRole")]
    [HttpGet("getAvatarAndTags")]
    public async Task<IActionResult> GetAvatarAndTags()
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            return Unauthorized($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}");
        }

        var tagovi = await Context.UserTags.Where(x => x.Korisnici!.Any(x => x.Korisnik == korisnik)).Select(x => x.TagName).ToListAsync();

        var avatarAndTags = new
        {
            Avatar = korisnik!.SlikaProfila,
            Tags = tagovi
        };

        return Ok(avatarAndTags);
    }


    [Authorize(Policy = "RequireVisitorRole")]
    [HttpGet("getStatistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            return Unauthorized($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}");
        }

        var rezervacije = await Context.Rezervacije.Include(x => x.Korisnik)
                                                    .Include(x => x.Dogadjaj)
                                                    .Include(x => x.Sto)
                                                    .Where(x => x.Korisnik == korisnik
                                                    && x.Dogadjaj!.Vreme < DateTime.Now)
                                                    .ToListAsync();

        int visitedEventsCount = rezervacije.Select(x => x.Dogadjaj!.ID).Distinct().Count();

        var rank = await Context.VisitorRanks
            .Where(x => x.Points <= visitedEventsCount)
            .OrderByDescending(x => x.Points)
            .FirstOrDefaultAsync();

        if (rank == null)
        {
            return BadRequest("No ranks found.");
        }

        var nextRank = await Context.VisitorRanks
            .Where(x => x.Points > visitedEventsCount)
            .OrderBy(x => x.Points)
            .FirstOrDefaultAsync();

        int nextRankPoints = nextRank?.Points ?? -1;

        korisnik!.VisitorRank = rank;

        var result = await _userManager.UpdateAsync(korisnik);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        int? moneySpent = rezervacije.Select(x => x.Sto!.Price * x.BrojMesta).Sum();

        var statistics = new
        {
            VisitedEventsCount = visitedEventsCount,
            MoneySpent = moneySpent,
            RankName = rank.RankName,
            Points = rank.Points,
            NextRankPoints = nextRankPoints
        };

        return Ok(statistics);
    }

    [Authorize(Policy = "RequireVisitorRole")]
    [HttpGet("getActiveReservations")]
    public async Task<IActionResult> GetActiveReservations()
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            return Unauthorized($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}");
        }

        var rezervacije = await Context.Rezervacije.Include(x => x.Korisnik)
                                                    .Include(x => x.Dogadjaj)
                                                    .Include(x => x.Sto)
                                                    .Where(x => x.Korisnik == korisnik
                                                    && x.Dogadjaj!.Vreme > DateTime.Now)
                                                    .ToListAsync();

        var activeReservations = rezervacije.Select(x => new
        {
            ReservationId = x.ID,
            Title = x.Dogadjaj!.Naziv,
            Date = x.Dogadjaj.Vreme,
            Image = x.Dogadjaj.Slika,
            Price = x.Sto!.Price * x.BrojMesta,
            Seats = x.BrojMesta
        }).OrderBy(x => x.Date);

        return Ok(activeReservations);
    }

    [Authorize(Policy = "RequireVisitorRole")]
    [HttpDelete("cancelReservation/{reservationId}")]
    public async Task<IActionResult> CancelReservation(int reservationId)
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            return Unauthorized($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}");
        }

        var rezervacija = await Context.Rezervacije.Include(x => x.Korisnik)
                                                    .Include(x => x.Dogadjaj)
                                                    .Include(x => x.Sto)
                                                    .FirstOrDefaultAsync(x => x.ID == reservationId);

        if (rezervacija == null)
        {
            return NotFound("Reservation not found.");
        }

        if (rezervacija.Korisnik != korisnik)
        {
            return Unauthorized("You are not authorized to cancel this reservation.");
        }

        if ((rezervacija.Dogadjaj!.Vreme - DateTime.Now) < TimeSpan.FromHours(24))
        {
            return BadRequest("Cannot cancel reservation for event that is less than 24 hours from now.");
        }

        rezervacija.Sto!.Reserved = false;

        Context.Rezervacije.Remove(rezervacija);

        await Context.SaveChangesAsync();

        return Ok();
    }

    [Authorize(Policy = "RequireVisitorRole")]
    [HttpGet("getVisitedEvents")]
    public async Task<IActionResult> GetVisitedEvents()
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            return Unauthorized($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}");
        }

        var dogadjaji = await Context.Dogadjaji.Include(x => x.Rezervacije!)
                                                    .ThenInclude(x => x.Korisnik)
                                                    .Include(x => x.Ocene!)
                                                    .ThenInclude(x => x.Korisnik)
                                                    .Where(x => x.Rezervacije!.Any(y => y.Korisnik == korisnik)
                                                    && x.Vreme < DateTime.Now
                                                    && x.Status == StatusDogadjaja.Passed)
                                                    .Distinct()
                                                    .ToListAsync();

        var visitedEvents = dogadjaji.Select(x => new
        {
            EventId = x.ID,
            Title = x.Naziv,
            Date = x.Vreme,
            Image = x.Slika,
            CanLeaveReview = !x.Ocene!.Any(y => y.Korisnik == korisnik)
        }).OrderByDescending(x => x.Date);

        return Ok(visitedEvents);
    }

    [Authorize(Policy = "RequireVisitorRole")]
    [HttpPost("postComment")]
    public async Task<IActionResult> PostComment([FromBody] CommentDto commentDto)
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            return Unauthorized($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}");
        }

        var dogadjaj = await Context.Dogadjaji.Include(x => x.Rezervacije!)
                                              .ThenInclude(x => x.Korisnik)
                                              .FirstOrDefaultAsync(x => x.ID == commentDto.DogadjajId);

        if (dogadjaj == null)
        {
            return NotFound("Event not found.");
        }

        if (!dogadjaj.Rezervacije!.Any(x => x.Korisnik == korisnik))
        {
            return Unauthorized("You can't comment on event you did not attend.");
        }

        if (dogadjaj.Vreme > DateTime.Now)
        {
            return BadRequest("You can't comment on event that has not happened yet.");
        }

        var komentar = new Ocena
        {
            Korisnik = korisnik,
            Dogadjaj = dogadjaj,
            Komentar = commentDto.Komentar,
            Vrednost = commentDto.Ocena
        };

        await Context.Ocene.AddAsync(komentar);
        await Context.SaveChangesAsync();

        return Ok();
    }


}