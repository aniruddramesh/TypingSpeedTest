using Microsoft.AspNetCore.Mvc;

namespace TypingSpeedAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TypingTestController : ControllerBase
    {
        [HttpGet("quote")]
        public IActionResult GetQuote([FromQuery] int words = 25)
        {
            var random = new Random();
            var wordList = new List<string>
            {
                "code", "keyboard", "performance", "developer", "syntax", "speed", "logic",
                "test", "algorithm", "compile", "variable", "method", "system", "console",
                "application", "backend", "frontend", "string", "integer", "object", "framework",
                "async", "response", "function", "parameter", "project", "language", "runtime"
            };

            var sentenceWords = new List<string>();
            for (int i = 0; i < words; i++)
            {
                sentenceWords.Add(wordList[random.Next(wordList.Count)]);
            }

            string sentence = string.Join(" ", sentenceWords) + ".";
            return Ok(new { sentence });
        }

        [HttpPost("result")]
        public IActionResult CalculateResults([FromBody] TypingResult model)
        {
            int totalWords = model.typed.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            int correctChars = 0;

            for (int i = 0; i < Math.Min(model.sentence.Length, model.typed.Length); i++)
            {
                if (model.sentence[i] == model.typed[i])
                    correctChars++;
            }

            double accuracy = (double)correctChars / model.sentence.Length * 100;
            double wpm = (totalWords / model.timeInSeconds) * 60;

            return Ok(new { accuracy, wpm });
        }
    }

    public class TypingResult
    {
        public string sentence { get; set; }
        public string typed { get; set; }
        public double timeInSeconds { get; set; }
    }
}
