using System.Text;
using Microsoft.AspNetCore.Http;
using Moq;

namespace UnitTests;

[TestFixture]
public class ImageTests
{
    private static IFormFile CreateMockFile(string content, string fileName = "test.jpg", string contentType = "image/jpeg")
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var file = new Mock<IFormFile>();
        var ms = new MemoryStream(bytes);

        file.Setup(f => f.OpenReadStream()).Returns(ms);
        file.Setup(f => f.FileName).Returns(fileName);
        file.Setup(f => f.Length).Returns(bytes.Length);
        file.Setup(f => f.ContentType).Returns(contentType);
        file.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((stream, token) =>
            {
                ms.CopyTo(stream);
            })
            .Returns(Task.CompletedTask);

        return file.Object;
    }

    [Test]
    // [Ignore("Temp")]
    public async Task UploadImage_UploadsImage()
    {
        var (_, _, _context) = UserManagerHelper.CreateUserManager();
        var _imageLogic = new ImageLogic(_context);

        await _context.Database.BeginTransactionAsync();

        var dogadjajId = 5;
        var testContent = "This is a test image content";
        var file = CreateMockFile(testContent);

        await _imageLogic.UploadImage(dogadjajId, file);

        var dogadjaj = await _context.Dogadjaji.FindAsync(dogadjajId);
        Assert.That(dogadjaj!, Is.Not.Null);
        Assert.That(dogadjaj!.Slika, Is.Not.Null);

        var expectedBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(testContent));
        Assert.That(dogadjaj.Slika, Is.EqualTo(expectedBase64));

        await _context.Database.RollbackTransactionAsync();
        await _context.DisposeAsync();
    }


    [Test]
    // [Ignore("Temp")]
    public async Task UploadImage_EmptyFile_ThrowsException()
    {
        var (_, _, _context) = UserManagerHelper.CreateUserManager();
        var _imageLogic = new ImageLogic(_context);

        await _context.Database.BeginTransactionAsync();

        var dogadjajId = 5;
        var file = CreateMockFile("");

        var exception = Assert.ThrowsAsync<EmptyFileException>(async () =>
        {
            await _imageLogic.UploadImage(dogadjajId, file);
        });
        Assert.That(exception!.Message, Is.EqualTo("File is empty."));

        await _context.Database.RollbackTransactionAsync();
        await _context.DisposeAsync();
    }

    [Test]
    // [Ignore("Temp")]
    public async Task UploadImage_FileTooLarge_ThrowsException()
    {
        var (_, _, _context) = UserManagerHelper.CreateUserManager();
        var _imageLogic = new ImageLogic(_context);

        await _context.Database.BeginTransactionAsync();

        var dogadjajId = 5;
        var largeContent = new string('x', 10485761);
        var file = CreateMockFile(largeContent);

        var exception = Assert.ThrowsAsync<FileTooLargeException>(async () =>
        {
            await _imageLogic.UploadImage(dogadjajId, file);
        });
        Assert.That(exception!.Message, Is.EqualTo("File is too large."));

        await _context.Database.RollbackTransactionAsync();
        await _context.DisposeAsync();
    }
}