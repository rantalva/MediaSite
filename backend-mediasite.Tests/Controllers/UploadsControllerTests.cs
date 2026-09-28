using Cloudflare.NET.R2;
using Cloudflare.NET.R2.Models;
using MediaSite_backend.Controllers;
using MediaSite_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MediaSite_backend.Tests.Controllers;

public class UploadsControllerTests
{
    private const string BucketName = "test-bucket";

    private readonly IR2Client _r2 = Substitute.For<IR2Client>();
    private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?> { ["R2:BucketName"] = BucketName })
        .Build();

    private UploadsController CreateController() => new(new StorageService(_r2, _configuration));

    private static IFormFile File(string filename, int length = 4)
    {
        var bytes = new byte[length];
        Array.Fill(bytes, (byte)1);
        return new FormFile(new MemoryStream(bytes), 0, length, "file", filename);
    }

    private static string MessageOf(IActionResult result) =>
        ((ObjectResult)result).Value!.ToString()!;

    [Fact]
    public async Task Upload_returns_BadRequest_when_no_file_supplied()
    {
        var result = await CreateController().Upload(null!);

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("No file uploaded.");
    }

    [Fact]
    public async Task Upload_returns_BadRequest_when_file_is_empty()
    {
        var result = await CreateController().Upload(File("tyhja.jpg", length: 0));

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("No file uploaded.");
    }

    [Theory]
    [InlineData("virus.exe")]
    [InlineData("skripti.sh")]
    [InlineData("muistiinpano.txt")]
    [InlineData("haitapa.html")]
    [InlineData("ei-paannetta")]
    public async Task Upload_returns_BadRequest_when_extension_not_allowed(string filename)
    {
        var result = await CreateController().Upload(File(filename));

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("Invalid file type.");
        await _r2.DidNotReceive().UploadAsync(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Stream>());
    }

    [Theory]
    [InlineData("valokuva.jpg")]
    [InlineData("valokuva.jpeg")]
    [InlineData("valokuva.png")]
    [InlineData("dokumentti.pdf")]
    public async Task Upload_returns_Ok_for_every_allowed_extension(string filename)
    {
        _r2.UploadAsync(BucketName, Arg.Any<string>(), Arg.Any<Stream>())
            .Returns(Task.FromResult<R2Result>(null!));

        var result = await CreateController().Upload(File(filename));

        result.Should().BeOfType<OkObjectResult>();
    }

    [Theory]
    [InlineData("VALOKUVA.JPG", ".jpg")]
    [InlineData("Valokuva.PNG", ".png")]
    [InlineData("Dokumentti.PDF", ".pdf")]
    public async Task Upload_lowercases_uppercase_extensions(string filename, string expectedExtension)
    {
        _r2.UploadAsync(BucketName, Arg.Any<string>(), Arg.Any<Stream>())
            .Returns(Task.FromResult<R2Result>(null!));

        var result = (OkObjectResult)await CreateController().Upload(File(filename));

        var key = (string)result.Value!.GetType().GetProperty("key")!.GetValue(result.Value)!;
        key.Should().EndWith(expectedExtension);
    }

    [Fact]
    public async Task Upload_generates_key_under_uploads_prefix()
    {
        _r2.UploadAsync(BucketName, Arg.Any<string>(), Arg.Any<Stream>())
            .Returns(Task.FromResult<R2Result>(null!));

        var result = (OkObjectResult)await CreateController().Upload(File("valokuva.jpg"));

        var key = (string)result.Value!.GetType().GetProperty("key")!.GetValue(result.Value)!;
        key.Should().MatchRegex(@"^uploads/[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}\.jpg$");
    }

    [Fact]
    public async Task Upload_generates_a_different_key_for_each_upload()
    {
        _r2.UploadAsync(BucketName, Arg.Any<string>(), Arg.Any<Stream>())
            .Returns(Task.FromResult<R2Result>(null!));

        var controller = CreateController();
        var first = await KeyOf(controller, "a.jpg");
        var second = await KeyOf(controller, "b.jpg");

        // Two identical filenames must not collide, otherwise the second upload overwrites the first.
        first.Should().NotBe(second);
    }

    [Fact]
    public async Task Upload_streams_file_to_R2_under_the_generated_key()
    {
        _r2.UploadAsync(BucketName, Arg.Any<string>(), Arg.Any<Stream>())
            .Returns(Task.FromResult<R2Result>(null!));

        var controller = CreateController();
        var expectedKey = await KeyOf(controller, "valokuva.jpg");

        await _r2.Received(1).UploadAsync(BucketName, expectedKey, Arg.Any<Stream>());
    }

    [Fact]
    public async Task Upload_streams_the_actual_file_bytes_to_R2()
    {
        byte[] captured = [];
        _r2.UploadAsync(BucketName, Arg.Any<string>(), Arg.Any<Stream>())
            .Returns(call =>
            {
                var stream = call.ArgAt<Stream>(2);
                using var buffer = new MemoryStream();
                stream.CopyTo(buffer);
                captured = buffer.ToArray();
                return Task.FromResult<R2Result>(null!);
            });

        await CreateController().Upload(File("valokuva.jpg", length: 8));

        captured.Should().HaveCount(8);
        captured.Should().OnlyContain(b => b == 1);
    }

    private static async Task<string> KeyOf(UploadsController controller, string filename)
    {
        var result = (OkObjectResult)await controller.Upload(File(filename));
        return (string)result.Value!.GetType().GetProperty("key")!.GetValue(result.Value)!;
    }
}
