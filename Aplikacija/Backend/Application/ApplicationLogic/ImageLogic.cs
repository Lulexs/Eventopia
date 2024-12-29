using Backend.ApplicationLogic.Exceptions;

namespace Backend.ApplicationLogic;

public class ImageLogic
{
    private Context _context;

    public ImageLogic(Context context)
    {
        _context = context;
    }

    public async Task UploadImage(int dogadjajId, IFormFile file)
    {
        var dogadjaj = await _context.Dogadjaji.FindAsync(dogadjajId);

        if (dogadjaj == null)
        {
            throw new EventNotFoundException("Event not found.");
        }

        if (file == null)
        {
            throw new MissingFileException("No file was uploaded.");
        }

        if (file.Length == 0)
        {
            throw new EmptyFileException("File is empty.");
        }

        if (file.Length > 10485760)
        {
            throw new FileTooLargeException("File is too large.");
        }

        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();
            var base64String = Convert.ToBase64String(fileBytes);
            dogadjaj.Slika = base64String;
            await _context.SaveChangesAsync();
        }
    }
}