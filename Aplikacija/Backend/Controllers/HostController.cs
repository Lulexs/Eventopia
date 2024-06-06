namespace Backend.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class HostController : ControllerBase
    {

        private readonly UserManager<Korisnik> _userManager;
        public Context Context { get; set; }
        public HostController(Context context, UserManager<Korisnik> userManager)
        {
            Context = context;
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireHostRole")]
        [HttpPost("createEvent")]
        public async Task<ActionResult> newEvent([FromBody] CreateEventDto createEventDto)
        {
            var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
            if (korisnik == null)
            {
                return NotFound("User not found.");
            }

            string dateTimeString = $"{createEventDto.Datum} {createEventDto.Vreme}";
            DateTime dateTime;
            if (DateTime.TryParseExact(dateTimeString, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {

                List<Tag> tags = new List<Tag>();
                foreach (var tag in createEventDto.Tags)
                {
                    Tag existingTag = await Context.Tagovi.FirstOrDefaultAsync(x => x.TagName == tag);
                    if (existingTag != null)
                    {
                        tags.Add(existingTag);
                    }
                    else
                    {
                        Tag newTag = new Tag
                        {
                            TagName = tag,
                            Dogadjaji = new List<Dogadjaj>()
                        };
                        tags.Add(newTag);
                    }
                }
                var dogadjaj = new Dogadjaj
                {
                    Naziv = createEventDto.Naziv,
                    Opis = createEventDto.Opis,
                    Slika = createEventDto.Slika,
                    Vreme = dateTime,
                    Organizator = korisnik,
                    VideoLink = createEventDto.Video,
                    Status = StatusDogadjaja.Active, // nisam siguran sta bi trebalo da se prenese
                    Tagovi = tags,
                    //ne znam za rezervacijaProstora i Rezervacija treba
                };

                foreach (var tag in dogadjaj.Tagovi)
                {
                    Tag existingTag = await Context.Tagovi.FirstOrDefaultAsync(x => x.TagName == tag.TagName);
                    if (existingTag != null)
                    {
                        existingTag.Dogadjaji.Add(dogadjaj);
                    }
                    else
                    {
                        Context.Tagovi.Update(tag);
                    }

                }

                await Context.Dogadjaji.AddAsync(dogadjaj);
            }
            else
            {
                return BadRequest("Invalid date and time format.");
            }


            return Ok();
        }


        [Authorize(Policy = "RequireHostRole")]
        [HttpPost("manageEvent")]
        public async Task<ActionResult> manageEvent([FromBody] CreateEventDto createEventDto)
        {
            var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
            if (korisnik == null)
            {
                return NotFound("User not found.");
            }

            return Ok();
        }


        [Authorize(Policy = "RequireHostRole")]
        [HttpDelete("deleteEvent/{id}")]
        public async Task<ActionResult> deleteEvent([FromRoute] int id)
        {
            var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
            if (korisnik == null)
            {
                return NotFound("User not found.");
            }

            var dogadjaj = await Context.Dogadjaji.FirstOrDefaultAsync(x => x.Organizator == korisnik && x.ID == id);
            if (dogadjaj == null)
            {
                return NotFound("Event not found.");
            }

            Context.Dogadjaji.Remove(dogadjaj);
            await Context.SaveChangesAsync();
            return Ok();
        }


        [Authorize(Policy = "RequireHostRole")]
        [HttpGet("getIncomingEvents")]
        public async Task<ActionResult> getIncomingEvents()
        {
            var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
            if (korisnik == null)
            {
                return NotFound("User not found.");
            }

            var dogadjaji = await Context.Dogadjaji.Where(x => x.Organizator == korisnik && x.Vreme > DateTime.Now).ToListAsync();

            if (dogadjaji == null)
            {
                return NotFound("No incoming events found for the given host.");
            }

            List<EventForListDto> events = new List<EventForListDto>();
            foreach (var dogadjaj in dogadjaji)
            {
                events.Add(new EventForListDto
                {
                    Naziv = dogadjaj.Naziv,
                    Slika = dogadjaj.Slika,
                    Datum = dogadjaj.Vreme.ToString("dd.MM.yyyy."),
                });
            }


            return Ok(events);
        }




        [Authorize(Policy = "RequireHostRole")]
        [HttpGet("getPastEvents")]
        public async Task<ActionResult> getPastEvents()
        {
            var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
            if (korisnik == null)
            {
                return NotFound("User not found.");
            }

            var dogadjaji = await Context.Dogadjaji.Where(x => x.Organizator == korisnik && x.Vreme < DateTime.Now).ToListAsync();
            if (dogadjaji == null)
            {
                return NotFound("No past events found for the given host.");
            }

            if (dogadjaji == null)
            {
                return NotFound("No past events found for the given host.");
            }
            List<EventForListDto> events = new List<EventForListDto>();

            foreach (var dogadjaj in dogadjaji)
            {
                events.Add(new EventForListDto
                {
                    Naziv = dogadjaj.Naziv,
                    Slika = dogadjaj.Slika,
                    Datum = dogadjaj.Vreme.ToString("dd.MM.yyyy."),
                });
            }

            //kako zelim DTO da bude
            return Ok(events);
        }


        [Authorize(Policy = "RequireHostRole")]
        [HttpGet("getReviewsForEvent/{id}")]
        public async Task<ActionResult> getReviewsForEvent([FromRoute] int id)
        {
            var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
            if (korisnik == null)
            {
                return NotFound("User not found.");
            }

            var dogadjaj = await Context.Dogadjaji.FirstOrDefaultAsync(x => x.Organizator == korisnik && x.ID == id);
            if (dogadjaj == null)
            {
                return NotFound("Event not found.");
            }

            List<Ocena> ocene = await Context.Ocene.Where(x => x.Dogadjaj == dogadjaj).ToListAsync();
            List<ReviewDto> reviews = new List<ReviewDto>();

            //json namesti za ReviewDto
            foreach (var oc in ocene)
            {
                reviews.Add(new ReviewDto
                {
                    Vrednost = oc.Vrednost,
                    Komentar = oc.Komentar,
                    Korisnik = $"{oc.Korisnik.Ime} {oc.Korisnik.Prezime}",
                    VremeKomentara = oc.VremeKomentara,

                });
            }

            return Ok(reviews);
        }

    }
}