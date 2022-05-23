using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Diceware.Library;

namespace Diceware.Console;

class Program
{
    static Task<int> Main(string[] args)
    {
        var optionWordCount = new Option(
            new[] { "-c", "--word-count" },
            "The number of words that compose the passphrase.",
            new Argument<int>(6)
        );

        var optionExtraSecurity = new Option(
            new[] { "-s", "--extra-security" },
            "Adds an extra symbol in the passphrase.",
            new Argument<bool>(false)
        );

        var optionDownloadUrl = new Option(
            new[] { "-u", "--download-url" },
            "An URL where to download the words list.",
            new Argument<string>(WordList.OfficialWordListDownloadUrl)
        );

        var optionForceDownload = new Option(
            new[] { "-f", "--force-download" },
            "Force download of the words list even if cache exists. This overwrites the currently cached file if exists.",
            new Argument<bool>(false)
        );

        var rootCommand = new RootCommand
        {
            Description = "Application that randomly draws words folling the Diceware(TM) rules."
        };
        rootCommand.AddOption(optionWordCount);
        rootCommand.AddOption(optionExtraSecurity);
        rootCommand.AddOption(optionDownloadUrl);
        rootCommand.AddOption(optionForceDownload);

        rootCommand.Handler = CommandHandler.Create<int, bool, string, bool>(Run);

        return rootCommand.InvokeAsync(args);
    }

    private static async Task Run(int wordCount, bool extraSecurity, string downloadUrl, bool forceDownload)
    {
        using var httpClient = new HttpClient();

        var wordList = new WordList(httpClient, downloadUrl, new DiskStorage(Path.GetFileName(new Uri(downloadUrl).LocalPath)));

        Dictionary<int, string> words = await wordList.GetWordList(forceDownload);
        EntropyCalculator entropyCalculator = new(words);

        string passphrase = Passphrase.Generate(wordCount, words, extraSecurity);

        double entropy = Math.Round(entropyCalculator.Compute(passphrase, extraSecurity), 1);
        string strength = EntropyCalculator.StrengthLevelToEnglishText(EntropyCalculator.EntropyToStrengthLevel(entropy));

        System.Console.WriteLine($"Passphrase: {passphrase}");
        System.Console.WriteLine($"Entropy:    {entropy} bits ({strength})");
    }
}
