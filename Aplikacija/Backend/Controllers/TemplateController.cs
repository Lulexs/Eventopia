namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TemplateController : ControllerBase
{
    public Context Context { get; set; }
    public TemplateController(Context context)
    {
        Context = context;
    }

}