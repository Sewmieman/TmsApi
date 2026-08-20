using Microsoft.AspNetCore.Mvc;
using TmsApi.Infrastructure.Services;

[ApiController]
[Route("api/[controller]")]
public class CryptoDemoController : ControllerBase
{
    [HttpGet("test")]
    public IActionResult CryptoTest()
    {
        var service = new CryptoDemoService();

        string hash1 = service.HashUserPassword("Password123!");
        string hash2 = service.HashUserPassword("Password123!");

        bool match1 = service.VerifyUserPassword("Password123!", hash1);
        bool match2 = service.VerifyUserPassword("Password123!", hash2);

        Console.WriteLine($"Hash 1: {hash1}");
        Console.WriteLine($"Hash 2: {hash2}");

        return Ok(new
        {
            Hash1 = hash1,
            Hash2 = hash2,
            HashesAreDifferent = hash1 != hash2,
            Match1 = match1,
            Match2 = match2
        });
    }
}