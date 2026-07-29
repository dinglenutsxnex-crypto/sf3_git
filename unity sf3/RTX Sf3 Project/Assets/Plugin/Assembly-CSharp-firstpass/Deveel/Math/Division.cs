using System;

namespace Deveel.Math
{
	internal static class Division
	{
		public static int[] Divide(int[] quot, int quotLength, int[] a, int aLength, int[] b, int bLength)
		{
			int[] array = new int[aLength + 1];
			int[] array2 = new int[bLength + 1];
			int num = Utils.NumberOfLeadingZeros(b[bLength - 1]);
			if (num != 0)
			{
				BitLevel.ShiftLeft(array2, b, 0, num);
				BitLevel.ShiftLeft(array, a, 0, num);
			}
			else
			{
				Array.Copy(a, 0, array, 0, aLength);
				Array.Copy(b, 0, array2, 0, bLength);
			}
			int num2 = array2[bLength - 1];
			int num3 = quotLength - 1;
			int num4 = aLength;
			while (num3 >= 0)
			{
				int num5 = 0;
				if (array[num4] == num2)
				{
					num5 = -1;
				}
				else
				{
					long a2 = ((array[num4] & 0xFFFFFFFFu) << 32) + (array[num4 - 1] & 0xFFFFFFFFu);
					long num6 = DivideLongByInt(a2, num2);
					num5 = (int)num6;
					int num7 = (int)(num6 >> 32);
					if (num5 != 0)
					{
						long num8 = 0L;
						long num9 = 0L;
						bool flag = false;
						num5++;
						do
						{
							num5--;
							if (flag)
							{
								break;
							}
							num8 = (num5 & 0xFFFFFFFFu) * (array2[bLength - 2] & 0xFFFFFFFFu);
							num9 = ((long)num7 << 32) + (array[num4 - 2] & 0xFFFFFFFFu);
							long num10 = (num7 & 0xFFFFFFFFu) + (num2 & 0xFFFFFFFFu);
							if (Utils.NumberOfLeadingZeros((int)Utils.URShift(num10, 32)) < 32)
							{
								flag = true;
							}
							else
							{
								num7 = (int)num10;
							}
						}
						while ((num8 ^ long.MinValue) > (num9 ^ long.MinValue));
					}
				}
				if (num5 != 0 && MultiplyAndSubtract(array, num4 - bLength, array2, bLength, num5) != 0)
				{
					num5--;
					long num11 = 0L;
					for (int i = 0; i < bLength; i++)
					{
						num11 += (array[num4 - bLength + i] & 0xFFFFFFFFu) + (array2[i] & 0xFFFFFFFFu);
						array[num4 - bLength + i] = (int)num11;
						num11 = Utils.URShift(num11, 32);
					}
				}
				if (quot != null)
				{
					quot[num3] = num5;
				}
				num4--;
				num3--;
			}
			if (num != 0)
			{
				BitLevel.ShiftRight(array2, bLength, array, 0, num);
				return array2;
			}
			Array.Copy(array, 0, array2, 0, bLength);
			return array;
		}

		public static int DivideArrayByInt(int[] dest, int[] src, int srcLength, int divisor)
		{
			long num = 0L;
			long num2 = divisor & 0xFFFFFFFFu;
			for (int num3 = srcLength - 1; num3 >= 0; num3--)
			{
				long num4 = (num << 32) | (src[num3] & 0xFFFFFFFFu);
				long num5;
				if (num4 >= 0)
				{
					num5 = num4 / num2;
					num = num4 % num2;
				}
				else
				{
					long num6 = Utils.URShift(num4, 1);
					long num7 = Utils.URShift(divisor, 1);
					num5 = num6 / num7;
					num = num6 % num7;
					num = (num << 1) + (num4 & 1);
					if (((uint)divisor & (true ? 1u : 0u)) != 0)
					{
						if (num5 <= num)
						{
							num -= num5;
						}
						else if (num5 - num <= num2)
						{
							num += num2 - num5;
							num5--;
						}
						else
						{
							num += (num2 << 1) - num5;
							num5 -= 2;
						}
					}
				}
				dest[num3] = (int)(num5 & 0xFFFFFFFFu);
			}
			return (int)num;
		}

		public static int RemainderArrayByInt(int[] src, int srcLength, int divisor)
		{
			long num = 0L;
			for (int num2 = srcLength - 1; num2 >= 0; num2--)
			{
				long a = (num << 32) + (src[num2] & 0xFFFFFFFFu);
				long num3 = DivideLongByInt(a, divisor);
				num = (int)(num3 >> 32);
			}
			return (int)num;
		}

		public static int Remainder(BigInteger dividend, int divisor)
		{
			return RemainderArrayByInt(dividend.Digits, dividend.numberLength, divisor);
		}

		private static long DivideLongByInt(long a, int b)
		{
			long num = b & 0xFFFFFFFFu;
			long num2;
			long num3;
			if (a >= 0)
			{
				num2 = a / num;
				num3 = a % num;
			}
			else
			{
				long num4 = Utils.URShift(a, 1);
				long num5 = Utils.URShift(b, 1);
				num2 = num4 / num5;
				num3 = num4 % num5;
				num3 = (num3 << 1) + (a & 1);
				if (((uint)b & (true ? 1u : 0u)) != 0)
				{
					if (num2 <= num3)
					{
						num3 -= num2;
					}
					else if (num2 - num3 <= num)
					{
						num3 += num - num2;
						num2--;
					}
					else
					{
						num3 += (num << 1) - num2;
						num2 -= 2;
					}
				}
			}
			return (num3 << 32) | (num2 & 0xFFFFFFFFu);
		}

		public static BigInteger[] DivideAndRemainderByInteger(BigInteger val, int divisor, int divisorSign)
		{
			int[] digits = val.Digits;
			int numberLength = val.numberLength;
			int sign = val.Sign;
			if (numberLength == 1)
			{
				long num = digits[0] & 0xFFFFFFFFu;
				long num2 = divisor & 0xFFFFFFFFu;
				long num3 = num / num2;
				long num4 = num % num2;
				if (sign != divisorSign)
				{
					num3 = -num3;
				}
				if (sign < 0)
				{
					num4 = -num4;
				}
				return new BigInteger[2]
				{
					BigInteger.ValueOf(num3),
					BigInteger.ValueOf(num4)
				};
			}
			int num5 = numberLength;
			int sign2 = ((sign == divisorSign) ? 1 : (-1));
			int[] array = new int[num5];
			int[] digits2 = new int[1] { DivideArrayByInt(array, digits, numberLength, divisor) };
			BigInteger bigInteger = new BigInteger(sign2, num5, array);
			BigInteger bigInteger2 = new BigInteger(sign, 1, digits2);
			bigInteger.CutOffLeadingZeroes();
			bigInteger2.CutOffLeadingZeroes();
			return new BigInteger[2] { bigInteger, bigInteger2 };
		}

		public static int MultiplyAndSubtract(int[] a, int start, int[] b, int bLen, int c)
		{
			long num = 0L;
			long num2 = 0L;
			for (int i = 0; i < bLen; i++)
			{
				num = Multiplication.UnsignedMultAddAdd(b[i], c, (int)num, 0);
				num2 = (a[start + i] & 0xFFFFFFFFu) - (num & 0xFFFFFFFFu) + num2;
				a[start + i] = (int)num2;
				num2 >>= 32;
				num = Utils.URShift(num, 32);
			}
			num2 = (a[start + bLen] & 0xFFFFFFFFu) - num + num2;
			a[start + bLen] = (int)num2;
			return (int)(num2 >> 32);
		}

		public static BigInteger GcdBinary(BigInteger op1, BigInteger op2)
		{
			int lowestSetBit = op1.LowestSetBit;
			int lowestSetBit2 = op2.LowestSetBit;
			int n = System.Math.Min(lowestSetBit, lowestSetBit2);
			BitLevel.InplaceShiftRight(op1, lowestSetBit);
			BitLevel.InplaceShiftRight(op2, lowestSetBit2);
			if (op1.CompareTo(op2) == BigInteger.GREATER)
			{
				BigInteger bigInteger = op1;
				op1 = op2;
				op2 = bigInteger;
			}
			do
			{
				if (op2.numberLength == 1 || (op2.numberLength == 2 && op2.Digits[1] > 0))
				{
					op2 = BigInteger.ValueOf(GcdBinary(op1.ToInt64(), op2.ToInt64()));
					break;
				}
				if ((double)op2.numberLength > (double)op1.numberLength * 1.2)
				{
					op2 = op2.Remainder(op1);
					if (op2.Sign != 0)
					{
						BitLevel.InplaceShiftRight(op2, op2.LowestSetBit);
					}
				}
				else
				{
					do
					{
						Elementary.inplaceSubtract(op2, op1);
						BitLevel.InplaceShiftRight(op2, op2.LowestSetBit);
					}
					while (op2.CompareTo(op1) >= BigInteger.EQUALS);
				}
				BigInteger bigInteger = op2;
				op2 = op1;
				op1 = bigInteger;
			}
			while (op1.Sign != 0);
			return op2.ShiftLeft(n);
		}

		public static long GcdBinary(long op1, long op2)
		{
			int num = Utils.NumberOfTrailingZeros(op1);
			int num2 = Utils.NumberOfTrailingZeros(op2);
			int num3 = System.Math.Min(num, num2);
			if (num != 0)
			{
				op1 = Utils.URShift(op1, num);
			}
			if (num2 != 0)
			{
				op2 = Utils.URShift(op2, num2);
			}
			do
			{
				if (op1 >= op2)
				{
					op1 -= op2;
					op1 = Utils.URShift(op1, Utils.NumberOfTrailingZeros(op1));
				}
				else
				{
					op2 -= op1;
					op2 = Utils.URShift(op2, Utils.NumberOfTrailingZeros(op2));
				}
			}
			while (op1 != 0);
			return op2 << num3;
		}

		public static BigInteger ModInverseMontgomery(BigInteger a, BigInteger p)
		{
			if (a.Sign == 0)
			{
				throw new ArithmeticException("BigInteger not invertible.");
			}
			if (!p.TestBit(0))
			{
				return ModInverseLorencz(a, p);
			}
			int num = p.numberLength * 32;
			BigInteger bigInteger = p.Copy();
			BigInteger bigInteger2 = a.Copy();
			int num2 = System.Math.Max(bigInteger2.numberLength, bigInteger.numberLength);
			BigInteger bigInteger3 = new BigInteger(1, 1, new int[num2 + 1]);
			BigInteger bigInteger4 = new BigInteger(1, 1, new int[num2 + 1]);
			bigInteger4.Digits[0] = 1;
			int num3 = 0;
			int lowestSetBit = bigInteger.LowestSetBit;
			int lowestSetBit2 = bigInteger2.LowestSetBit;
			if (lowestSetBit > lowestSetBit2)
			{
				BitLevel.InplaceShiftRight(bigInteger, lowestSetBit);
				BitLevel.InplaceShiftRight(bigInteger2, lowestSetBit2);
				BitLevel.InplaceShiftLeft(bigInteger3, lowestSetBit2);
				num3 += lowestSetBit - lowestSetBit2;
			}
			else
			{
				BitLevel.InplaceShiftRight(bigInteger, lowestSetBit);
				BitLevel.InplaceShiftRight(bigInteger2, lowestSetBit2);
				BitLevel.InplaceShiftLeft(bigInteger4, lowestSetBit);
				num3 += lowestSetBit2 - lowestSetBit;
			}
			bigInteger3.Sign = 1;
			while (bigInteger2.Sign > 0)
			{
				while (bigInteger.CompareTo(bigInteger2) > BigInteger.EQUALS)
				{
					Elementary.inplaceSubtract(bigInteger, bigInteger2);
					int lowestSetBit3 = bigInteger.LowestSetBit;
					BitLevel.InplaceShiftRight(bigInteger, lowestSetBit3);
					Elementary.inplaceAdd(bigInteger3, bigInteger4);
					BitLevel.InplaceShiftLeft(bigInteger4, lowestSetBit3);
					num3 += lowestSetBit3;
				}
				while (bigInteger.CompareTo(bigInteger2) <= BigInteger.EQUALS)
				{
					Elementary.inplaceSubtract(bigInteger2, bigInteger);
					if (bigInteger2.Sign == 0)
					{
						break;
					}
					int lowestSetBit3 = bigInteger2.LowestSetBit;
					BitLevel.InplaceShiftRight(bigInteger2, lowestSetBit3);
					Elementary.inplaceAdd(bigInteger4, bigInteger3);
					BitLevel.InplaceShiftLeft(bigInteger3, lowestSetBit3);
					num3 += lowestSetBit3;
				}
			}
			if (!bigInteger.IsOne)
			{
				throw new ArithmeticException("BigInteger not invertible.");
			}
			if (bigInteger3.CompareTo(p) >= BigInteger.EQUALS)
			{
				Elementary.inplaceSubtract(bigInteger3, p);
			}
			bigInteger3 = p.Subtract(bigInteger3);
			int n = CalcN(p);
			if (num3 > num)
			{
				bigInteger3 = MonPro(bigInteger3, BigInteger.One, p, n);
				num3 -= num;
			}
			return MonPro(bigInteger3, BigInteger.GetPowerOfTwo(num - num3), p, n);
		}

		private static int CalcN(BigInteger a)
		{
			long num = a.Digits[0] & 0xFFFFFFFFu;
			long num2 = 1L;
			long num3 = 2L;
			do
			{
				if (((num * num2) & num3) != 0)
				{
					num2 |= num3;
				}
				num3 <<= 1;
			}
			while (num3 < 4294967296L);
			num2 = -num2;
			return (int)(num2 & 0xFFFFFFFFu);
		}

		public static bool IsPowerOfTwo(BigInteger bi, int exp)
		{
			bool flag = false;
			flag = exp >> 5 == bi.numberLength - 1 && bi.Digits[bi.numberLength - 1] == 1 << exp;
			if (flag)
			{
				int num = 0;
				while (flag && num < bi.numberLength - 1)
				{
					flag = bi.Digits[num] == 0;
					num++;
				}
			}
			return flag;
		}

		private static int HowManyIterations(BigInteger bi, int n)
		{
			int num = n - 1;
			if (bi.Sign > 0)
			{
				while (!bi.TestBit(num))
				{
					num--;
				}
				return n - 1 - num;
			}
			while (bi.TestBit(num))
			{
				num--;
			}
			return n - 1 - System.Math.Max(num, bi.LowestSetBit);
		}

		private static BigInteger ModInverseLorencz(BigInteger a, BigInteger modulo)
		{
			int num = System.Math.Max(a.numberLength, modulo.numberLength);
			int[] array = new int[num + 1];
			int[] array2 = new int[num + 1];
			Array.Copy(modulo.Digits, 0, array, 0, modulo.numberLength);
			Array.Copy(a.Digits, 0, array2, 0, a.numberLength);
			BigInteger bigInteger = new BigInteger(modulo.Sign, modulo.numberLength, array);
			BigInteger bigInteger2 = new BigInteger(a.Sign, a.numberLength, array2);
			BigInteger bigInteger3 = new BigInteger(0, 1, new int[num + 1]);
			BigInteger bigInteger4 = new BigInteger(1, 1, new int[num + 1]);
			bigInteger4.Digits[0] = 1;
			int num2 = 0;
			int num3 = 0;
			int bitLength = modulo.BitLength;
			while (!IsPowerOfTwo(bigInteger, num2) && !IsPowerOfTwo(bigInteger2, num3))
			{
				int num4 = HowManyIterations(bigInteger, bitLength);
				if (num4 != 0)
				{
					BitLevel.InplaceShiftLeft(bigInteger, num4);
					if (num2 >= num3)
					{
						BitLevel.InplaceShiftLeft(bigInteger3, num4);
					}
					else
					{
						BitLevel.InplaceShiftRight(bigInteger4, System.Math.Min(num3 - num2, num4));
						if (num4 - (num3 - num2) > 0)
						{
							BitLevel.InplaceShiftLeft(bigInteger3, num4 - num3 + num2);
						}
					}
					num2 += num4;
				}
				num4 = HowManyIterations(bigInteger2, bitLength);
				if (num4 != 0)
				{
					BitLevel.InplaceShiftLeft(bigInteger2, num4);
					if (num3 >= num2)
					{
						BitLevel.InplaceShiftLeft(bigInteger4, num4);
					}
					else
					{
						BitLevel.InplaceShiftRight(bigInteger3, System.Math.Min(num2 - num3, num4));
						if (num4 - (num2 - num3) > 0)
						{
							BitLevel.InplaceShiftLeft(bigInteger4, num4 - num2 + num3);
						}
					}
					num3 += num4;
				}
				if (bigInteger.Sign == bigInteger2.Sign)
				{
					if (num2 <= num3)
					{
						Elementary.completeInPlaceSubtract(bigInteger, bigInteger2);
						Elementary.completeInPlaceSubtract(bigInteger3, bigInteger4);
					}
					else
					{
						Elementary.completeInPlaceSubtract(bigInteger2, bigInteger);
						Elementary.completeInPlaceSubtract(bigInteger4, bigInteger3);
					}
				}
				else if (num2 <= num3)
				{
					Elementary.completeInPlaceAdd(bigInteger, bigInteger2);
					Elementary.completeInPlaceAdd(bigInteger3, bigInteger4);
				}
				else
				{
					Elementary.completeInPlaceAdd(bigInteger2, bigInteger);
					Elementary.completeInPlaceAdd(bigInteger4, bigInteger3);
				}
				if (bigInteger2.Sign == 0 || bigInteger.Sign == 0)
				{
					throw new ArithmeticException("BigInteger not invertible.");
				}
			}
			if (IsPowerOfTwo(bigInteger2, num3))
			{
				bigInteger3 = bigInteger4;
				if (bigInteger2.Sign != bigInteger.Sign)
				{
					bigInteger = bigInteger.Negate();
				}
			}
			if (bigInteger.TestBit(bitLength))
			{
				bigInteger3 = ((bigInteger3.Sign >= 0) ? modulo.Subtract(bigInteger3) : bigInteger3.Negate());
			}
			if (bigInteger3.Sign < 0)
			{
				bigInteger3 = bigInteger3.Add(modulo);
			}
			return bigInteger3;
		}

		private static BigInteger SquareAndMultiply(BigInteger x2, BigInteger a2, BigInteger exponent, BigInteger modulus, int n2)
		{
			BigInteger bigInteger = x2;
			for (int num = exponent.BitLength - 1; num >= 0; num--)
			{
				bigInteger = MonPro(bigInteger, bigInteger, modulus, n2);
				if (BitLevel.TestBit(exponent, num))
				{
					bigInteger = MonPro(bigInteger, a2, modulus, n2);
				}
			}
			return bigInteger;
		}

		private static BigInteger SlidingWindow(BigInteger x2, BigInteger a2, BigInteger exponent, BigInteger modulus, int n2)
		{
			BigInteger[] array = new BigInteger[8];
			BigInteger bigInteger = x2;
			array[0] = a2;
			BigInteger b = MonPro(a2, a2, modulus, n2);
			for (int i = 1; i <= 7; i++)
			{
				array[i] = MonPro(array[i - 1], b, modulus, n2);
			}
			for (int num = exponent.BitLength - 1; num >= 0; num--)
			{
				if (BitLevel.TestBit(exponent, num))
				{
					int num2 = 1;
					int num3 = num;
					for (int j = System.Math.Max(num - 3, 0); j <= num - 1; j++)
					{
						if (BitLevel.TestBit(exponent, j))
						{
							if (j < num3)
							{
								num3 = j;
								num2 = (num2 << num - j) ^ 1;
							}
							else
							{
								num2 ^= 1 << j - num3;
							}
						}
					}
					for (int k = num3; k <= num; k++)
					{
						bigInteger = MonPro(bigInteger, bigInteger, modulus, n2);
					}
					bigInteger = MonPro(array[num2 - 1 >> 1], bigInteger, modulus, n2);
					num = num3;
				}
				else
				{
					bigInteger = MonPro(bigInteger, bigInteger, modulus, n2);
				}
			}
			return bigInteger;
		}

		public static BigInteger OddModPow(BigInteger b, BigInteger exponent, BigInteger modulus)
		{
			int num = modulus.numberLength << 5;
			BigInteger a = b.ShiftLeft(num).Mod(modulus);
			BigInteger x = BigInteger.GetPowerOfTwo(num).Mod(modulus);
			int n = CalcN(modulus);
			BigInteger a2 = ((modulus.numberLength != 1) ? SlidingWindow(x, a, exponent, modulus, n) : SquareAndMultiply(x, a, exponent, modulus, n));
			return MonPro(a2, BigInteger.One, modulus, n);
		}

		public static BigInteger EvenModPow(BigInteger b, BigInteger exponent, BigInteger modulus)
		{
			int lowestSetBit = modulus.LowestSetBit;
			BigInteger bigInteger = modulus.ShiftRight(lowestSetBit);
			BigInteger bigInteger2 = OddModPow(b, exponent, bigInteger);
			BigInteger bigInteger3 = Pow2ModPow(b, exponent, lowestSetBit);
			BigInteger val = ModPow2Inverse(bigInteger, lowestSetBit);
			BigInteger bigInteger4 = bigInteger3.Subtract(bigInteger2).Multiply(val);
			InplaceModPow2(bigInteger4, lowestSetBit);
			if (bigInteger4.Sign < 0)
			{
				bigInteger4 = bigInteger4.Add(BigInteger.GetPowerOfTwo(lowestSetBit));
			}
			return bigInteger2.Add(bigInteger.Multiply(bigInteger4));
		}

		private static BigInteger Pow2ModPow(BigInteger b, BigInteger exponent, int j)
		{
			BigInteger bigInteger = BigInteger.One;
			BigInteger bigInteger2 = exponent.Copy();
			BigInteger bigInteger3 = b.Copy();
			if (b.TestBit(0))
			{
				InplaceModPow2(bigInteger2, j - 1);
			}
			InplaceModPow2(bigInteger3, j);
			for (int num = bigInteger2.BitLength - 1; num >= 0; num--)
			{
				BigInteger bigInteger4 = bigInteger.Copy();
				InplaceModPow2(bigInteger4, j);
				bigInteger = bigInteger.Multiply(bigInteger4);
				if (BitLevel.TestBit(bigInteger2, num))
				{
					bigInteger = bigInteger.Multiply(bigInteger3);
					InplaceModPow2(bigInteger, j);
				}
			}
			InplaceModPow2(bigInteger, j);
			return bigInteger;
		}

		private static void MonReduction(int[] res, BigInteger modulus, int n2)
		{
			int[] digits = modulus.Digits;
			int numberLength = modulus.numberLength;
			long num = 0L;
			for (int i = 0; i < numberLength; i++)
			{
				long num2 = 0L;
				int a = (int)Multiplication.UnsignedMultAddAdd(res[i], n2, 0, 0);
				for (int j = 0; j < numberLength; j++)
				{
					num2 = Multiplication.UnsignedMultAddAdd(a, digits[j], res[i + j], (int)num2);
					res[i + j] = (int)num2;
					num2 = Utils.URShift(num2, 32);
				}
				num += (res[i + numberLength] & 0xFFFFFFFFu) + num2;
				res[i + numberLength] = (int)num;
				num = Utils.URShift(num, 32);
			}
			res[numberLength << 1] = (int)num;
			for (int k = 0; k < numberLength + 1; k++)
			{
				res[k] = res[k + numberLength];
			}
		}

		private static BigInteger MonPro(BigInteger a, BigInteger b, BigInteger modulus, int n2)
		{
			int numberLength = modulus.numberLength;
			int[] array = new int[(numberLength << 1) + 1];
			Multiplication.MultArraysPap(a.Digits, System.Math.Min(numberLength, a.numberLength), b.Digits, System.Math.Min(numberLength, b.numberLength), array);
			MonReduction(array, modulus, n2);
			return FinalSubtraction(array, modulus);
		}

		private static BigInteger FinalSubtraction(int[] res, BigInteger modulus)
		{
			int numberLength = modulus.numberLength;
			bool flag = res[numberLength] != 0;
			if (!flag)
			{
				int[] digits = modulus.Digits;
				flag = true;
				for (int num = numberLength - 1; num >= 0; num--)
				{
					if (res[num] != digits[num])
					{
						flag = res[num] != 0 && (res[num] & 0xFFFFFFFFu) > (digits[num] & 0xFFFFFFFFu);
						break;
					}
				}
			}
			BigInteger bigInteger = new BigInteger(1, numberLength + 1, res);
			if (flag)
			{
				Elementary.inplaceSubtract(bigInteger, modulus);
			}
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		private static BigInteger ModPow2Inverse(BigInteger x, int n)
		{
			BigInteger bigInteger = new BigInteger(1, new int[1 << n]);
			bigInteger.numberLength = 1;
			bigInteger.Digits[0] = 1;
			bigInteger.Sign = 1;
			for (int i = 1; i < n; i++)
			{
				if (BitLevel.TestBit(x.Multiply(bigInteger), i))
				{
					bigInteger.Digits[i >> 5] |= 1 << i;
				}
			}
			return bigInteger;
		}

		public static void InplaceModPow2(BigInteger x, int n)
		{
			int num = n >> 5;
			if (x.numberLength >= num && x.BitLength > n)
			{
				int num2 = 32 - (n & 0x1F);
				x.numberLength = num + 1;
				x.Digits[num] &= ((num2 < 32) ? Utils.URShift(-1, num2) : 0);
				x.CutOffLeadingZeroes();
			}
		}
	}
}
