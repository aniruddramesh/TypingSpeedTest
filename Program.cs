using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Program
{
    static async Task Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("📄 Typing Speed Test (Live Quote Version)");
            Console.WriteLine("------------------------------------------");

            string sentence = await GetQuoteAsync();
            Console.WriteLine("\nType the following sentence:\n");
            Console.WriteLine($"👉 {sentence}\n");

            Console.WriteLine("Start typing below:");

            Stopwatch stopwatch = new Stopwatch();
            string typed = "";
            int cursorTop = Console.CursorTop;
            stopwatch.Start();

            while (typed.Length < sentence.Length)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                // Allow only printable characters or backspace
                if (keyInfo.Key == ConsoleKey.Backspace && typed.Length > 0)
                {
                    typed = typed.Substring(0, typed.Length - 1);
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    typed += keyInfo.KeyChar;
                }

                // Overwrite same line
                Console.SetCursorPosition(0, cursorTop);
                Console.Write(new string(' ', Console.WindowWidth)); // Clear line
                Console.SetCursorPosition(0, cursorTop);
                Console.Write(typed);
            }

            stopwatch.Stop();

            double timeInSeconds = stopwatch.Elapsed.TotalSeconds;
            double timeInMinutes = timeInSeconds / 60;
            int wordCount = sentence.Split(' ').Length;
            double wpm = wordCount / timeInMinutes;

            // Accuracy calculation
            int correctChars = 0;
            for (int i = 0; i < sentence.Length; i++)
            {
                if (i < typed.Length && sentence[i] == typed[i])
                    correctChars++;
            }
            double accuracy = (double)correctChars / sentence.Length * 100;

            Console.WriteLine("\n\n✅ Typing complete!");
            Console.WriteLine($"⏱️ Time Taken: {timeInSeconds:F2} seconds");
            Console.WriteLine($"🧠 WPM: {wpm:F2}");
            Console.WriteLine($"🎯 Accuracy: {accuracy:F2}%");

            Console.WriteLine("\n🔁 Do you want to try again? (y/n):");
            string retry = Console.ReadLine().Trim().ToLower();
            if (retry != "y") break;
        }
    }

    static async Task<string> GetQuoteAsync()
    {
        // Bypass SSL (safe locally)
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        using (HttpClient client = new HttpClient(handler))
        {
            string url = "https://api.quotable.io/random";
            string response = await client.GetStringAsync(url);
            dynamic json = JsonConvert.DeserializeObject(response);
            return json.content;
        }
    }
}
