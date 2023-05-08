using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace TranslatorGame.Data
{
    public static class Shuffle
    {
        public static void ShuffleMe<T>(this IList<T> list)
        {
            RandomNumberGenerator provider = RandomNumberGenerator.Create();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (byte.MaxValue / n)));
                int k = box[0] % n;
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
