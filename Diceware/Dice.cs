using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using freakcode.Cryptography;

namespace Diceware
{
    public class Dice
    {
        private readonly CryptoRandom random = new CryptoRandom();

        public int RollD6()
        {
            return random.Next(1, 7);
        }

        public int DrawWordNumber()
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
}
