using System;

namespace Deveel.Math
{
	internal static class BitLevel
	{
		public static int BitLength(BigInteger val)
		{
			if (val.Sign == 0)
			{
				return 0;
			}
			int num = val.numberLength << 5;
			int num2 = val.Digits[val.numberLength - 1];
			if (val.Sign < 0)
			{
				int firstNonzeroDigit = val.FirstNonzeroDigit;
				if (firstNonzeroDigit == val.numberLength - 1)
				{
					num2--;
				}
			}
			return num - Utils.NumberOfLeadingZeros(num2);
		}

		public static int BitCount(BigInteger val)
		{
			int num = 0;
			if (val.Sign == 0)
			{
				return 0;
			}
			int i = val.FirstNonzeroDigit;
			if (val.Sign > 0)
			{
				for (; i < val.numberLength; i++)
				{
					num += Utils.BitCount(val.Digits[i]);
				}
			}
			else
			{
				num += Utils.BitCount(-val.Digits[i]);
				for (i++; i < val.numberLength; i++)
				{
					num += Utils.BitCount(~val.Digits[i]);
				}
				num = (val.numberLength << 5) - num;
			}
			return num;
		}

		public static bool TestBit(BigInteger val, int n)
		{
			return (val.Digits[n >> 5] & (1 << (n & 0x1F))) != 0;
		}

		public static bool NonZeroDroppedBits(int numberOfBits, int[] digits)
		{
			int num = numberOfBits >> 5;
			int num2 = numberOfBits & 0x1F;
			int i;
			for (i = 0; i < num && digits[i] == 0; i++)
			{
			}
			return i != num || digits[i] << 32 - num2 != 0;
		}

		public static BigInteger ShiftLeft(BigInteger source, int count)
		{
			int num = count >> 5;
			count &= 0x1F;
			int num2 = source.numberLength + num + ((count != 0) ? 1 : 0);
			int[] array = new int[num2];
			ShiftLeft(array, source.Digits, num, count);
			BigInteger bigInteger = new BigInteger(source.Sign, num2, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		public static void InplaceShiftLeft(BigInteger val, int count)
		{
			int num = count >> 5;
			val.numberLength += num + ((Utils.NumberOfLeadingZeros(val.Digits[val.numberLength - 1]) - (count & 0x1F) < 0) ? 1 : 0);
			ShiftLeft(val.Digits, val.Digits, num, count & 0x1F);
			val.CutOffLeadingZeroes();
			val.UnCache();
		}

		public static void ShiftLeft(int[] result, int[] source, int intCount, int count)
		{
			if (count == 0)
			{
				Array.Copy(source, 0, result, intCount, result.Length - intCount);
			}
			else
			{
				int bits = 32 - count;
				result[result.Length - 1] = 0;
				for (int num = result.Length - 1; num > intCount; num--)
				{
					result[num] |= Utils.URShift(source[num - intCount - 1], bits);
					result[num - 1] = source[num - intCount - 1] << count;
				}
			}
			for (int i = 0; i < intCount; i++)
			{
				result[i] = 0;
			}
		}

		public static void ShiftLeftOneBit(int[] result, int[] source, int srcLen)
		{
			int num = 0;
			for (int i = 0; i < srcLen; i++)
			{
				int num2 = source[i];
				result[i] = (num2 << 1) | num;
				num = Utils.URShift(num2, 31);
			}
			if (num != 0)
			{
				result[srcLen] = num;
			}
		}

		public static BigInteger ShiftLeftOneBit(BigInteger source)
		{
			int numberLength = source.numberLength;
			int num = numberLength + 1;
			int[] array = new int[num];
			ShiftLeftOneBit(array, source.Digits, numberLength);
			BigInteger bigInteger = new BigInteger(source.Sign, num, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		public static BigInteger ShiftRight(BigInteger source, int count)
		{
			int num = count >> 5;
			count &= 0x1F;
			if (num >= source.numberLength)
			{
				return (source.Sign >= 0) ? BigInteger.Zero : BigInteger.MinusOne;
			}
			int num2 = source.numberLength - num;
			int[] array = new int[num2 + 1];
			ShiftRight(array, num2, source.Digits, num, count);
			if (source.Sign < 0)
			{
				int i;
				for (i = 0; i < num && source.Digits[i] == 0; i++)
				{
				}
				if (i < num || (count > 0 && source.Digits[i] << 32 - count != 0))
				{
					for (i = 0; i < num2 && array[i] == -1; i++)
					{
						array[i] = 0;
					}
					if (i == num2)
					{
						num2++;
					}
					array[i]++;
				}
			}
			BigInteger bigInteger = new BigInteger(source.Sign, num2, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		public static void InplaceShiftRight(BigInteger val, int count)
		{
			int sign = val.Sign;
			if (count == 0 || val.Sign == 0)
			{
				return;
			}
			int num = count >> 5;
			val.numberLength -= num;
			if (!ShiftRight(val.Digits, val.numberLength, val.Digits, num, count & 0x1F) && sign < 0)
			{
				int i;
				for (i = 0; i < val.numberLength && val.Digits[i] == -1; i++)
				{
					val.Digits[i] = 0;
				}
				if (i == val.numberLength)
				{
					val.numberLength++;
				}
				val.Digits[i]++;
			}
			val.CutOffLeadingZeroes();
			val.UnCache();
		}

		public static bool ShiftRight(int[] result, int resultLen, int[] source, int intCount, int count)
		{
			bool flag = true;
			int i;
			for (i = 0; i < intCount; i++)
			{
				flag &= source[i] == 0;
			}
			if (count == 0)
			{
				Array.Copy(source, intCount, result, 0, resultLen);
				i = resultLen;
			}
			else
			{
				int num = 32 - count;
				flag &= source[i] << num == 0;
				for (i = 0; i < resultLen - 1; i++)
				{
					result[i] = Utils.URShift(source[i + intCount], count) | (source[i + intCount + 1] << num);
				}
				result[i] = Utils.URShift(source[i + intCount], count);
				i++;
			}
			return flag;
		}

		public static BigInteger FlipBit(BigInteger val, int n)
		{
			int sign = ((val.Sign == 0) ? 1 : val.Sign);
			int num = n >> 5;
			int num2 = n & 0x1F;
			int num3 = System.Math.Max(num + 1, val.numberLength) + 1;
			int[] array = new int[num3];
			int num4 = 1 << num2;
			Array.Copy(val.Digits, 0, array, 0, val.numberLength);
			if (val.Sign < 0)
			{
				if (num >= val.numberLength)
				{
					array[num] = num4;
				}
				else
				{
					int firstNonzeroDigit = val.FirstNonzeroDigit;
					if (num > firstNonzeroDigit)
					{
						array[num] ^= num4;
					}
					else if (num < firstNonzeroDigit)
					{
						array[num] = -num4;
						int i;
						for (i = num + 1; i < firstNonzeroDigit; i++)
						{
							array[i] = -1;
						}
						array[i] = array[i]--;
					}
					else
					{
						int i = num;
						array[i] = -(-array[num] ^ num4);
						if (array[i] == 0)
						{
							for (i++; array[i] == -1; i++)
							{
								array[i] = 0;
							}
							array[i]++;
						}
					}
				}
			}
			else
			{
				array[num] ^= num4;
			}
			BigInteger bigInteger = new BigInteger(sign, num3, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}
	}
}
