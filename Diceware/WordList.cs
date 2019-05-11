﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Diceware
{
    public class WordList
    {
        private readonly string downloadUrl;
        private readonly string localFilename;

        private Dictionary<int, string> wordList;

        private const string StartMarker = "-----BEGIN PGP SIGNED MESSAGE-----";
        private const string EndMarker = "-----BEGIN PGP SIGNATURE-----";

        public const string OfficialWordListDownloadUrl = "http://world.std.com/~reinhold/diceware.wordlist.asc";

        public static readonly char[,] ExtraSecurityMatrix =
        {
            { '~', '!', '#', '$', '%', '^' },
            { '&', '*', '(', ')', '-', '=' },
            { '+', '[', ']', '\\', '{', '}' },
            { ':', ';', '"', '\'', '<', '>' },
            { '?', '/', '0', '1', '2', '3' },
            { '4', '5', '6', '7', '8', '9' },
        };

        public WordList(string downloadUrl)
            : this(downloadUrl, Path.GetFileName(new Uri(downloadUrl).LocalPath))
        {
        }

        public WordList(string downloadUrl, string localFilename)
        {
            this.downloadUrl = downloadUrl;
            this.localFilename = localFilename;
        }

        private Dictionary<int, string> LoadFromContent(string content)
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
            using var httpClient = new HttpClient();
            return await httpClient.GetStringAsync(downloadUrl);
        }

        public async Task<Dictionary<int, string>> GetWordList()
        {
            if (wordList == null)
            {
                string wordListFilename = Path.Combine(AppContext.BaseDirectory, localFilename);

                string content;

                if (File.Exists(wordListFilename) == false)
                {
                    content = await DownloadWordListAsync();
                    await File.WriteAllTextAsync(wordListFilename, content);
                }
                else
                    content = await File.ReadAllTextAsync(wordListFilename);

                wordList = LoadFromContent(content);
            }

            return wordList;
        }
    }
}
