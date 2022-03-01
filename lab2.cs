using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

public static class lab2
{

    public static void task(string s)
    {
        long p = 41, q = 17;
        //long p = 7, q = 13;
        //long p = 797, q = 613;
        long n = p * q;

        long f = (q - 1) * (p - 1);

        //long e = 5;//not hard
        long e = getE(f);

        var open_key = (e, n);

        long k = 1;
        double d = (k * f + 1) / (double)e;
        for (int i = 2; i < int.MaxValue; ++i)
        {
            if (d % 1 == 0)
            {
                k = i - 1;
                break;
            }

            d = (i * f + 1) / (double)e;
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

    public static BigInteger[] encryptString(string s, IDictionary<char, byte> relation_table, (long e, long n) open_key)
    {
        BigInteger[] res = new BigInteger[s.Length];

        for (int i = 0; i < s.Length; ++i)
        {
            char c = s[i];
            byte c_code = relation_table[c];
            res[i] = BigInteger.Pow(c_code, (int)open_key.e) % open_key.n;
        }
        return res;
    }

    public static string decryptArray(BigInteger[] arr, IDictionary<char, byte> relation_table, (double d, long n) cloused_key)
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

    private static int getE(long f){
        Random rg = new();
        long num = f;
        List<int> dels = new();
        int del = 2;
        while(num != 1){//Can be more faster.
            if(num % del == 0){
                dels.Add(del);
                while(num % del == 0)
                    num /= del;
            }
            ++del;
        }
        
        int res = 0;

        long n = f;
        bool[] prime = Enumerable.Repeat(true, (int)(n + 1)).ToArray();
        prime[0] = prime[1] = false;
        for(int i = 2; i <= n; ++i)
            if(prime[i] && i > dels.Last()){
                res = i;
                break;
            }
            else
                if(prime[i] && (long)i * i <= n)
                    for(int j = i * i; j <=n; j+=i)
                        prime[j] = false;

        return res;

        // for(int i = 0; i < prime.Length; ++i)
        //     if(prime[i] && i > dels.Last()){
        //         res = i;
        //         break;
        //     }


    } 
}