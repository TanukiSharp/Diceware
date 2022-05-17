using System.Collections.Generic;

namespace Diceware.Library;

public static class Passphrase
{
    public static string Generate(int wordCount, Dictionary<int, string> words, bool extraSecurity)
    {
        string[] passphrase = new string[wordCount];

        for (int i = 0; i < wordCount; i++)
        {
            int number = Dice.DrawWordNumber();
            passphrase[i] = words[number];
        }

        if (extraSecurity)
        {
            int wordIndex = Dice.RollD6() - 1;
            string chosenWord = passphrase[wordIndex % passphrase.Length];
            int position = (Dice.RollD6() - 1) % chosenWord.Length;

            char extraSecuritySymbol = WordList.ExtraSecurityMatrix[
                Dice.RollD6() - 1,
                Dice.RollD6() - 1
            ];

            chosenWord = $"{chosenWord[..position]}{extraSecuritySymbol}{chosenWord[position..]}";

            passphrase[wordIndex % passphrase.Length] = chosenWord;
        }

        return string.Join(' ', passphrase);
    }
}
