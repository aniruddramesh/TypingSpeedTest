using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

[ApiController]
[Route("[controller]")]
public class TypingTestController : ControllerBase
{
    private static readonly HttpClient client = new HttpClient(
        new HttpClientHandler {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });

    [HttpGet("quote")]
    public async Task<IActionResult> GetQuote()
    {
        var response = await client.GetStringAsync("https://api.quotable.io/random");
        dynamic json = JsonConvert.DeserializeObject(response);
        return Ok(new { sentence = json.content.ToString() });
    }

    [HttpPost("result")]
    public IActionResult GetResults([FromBody] TypingRequest input)
    {
        int correctChars = 0;
        for (int i = 0; i < input.Sentence.Length && i < input.Typed.Length; i++)
        {
            if (input.Sentence[i] == input.Typed[i]) correctChars++;
        }

        double accuracy = (double)correctChars / input.Sentence.Length * 100;
        double minutes = input.TimeInSeconds / 60.0;
        int words = input.Sentence.Split(' ').Length;
        double wpm = words / minutes;

        return Ok(new { wpm = wpm, accuracy = accuracy });
    }

    public class TypingRequest
    {
        public string Sentence { get; set; }
        public string Typed { get; set; }
        public double TimeInSeconds { get; set; }
    }
}
