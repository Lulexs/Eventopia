using Backend.ApplicationLogic;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    private ImageLogic _imageLogic;
    public ImageController(ImageLogic imageLogic)
    {
        _imageLogic = imageLogic;
    }

    [Authorize(Policy = "RequireHostRole")]
    [HttpPost("uploadImage/{dogadjajId}")]
    public async Task<IActionResult> UploadImage([FromRoute] int dogadjajId, [FromForm] IFormFile file)
    {
        try
        {
            await _imageLogic.UploadImage(dogadjajId, file);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

}