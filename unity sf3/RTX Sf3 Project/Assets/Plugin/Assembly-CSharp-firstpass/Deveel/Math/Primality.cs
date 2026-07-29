using System;

namespace Deveel.Math
{
	internal static class Primality
	{
		private static readonly int[] primes;

		private static readonly BigInteger[] BIprimes;

		private static readonly int[] BITS;

		private static readonly int[][] offsetPrimes;

		static Primality()
		{
			primes = new int[172]
			{
				2, 3, 5, 7, 11, 13, 17, 19, 23, 29,
				31, 37, 41, 43, 47, 53, 59, 61, 67, 71,
				73, 79, 83, 89, 97, 101, 103, 107, 109, 113,
				127, 131, 137, 139, 149, 151, 157, 163, 167, 173,
				179, 181, 191, 193, 197, 199, 211, 223, 227, 229,
				233, 239, 241, 251, 257, 263, 269, 271, 277, 281,
				283, 293, 307, 311, 313, 317, 331, 337, 347, 349,
				353, 359, 367, 373, 379, 383, 389, 397, 401, 409,
				419, 421, 431, 433, 439, 443, 449, 457, 461, 463,
				467, 479, 487, 491, 499, 503, 509, 521, 523, 541,
				547, 557, 563, 569, 571, 577, 587, 593, 599, 601,
				607, 613, 617, 619, 631, 641, 643, 647, 653, 659,
				661, 673, 677, 683, 691, 701, 709, 719, 727, 733,
				739, 743, 751, 757, 761, 769, 773, 787, 797, 809,
				811, 821, 823, 827, 829, 839, 853, 857, 859, 863,
				877, 881, 883, 887, 907, 911, 919, 929, 937, 941,
				947, 953, 967, 971, 977, 983, 991, 997, 1009, 1013,
				1019, 1021
			};
			BIprimes = new BigInteger[primes.Length];
			BITS = new int[53]
			{
				0, 0, 1854, 1233, 927, 747, 627, 543, 480, 431,
				393, 361, 335, 314, 295, 279, 265, 253, 242, 232,
				223, 216, 181, 169, 158, 150, 145, 140, 136, 132,
				127, 123, 119, 114, 110, 105, 101, 96, 92, 87,
				83, 78, 73, 69, 64, 59, 54, 49, 44, 38,
				32, 26, 1
			};
			for (int i = 0; i < primes.Length; i++)
			{
				BIprimes[i] = BigInteger.ValueOf(primes[i]);
			}
			offsetPrimes = new int[11][];
			offsetPrimes[0] = null;
			offsetPrimes[1] = null;
			offsetPrimes[2] = new int[2] { 0, 2 };
			offsetPrimes[3] = new int[2] { 2, 2 };
			offsetPrimes[4] = new int[2] { 4, 2 };
			offsetPrimes[5] = new int[2] { 6, 5 };
			offsetPrimes[6] = new int[2] { 11, 7 };
			offsetPrimes[7] = new int[2] { 18, 13 };
			offsetPrimes[8] = new int[2] { 31, 23 };
			offsetPrimes[9] = new int[2] { 54, 43 };
			offsetPrimes[10] = new int[2] { 97, 75 };
		}

		public static BigInteger NextProbablePrime(BigInteger n)
		{
			int num = 1024;
			int[] array = new int[primes.Length];
			bool[] array2 = new bool[num];
			if (n.numberLength == 1 && n.Digits[0] >= 0 && n.Digits[0] < primes[primes.Length - 1])
			{
				int i;
				for (i = 0; n.Digits[0] >= primes[i]; i++)
				{
				}
				return BIprimes[i];
			}
			BigInteger bigInteger = new BigInteger(1, n.numberLength, new int[n.numberLength + 1]);
			Array.Copy(n.Digits, 0, bigInteger.Digits, 0, n.numberLength);
			if (n.TestBit(0))
			{
				Elementary.inplaceAdd(bigInteger, 2);
			}
			else
			{
				bigInteger.Digits[0] |= 1;
			}
			int bitLength = bigInteger.BitLength;
			int j;
			for (j = 2; bitLength < BITS[j]; j++)
			{
			}
			for (int i = 0; i < primes.Length; i++)
			{
				array[i] = Division.Remainder(bigInteger, primes[i]) - num;
			}
			while (true)
			{
				for (int k = 0; k < array2.Length; k++)
				{
					array2[k] = false;
				}
				for (int i = 0; i < primes.Length; i++)
				{
					array[i] = (array[i] + num) % primes[i];
					for (bitLength = ((array[i] != 0) ? (primes[i] - array[i]) : 0); bitLength < num; bitLength += primes[i])
					{
						array2[bitLength] = true;
					}
				}
				for (bitLength = 0; bitLength < num; bitLength++)
				{
					if (!array2[bitLength])
					{
						BigInteger bigInteger2 = bigInteger.Copy();
						Elementary.inplaceAdd(bigInteger2, bitLength);
						if (MillerRabin(bigInteger2, j))
						{
							return bigInteger2;
						}
					}
				}
				Elementary.inplaceAdd(bigInteger, num);
			}
		}

		public static BigInteger ConsBigInteger(int bitLength, int certainty, Random rnd)
		{
			if (bitLength <= 10)
			{
				int[] array = offsetPrimes[bitLength];
				return BIprimes[array[0] + rnd.Next(array[1])];
			}
			int bits = -bitLength & 0x1F;
			int num = bitLength + 31 >> 5;
			BigInteger bigInteger = new BigInteger(1, num, new int[num]);
			num--;
			do
			{
				for (int i = 0; i < bigInteger.numberLength; i++)
				{
					bigInteger.Digits[i] = rnd.Next();
				}
				bigInteger.Digits[num] |= int.MinValue;
				bigInteger.Digits[num] = Utils.URShift(bigInteger.Digits[num], bits);
				bigInteger.Digits[0] |= 1;
			}
			while (!IsProbablePrime(bigInteger, certainty));
			return bigInteger;
		}

		public static bool IsProbablePrime(BigInteger n, int certainty)
		{
			if (certainty <= 0 || (n.numberLength == 1 && n.Digits[0] == 2))
			{
				return true;
			}
			if (!n.TestBit(0))
			{
				return false;
			}
			if (n.numberLength == 1 && (n.Digits[0] & 0xFFFFFC00u) == 0)
			{
				return Array.BinarySearch(primes, n.Digits[0]) >= 0;
			}
			for (int i = 1; i < primes.Length; i++)
			{
				if (Division.RemainderArrayByInt(n.Digits, n.numberLength, primes[i]) == 0)
				{
					return false;
				}
			}
			int bitLength = n.BitLength;
			int j;
			for (j = 2; bitLength < BITS[j]; j++)
			{
			}
			certainty = System.Math.Min(j, 1 + (certainty - 1 >> 1));
			return MillerRabin(n, certainty);
		}

		private static bool MillerRabin(BigInteger n, int t)
		{
			BigInteger bigInteger = n.Subtract(BigInteger.One);
			int bitLength = bigInteger.BitLength;
			int lowestSetBit = bigInteger.LowestSetBit;
			BigInteger exponent = bigInteger.ShiftRight(lowestSetBit);
			Random rnd = new Random();
			for (int i = 0; i < t; i++)
			{
				BigInteger bigInteger2;
				if (i < primes.Length)
				{
					bigInteger2 = BIprimes[i];
				}
				else
				{
					do
					{
						bigInteger2 = new BigInteger(bitLength, rnd);
					}
					while (bigInteger2.CompareTo(n) >= BigInteger.EQUALS || bigInteger2.Sign == 0 || bigInteger2.IsOne);
				}
				BigInteger bigInteger3 = bigInteger2.ModPow(exponent, n);
				if (bigInteger3.IsOne || bigInteger3.Equals(bigInteger))
				{
					continue;
				}
				for (int j = 1; j < lowestSetBit; j++)
				{
					if (!bigInteger3.Equals(bigInteger))
					{
						bigInteger3 = bigInteger3.Multiply(bigInteger3).Mod(n);
						if (bigInteger3.IsOne)
						{
							return false;
						}
					}
				}
				if (!bigInteger3.Equals(bigInteger))
				{
					return false;
				}
			}
			return true;
		}
	}
}
