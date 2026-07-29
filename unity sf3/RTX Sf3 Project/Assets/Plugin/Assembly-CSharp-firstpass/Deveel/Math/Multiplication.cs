using System;

namespace Deveel.Math
{
	internal static class Multiplication
	{
		private const int WhenUseKaratsuba = 63;

		private static readonly int[] TenPows;

		private static readonly int[] FivePows;

		public static readonly BigInteger[] BigTenPows;

		public static readonly BigInteger[] BigFivePows;

		static Multiplication()
		{
			TenPows = new int[10] { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000 };
			FivePows = new int[14]
			{
				1, 5, 25, 125, 625, 3125, 15625, 78125, 390625, 1953125,
				9765625, 48828125, 244140625, 1220703125
			};
			BigTenPows = new BigInteger[32];
			BigFivePows = new BigInteger[32];
			long num = 1L;
			int i;
			for (i = 0; i <= 18; i++)
			{
				BigFivePows[i] = BigInteger.ValueOf(num);
				BigTenPows[i] = BigInteger.ValueOf(num << i);
				num *= 5;
			}
			for (; i < BigTenPows.Length; i++)
			{
				BigFivePows[i] = BigFivePows[i - 1].Multiply(BigFivePows[1]);
				BigTenPows[i] = BigTenPows[i - 1].Multiply(BigInteger.Ten);
			}
		}

		public static BigInteger Multiply(BigInteger x, BigInteger y)
		{
			return Karatsuba(x, y);
		}

		private static BigInteger Karatsuba(BigInteger op1, BigInteger op2)
		{
			if (op2.numberLength > op1.numberLength)
			{
				BigInteger bigInteger = op1;
				op1 = op2;
				op2 = bigInteger;
			}
			if (op2.numberLength < 63)
			{
				return MultiplyPap(op1, op2);
			}
			int num = (int)(op1.numberLength & 0xFFFFFFFEu) << 4;
			BigInteger bigInteger2 = op1.ShiftRight(num);
			BigInteger bigInteger3 = op2.ShiftRight(num);
			BigInteger bigInteger4 = op1.Subtract(bigInteger2.ShiftLeft(num));
			BigInteger bigInteger5 = op2.Subtract(bigInteger3.ShiftLeft(num));
			BigInteger bigInteger6 = Karatsuba(bigInteger2, bigInteger3);
			BigInteger val = Karatsuba(bigInteger4, bigInteger5);
			BigInteger bigInteger7 = Karatsuba(bigInteger2.Subtract(bigInteger4), bigInteger5.Subtract(bigInteger3));
			bigInteger7 = bigInteger7.Add(bigInteger6).Add(val);
			bigInteger7 = bigInteger7.ShiftLeft(num);
			bigInteger6 = bigInteger6.ShiftLeft(num << 1);
			return bigInteger6.Add(bigInteger7).Add(val);
		}

		private static BigInteger MultiplyPap(BigInteger a, BigInteger b)
		{
			int numberLength = a.numberLength;
			int numberLength2 = b.numberLength;
			int num = numberLength + numberLength2;
			int sign = ((a.Sign == b.Sign) ? 1 : (-1));
			if (num == 2)
			{
				long num2 = UnsignedMultAddAdd(a.Digits[0], b.Digits[0], 0, 0);
				int num3 = (int)num2;
				int num4 = (int)Utils.URShift(num2, 32);
				return (num4 != 0) ? new BigInteger(sign, 2, new int[2] { num3, num4 }) : new BigInteger(sign, num3);
			}
			int[] digits = a.Digits;
			int[] digits2 = b.Digits;
			int[] array = new int[num];
			MultArraysPap(digits, numberLength, digits2, numberLength2, array);
			BigInteger bigInteger = new BigInteger(sign, num, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		public static void MultArraysPap(int[] aDigits, int aLen, int[] bDigits, int bLen, int[] resDigits)
		{
			if (aLen != 0 && bLen != 0)
			{
				if (aLen == 1)
				{
					resDigits[bLen] = MultiplyByInt(resDigits, bDigits, bLen, aDigits[0]);
				}
				else if (bLen == 1)
				{
					resDigits[aLen] = MultiplyByInt(resDigits, aDigits, aLen, bDigits[0]);
				}
				else
				{
					MultPap(aDigits, bDigits, resDigits, aLen, bLen);
				}
			}
		}

		private static void MultPap(int[] a, int[] b, int[] t, int aLen, int bLen)
		{
			if (a == b && aLen == bLen)
			{
				Square(a, aLen, t);
				return;
			}
			for (int i = 0; i < aLen; i++)
			{
				long num = 0L;
				int a2 = a[i];
				for (int j = 0; j < bLen; j++)
				{
					num = UnsignedMultAddAdd(a2, b[j], t[i + j], (int)num);
					t[i + j] = (int)num;
					num = Utils.URShift(num, 32);
				}
				t[i + bLen] = (int)num;
			}
		}

		private static int MultiplyByInt(int[] res, int[] a, int aSize, int factor)
		{
			long num = 0L;
			for (int i = 0; i < aSize; i++)
			{
				num = UnsignedMultAddAdd(a[i], factor, (int)num, 0);
				res[i] = (int)num;
				num = Utils.URShift(num, 32);
			}
			return (int)num;
		}

		public static int MultiplyByInt(int[] a, int aSize, int factor)
		{
			return MultiplyByInt(a, a, aSize, factor);
		}

		public static BigInteger MultiplyByPositiveInt(BigInteger val, int factor)
		{
			int sign = val.Sign;
			if (sign == 0)
			{
				return BigInteger.Zero;
			}
			int numberLength = val.numberLength;
			int[] digits = val.Digits;
			if (numberLength == 1)
			{
				long num = UnsignedMultAddAdd(digits[0], factor, 0, 0);
				int num2 = (int)num;
				int num3 = (int)Utils.URShift(num, 32);
				return (num3 != 0) ? new BigInteger(sign, 2, new int[2] { num2, num3 }) : new BigInteger(sign, num2);
			}
			int num4 = numberLength + 1;
			int[] array = new int[num4];
			array[numberLength] = MultiplyByInt(array, digits, numberLength, factor);
			BigInteger bigInteger = new BigInteger(sign, num4, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		public static BigInteger Pow(BigInteger b, int exponent)
		{
			BigInteger bigInteger = BigInteger.One;
			BigInteger bigInteger2 = b;
			while (exponent > 1)
			{
				if (((uint)exponent & (true ? 1u : 0u)) != 0)
				{
					bigInteger = bigInteger.Multiply(bigInteger2);
				}
				bigInteger2 = ((bigInteger2.numberLength != 1) ? new BigInteger(1, Square(bigInteger2.Digits, bigInteger2.numberLength, new int[bigInteger2.numberLength << 1])) : bigInteger2.Multiply(bigInteger2));
				exponent >>= 1;
			}
			return bigInteger.Multiply(bigInteger2);
		}

		private static int[] Square(int[] a, int aLen, int[] res)
		{
			long num;
			for (int i = 0; i < aLen; i++)
			{
				num = 0L;
				for (int j = i + 1; j < aLen; j++)
				{
					num = UnsignedMultAddAdd(a[i], a[j], res[i + j], (int)num);
					res[i + j] = (int)num;
					num = Utils.URShift(num, 32);
				}
				res[i + aLen] = (int)num;
			}
			BitLevel.ShiftLeftOneBit(res, res, aLen << 1);
			num = 0L;
			int num2 = 0;
			int num3 = 0;
			while (num2 < aLen)
			{
				num = UnsignedMultAddAdd(a[num2], a[num2], res[num3], (int)num);
				res[num3] = (int)num;
				num = Utils.URShift(num, 32);
				num3++;
				num += res[num3] & 0xFFFFFFFFu;
				res[num3] = (int)num;
				num = Utils.URShift(num, 32);
				num2++;
				num3++;
			}
			return res;
		}

		public static BigInteger MultiplyByTenPow(BigInteger val, long exp)
		{
			return (exp >= TenPows.Length) ? val.Multiply(PowerOf10(exp)) : MultiplyByPositiveInt(val, TenPows[(int)exp]);
		}

		public static BigInteger PowerOf10(long exp)
		{
			int num = (int)exp;
			if (exp < BigTenPows.Length)
			{
				return BigTenPows[num];
			}
			if (exp <= 50)
			{
				return BigInteger.Ten.Pow(num);
			}
			if (exp <= 1000)
			{
				return BigFivePows[1].Pow(num).ShiftLeft(num);
			}
			double num2 = 1.0 + (double)exp / 2.4082399653118496;
			if (num2 > 131072.0)
			{
				throw new ArithmeticException("power of ten too big");
			}
			if (exp <= int.MaxValue)
			{
				return BigFivePows[1].Pow(num).ShiftLeft(num);
			}
			BigInteger bigInteger = BigFivePows[1].Pow(int.MaxValue);
			BigInteger bigInteger2 = bigInteger;
			long num3 = exp - int.MaxValue;
			num = (int)(exp % int.MaxValue);
			while (num3 > int.MaxValue)
			{
				bigInteger2 = bigInteger2.Multiply(bigInteger);
				num3 -= int.MaxValue;
			}
			bigInteger2 = bigInteger2.Multiply(BigFivePows[1].Pow(num));
			bigInteger2 = bigInteger2.ShiftLeft(int.MaxValue);
			for (num3 = exp - int.MaxValue; num3 > int.MaxValue; num3 -= int.MaxValue)
			{
				bigInteger2 = bigInteger2.ShiftLeft(int.MaxValue);
			}
			return bigInteger2.ShiftLeft(num);
		}

		public static BigInteger MultiplyByFivePow(BigInteger val, int exp)
		{
			if (exp < FivePows.Length)
			{
				return MultiplyByPositiveInt(val, FivePows[exp]);
			}
			if (exp < BigFivePows.Length)
			{
				return val.Multiply(BigFivePows[exp]);
			}
			return val.Multiply(BigFivePows[1].Pow(exp));
		}

		public static long UnsignedMultAddAdd(int a, int b, int c, int d)
		{
			return (a & 0xFFFFFFFFu) * (b & 0xFFFFFFFFu) + (c & 0xFFFFFFFFu) + (d & 0xFFFFFFFFu);
		}
	}
}
