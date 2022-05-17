using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Diceware.Library;

public class WordList
{
    private readonly HttpClient httpClient;

    private readonly string downloadUrl;
    private readonly IStorage storage;

    private Dictionary<int, string> wordList;

    private const string StartMarker = "-----BEGIN PGP SIGNED MESSAGE-----";
    private const string EndMarker = "-----BEGIN PGP SIGNATURE-----";

    public const string OfficialWordListDownloadUrl = "https://theworld.com/~reinhold/diceware.wordlist.asc";

    public static readonly char[,] ExtraSecurityMatrix =
    {
        { '~', '!', '#', '$', '%', '^' },
        { '&', '*', '(', ')', '-', '=' },
        { '+', '[', ']', '\\', '{', '}' },
        { ':', ';', '"', '\'', '<', '>' },
        { '?', '/', '0', '1', '2', '3' },
        { '4', '5', '6', '7', '8', '9' },
    };

    public WordList(HttpClient httpClient, string downloadUrl, IStorage storage)
    {
        this.httpClient = httpClient;

        this.downloadUrl = downloadUrl;
        this.storage = storage;
    }

    private static Dictionary<int, string> LoadFromContent(string content)
    {
        bool inData = false;
        int lineNumber = 0;

        var result = new Dictionary<int, string>();

        foreach (string rawLine in content.Split('\n'))
        {
            lineNumber++;

            string line = rawLine.TrimEnd();
            if (line.Length == 0)
                continue;

            if (inData == false)
            {
                if (line == StartMarker)
                    inData = true;
                continue;
            }
            else if (line == EndMarker)
                break;

            string[] parts = line.Split('\t');
            if (parts.Length != 2)
                throw new FormatException($"Invalid data at line {lineNumber}");

            if (int.TryParse(parts[0], out int number) == false)
                throw new FormatException($"Invalid number value at line {lineNumber} ({parts[0]})");

            result.Add(number, parts[1]);
        }

        return result;
    }

    private async Task<string> DownloadWordListAsync()
    {
        return await httpClient.GetStringAsync(downloadUrl);
    }

    public async Task<Dictionary<int, string>> GetWordList(bool forceDownload)
    {
        if (wordList == null)
        {
            string content;

            if (forceDownload || storage.IsContentAvailable == false)
            {
                Console.WriteLine($"Downloading '{downloadUrl}'...");
                content = await DownloadWordListAsync();
                Console.WriteLine($"Writing to cache...");
                await storage.SaveContent(content);
                Console.WriteLine("Done.");
            }
            else
                content = await storage.LoadContent();

            wordList = LoadFromContent(content);
        }

        return wordList;
    }
}
