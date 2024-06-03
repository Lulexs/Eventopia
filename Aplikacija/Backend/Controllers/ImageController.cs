namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    public Context Context { get; set; }
    public ImageController(Context context)
    {
        Context = context;
    }

    [Authorize(Policy = "RequireHostRole")]
    [HttpPost("uploadImage")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
    {
        if (file == null)
        {
            return BadRequest("No file was uploaded.");
        }

        if (file.Length == 0)
        {
            return BadRequest("File is empty.");
        }

        if (file.Length > 10485760)
        {
            return BadRequest("File is too large.");
        }

        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();
            var base64String = Convert.ToBase64String(fileBytes);
            return Ok(base64String);
        }
    }

}