using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

static class lab1
{

    public static void task1() => System.Console.WriteLine(new FileInfo(@"..\word.docx").Length);

    public static void task2()
    {
        const string file_path = @"d:\Documents\Labs_prog\6th sem\защита информации\work\lab1\word.docx";

        Dictionary<byte, double> dct = new();

        foreach (byte b in File.ReadAllBytes(file_path))
        {
            if (dct.ContainsKey(b))
                ++dct[b];
            else
                dct.Add(b, 1);
        }

        foreach (var k in dct.Keys)
        {
            dct[k] /= File.ReadAllBytes(file_path).Length;
        }

        System.Console.WriteLine(dct.Values.Sum());

        foreach (var pair in dct)
        {
            System.Console.WriteLine($"byte_{pair.Key} : {pair.Value} times.");
        }
    }



    public static void task3()
    {
        const string file_path = @"d:\Documents\Labs_prog\6th sem\защита информации\work\lab1\word.docx";

        const int key = 5 % 256;

        var rg = new Random(key);

        var shaffled_bytes = Enumerable.Range(byte.MinValue, byte.MaxValue + 1).OrderBy(i => rg.Next()).Select(i => (byte)i);

        var relation_table = new Dictionary<byte, byte>();

        byte cnt = 0;
        foreach (var b in shaffled_bytes)
        {
            relation_table.Add(cnt, b);
            //System.Console.WriteLine($"{cnt} -> {b}");
            ++cnt;
        }


        System.Console.WriteLine("e - encrypt, d - decrypt");
        string q = Console.ReadLine();
        if (q == "e")
            encrypt(file_path, relation_table);
        else if (q == "d")
            decrypt(file_path, relation_table);
        else
            Console.WriteLine("Wrong input.");
    }

    public static void encrypt(string file_path, IDictionary<byte, byte> relation_table)
    {
        var bytes = File.ReadAllBytes(file_path);
        for (int i = 0; i < bytes.Length; ++i)
        {
            bytes[i] = relation_table[bytes[i]];
        }
        File.WriteAllBytes(file_path, bytes);
    }

    public static void decrypt(string file_path, IDictionary<byte, byte> relation_table)
    {
        var bytes = File.ReadAllBytes(file_path);
        for (int i = 0; i < bytes.Length; ++i)
        {
            bytes[i] = relation_table.First(p => p.Value == bytes[i]).Key;
        }
        File.WriteAllBytes(file_path, bytes);
    }

}