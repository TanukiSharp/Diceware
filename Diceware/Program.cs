using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Reflection;
using System.Threading.Tasks;

namespace Diceware
{
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

            var rootCommand = new RootCommand
            {
                Description = "Application that randomly draws words folling the Diceware(TM) rules."
            };
            rootCommand.AddOption(optionWordCount);
            rootCommand.AddOption(optionExtraSecurity);
            rootCommand.AddOption(optionDownloadUrl);

            rootCommand.Handler = CommandHandler.Create<int, bool, string>(Run);

            return rootCommand.InvokeAsync(args);
        }

        private static async Task Run(int wordCount, bool extraSecurity, string downloadUrl)
        {
            var wordList = new WordList(downloadUrl);
            var dice = new Dice();

            Dictionary<int, string> words = await wordList.GetWordList();

            string[] passphrase = new string[wordCount];

            for (int i = 0; i < wordCount; i++)
            {
                int number = dice.DrawWordNumber();
                passphrase[i] = words[number];
            }

            if (extraSecurity)
            {
                int wordIndex = dice.RollD6() - 1;
                string chosenWord = passphrase[wordIndex % passphrase.Length];
                int position = (dice.RollD6() - 1) % chosenWord.Length;

                char extraSecuritySymbol = WordList.ExtraSecurityMatrix[
                    dice.RollD6() - 1,
                    dice.RollD6() - 1
                ];

                chosenWord = $"{chosenWord[..position]}{extraSecuritySymbol}{chosenWord[position..]}";

                passphrase[wordIndex % passphrase.Length] = chosenWord;
            }

            Console.WriteLine(string.Join(' ', passphrase));
        }
    }
}
