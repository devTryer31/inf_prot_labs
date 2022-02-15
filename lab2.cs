using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

public static class lab2
{

    public static void task(string s)
    {
        long p = 7, q = 13;
        long n = p * q;

        long f = (q - 1) * (p - 1);

        long e = 5;

        var open_key = (e, n);

        long k = 1;
        double d = (k * f + 1) / 5d;
        for (int i = 2; i < int.MaxValue; ++i)
        {
            if (d % 1 == 0)
            {
                k = i - 1;
                break;
            }

            d = (i * f + 1) / 5d;
        }

        var cloused_key = (d, n);

        Dictionary<char, byte> relation_table = new();

        int step_back = 0;
        for (int i = 0; i < 33; ++i)
        {
            char c = (char)((int)'А' + i - step_back);

            relation_table.Add(c, (byte)(i + 1));

            if (c == 'Е')
            {
                ++i;
                step_back = 1;
                relation_table.Add('Ё', (byte)(i + 1));
            }
        }
        relation_table.Add(' ', 34);
        for (int i = 0; i <= 9; ++i)
            relation_table.Add((char)(48 + i), (byte)(35 + i));



        var inc_arr = encryptString(s, relation_table, open_key);

        System.Console.WriteLine("encrypted: " + string.Join(' ', inc_arr));

        System.Console.WriteLine("decrypted: " + decryptArray(inc_arr, relation_table, cloused_key));

    }

    public static int[] encryptString(string s, IDictionary<char, byte> relation_table, (long e, long n) open_key)
    {
        int[] res = new int[s.Length];

        for (int i = 0; i < s.Length; ++i)
        {
            char c = s[i];
            byte c_code = relation_table[c];
            res[i] = (int)(Math.Pow(c_code, open_key.e) % open_key.n);
        }
        return res;
    }

    public static string decryptArray(int[] arr, IDictionary<char, byte> relation_table, (double d, long n) cloused_key)
    {
        StringBuilder sb = new(arr.Length);
        foreach (int ci in arr)
        {
            var bi = BigInteger.Pow(new BigInteger(ci), (int)cloused_key.d);
            var tmp = bi % cloused_key.n;
            sb.Append(relation_table.First(p => p.Value == tmp).Key);
        }
        return sb.ToString();
    }
}