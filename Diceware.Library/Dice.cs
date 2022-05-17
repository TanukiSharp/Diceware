using System.Security.Cryptography;

namespace Diceware.Library;

public static class Dice
{
    public static int RollD6()
    {
        return RandomNumberGenerator.GetInt32(1, 7);
    }

    public static int DrawWordNumber()
    {
        int number = 0;
        int multiplier = 10_000;

        for (int i = 0; i < 5; i++)
        {
            number += RollD6() * multiplier;
            multiplier /= 10;
        }

        return number;
    }
}
