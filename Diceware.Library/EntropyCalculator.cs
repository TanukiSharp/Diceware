using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Diceware.Library;

internal class CharacterPool
{
    public int PoolSize { get; private set; }
    public Func<char, bool> Matcher { get; }

    private readonly HashSet<char> pool = new();

    public CharacterPool(Func<char, bool> matcher)
    {
        Matcher = matcher;
    }

    public void Update(char character)
    {
        if (Matcher(character))
        {
            pool.Add(character);
        }
    }

    public void Update(string word)
    {
        foreach (char character in word)
        {
            Update(character);
        }
    }

    public void UpdateSize()
    {
        PoolSize = pool.Count;
    }

    public void Reset()
    {
        pool.Clear();
    }
}

public class EntropyCalculator
{
    private static readonly Func<char, bool> digitsMatcher = c => c >= '0' && c <= '9';
    private static readonly Func<char, bool> lowerCaseLatinLettersMatcher = c => c >= 'a' && c <= 'z';
    private static readonly Func<char, bool> upperCaseLatinLettersMatcher = c => c >= 'A' && c <= 'Z';
    private static readonly Func<char, bool> othersMatcher = c => digitsMatcher(c) == false && lowerCaseLatinLettersMatcher(c) == false && upperCaseLatinLettersMatcher(c) == false;

    private readonly CharacterPool[] characterPools;
    private readonly IReadOnlyList<int> basePoolSize;
    private readonly IReadOnlyList<int> extraSecurityPoolSize;

    public EntropyCalculator(Dictionary<int, string> words)
    {
        characterPools = CreatePools();

        basePoolSize = ComputeBasePoolSize(words);
        extraSecurityPoolSize = ComputeExtraSecurityPoolSize();

        ResetPools();
    }

    public double Compute(string passphrase, bool extraSecurity)
    {
        bool[] active = new bool[characterPools.Length];

        foreach (char character in passphrase)
        {
            for (int poolIndex = 0; poolIndex < characterPools.Length; poolIndex++)
            {
                if (characterPools[poolIndex].Matcher(character))
                {
                    active[poolIndex] = true;
                }
            }
        }

        int totalPoolSize = 0;

        for (int poolIndex = 0; poolIndex < characterPools.Length; poolIndex++)
        {
            if (active[poolIndex])
            {
                IReadOnlyList<int> poolSizeValues = extraSecurity ? extraSecurityPoolSize : basePoolSize;
                totalPoolSize += poolSizeValues[poolIndex];
            }
        }

        return passphrase.Length * Math.Log2(totalPoolSize);
    }

    private static CharacterPool[] CreatePools()
    {
        return new CharacterPool[]
        {
            new CharacterPool(digitsMatcher),
            new CharacterPool(lowerCaseLatinLettersMatcher),
            new CharacterPool(upperCaseLatinLettersMatcher),
            new CharacterPool(othersMatcher),
        };
    }

    private IReadOnlyList<int> ComputeBasePoolSize(Dictionary<int, string> words)
    {
        foreach (var kv in words)
        {
            string word = kv.Value;

            foreach (CharacterPool pool in characterPools)
            {
                pool.Update(word);
            }
        }

        foreach (CharacterPool pool in characterPools)
        {
            pool.UpdateSize();
        }

        return new ReadOnlyCollection<int>(characterPools.Select(x => x.PoolSize).ToList());
    }

    private IReadOnlyList<int> ComputeExtraSecurityPoolSize()
    {
        foreach (char character in WordList.ExtraSecurityMatrix)
        {
            foreach (CharacterPool pool in characterPools)
            {
                pool.Update(character);
            }
        }

        foreach (CharacterPool pool in characterPools)
        {
            pool.UpdateSize();
        }

        return new ReadOnlyCollection<int>(characterPools.Select(x => x.PoolSize).ToList());
    }

    private void ResetPools()
    {
        foreach (CharacterPool pool in characterPools)
        {
            pool.Reset();
        }
    }

    public enum StrengthLevel
    {
        ExtremelyStrong,
        VeryStrong,
        Strong,
        Reasonable,
        Weak,
        ExtremelyWeak
    }

    public static StrengthLevel EntropyToStrengthLevel(double entropy)
    {
        // Taken from https://www.omnicalculator.com/other/password-entropy

        if (entropy >= 128.0)
            return StrengthLevel.ExtremelyStrong;

        if (entropy >= 60.0)
            return StrengthLevel.VeryStrong;

        if (entropy >= 36.0)
            return StrengthLevel.Strong;

        if (entropy >= 28.0)
            return StrengthLevel.Reasonable;

        if (entropy >= 18.0)
            return StrengthLevel.Weak;

        return StrengthLevel.ExtremelyWeak;
    }

    public static string StrengthLevelToEnglishText(StrengthLevel strengthLevel)
    {
        switch (strengthLevel)
        {
            case StrengthLevel.ExtremelyStrong: return "extremely strong";
            case StrengthLevel.VeryStrong: return "very strong";
            case StrengthLevel.Strong: return "strong";
            case StrengthLevel.Reasonable: return "reasonable";
            case StrengthLevel.Weak: return "weak";
            case StrengthLevel.ExtremelyWeak: return "extremely weak.";
        }

        return $"<{strengthLevel}>";
    }
}
