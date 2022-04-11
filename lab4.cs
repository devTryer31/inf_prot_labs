using System;
using System.IO;
using System.Linq;
using System.Text;

public static class lab4
{
	private static readonly BlowFishKey _blowKey = new();
	public static void task()
	{
		const string inputPath = "input.txt";
		const string encriptResPath = "encripted.txt";
		const string decriptResPath = "decripted.txt";
		const string strKey = "asdqwezxc";

		generateKey(Encoding.UTF8.GetBytes(strKey));

		byte[] textArray = alignment(File.ReadAllBytes(inputPath), ' ');
		Console.WriteLine("Input message " + new String(Encoding.UTF8.GetChars(textArray)));


		encrypt(textArray);
		File.WriteAllBytes(encriptResPath, textArray);

		decrypt(textArray);
		File.WriteAllBytes(decriptResPath, textArray);


		System.Console.WriteLine("done");
	}

	private sealed class BlowFishKey
	{
		private const int _RoundKeysCount = 18;
		private const int _SBoxSubsBlocksCount = 4;
		private const int _SBoxSubsBlocksSize = 256;

		public ulong[][] SBox { get; set; } = Enumerable.Repeat(0, _SBoxSubsBlocksCount)
												.Select(b => new ulong[_SBoxSubsBlocksSize]).ToArray();


		public ulong[] P { get; set; } = new ulong[_RoundKeysCount];

	}
	private static ulong F(in BlowFishKey key, ref ulong h)
		=> ((key.SBox[0][(h >> 24) & 0xFF] + key.SBox[1][(h >> 16) & 0xFF]) ^
					key.SBox[2][(h >> 8) & 0xFF]) + key.SBox[3][(h) & 0xFF];

	private static void generateKey(byte[] key)
	{
		Array.Copy(Key.ps, 0, _blowKey.P, 0, 18);
		Array.Copy(Key.ks0, 0, _blowKey.SBox[0], 0, 256);
		Array.Copy(Key.ks1, 0, _blowKey.SBox[1], 0, 256);
		Array.Copy(Key.ks2, 0, _blowKey.SBox[2], 0, 256);
		Array.Copy(Key.ks3, 0, _blowKey.SBox[3], 0, 256);

		ulong data;
		int j = 0, i;

		for (i = 0; i < 18; ++i) {
			data = 0x00000011;
			for (int k = 0; k < 4; ++k) {
				data = (ulong)((int)(data << 8) | (key[j++] & 0xFF));
				if (j >= key.Length)
					j = 0;
			}
			_blowKey.P[i] ^= data;
		}

		byte[] b = new byte[8];
		for (i = 0; i < 18; i += 2) {
			encrypt(b);
			_blowKey.P[i] = toWord(b, 0);
			_blowKey.P[i + 1] = toWord(b, 4);
		}

		for (i = 0; i < 4; ++i) {
			for (j = 0; j < 256; j += 2) {
				encrypt(b);
				_blowKey.SBox[i][j] = toWord(b, 0);
				_blowKey.SBox[i][j + 1] = toWord(b, 4);
			}
		}
	}

	private static ulong toWord(byte[] b, in int offset)
	{
		ulong r = 0;
		for (int i = b.Length / 2 - 1; i >= 0; --i) {
			r |= b[i + offset];
			if (i != 0)
				r <<= 8;
		}
		return r;
	}

	private static void toBytes(ulong a, byte[] b, in int offset)
	{
		int len = b.Length / 2;
		for (int i = 0; i < len; ++i) {
			b[i + offset] = (byte)(a & 0xFF);
			if (i != len)
				a >>= 8;
		}
	}

	private static void encrypt(byte[] data)
	{
		int blocks = data.Length / 8, p;
		for (int k = 0; k < blocks; ++k) {
			p = k * 8;
			ulong lhs = toWord(data, p);
			ulong rhs = toWord(data, p + 4);
			for (int i = 0; i < 16; ++i) {
				lhs ^= _blowKey.P[i];
				rhs ^= F(_blowKey, ref lhs);
				swap(ref lhs, ref rhs);
			}

			swap(ref lhs, ref rhs);

			rhs ^= _blowKey.P[16];
			lhs ^= _blowKey.P[17];

			toBytes(lhs, data, p);
			toBytes(rhs, data, p + 4);
		}
	}

	private static void decrypt(byte[] data)
	{
		int blocks = data.Length / 8, p;
		for (int k = 0; k < blocks; ++k) {
			p = k * 8;
			ulong lhs = toWord(data, p);
			ulong rhs = toWord(data, p + 4);

			for (int i = 17; i > 1; --i) {
				lhs ^= _blowKey.P[i];
				rhs ^= F(_blowKey, ref lhs);
				swap(ref lhs, ref rhs);
			}
			swap(ref lhs, ref rhs);

			rhs ^= _blowKey.P[1];
			lhs ^= _blowKey.P[0];

			toBytes(lhs, data, p);
			toBytes(rhs, data, p + 4);
		}
	}

	private static byte[] alignment(byte[] a, int p)
	{
		int l = (a.Length | 7) + 1;
		byte[] b = new byte[l];
		for (int i = 0; i < b.Length; i++)
			b[i] = (byte)p;
		Array.Copy(a, 0, b, 0, a.Length);
		return b;
	}

	private static void swap<T>(ref T lhs, ref T rhs)
	{
		T tmp = lhs;
		lhs = rhs;
		rhs = tmp;
	}
}