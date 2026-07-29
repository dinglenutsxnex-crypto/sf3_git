using System;
using System.Text;

namespace Deveel.Math
{
	internal static class Conversion
	{
		internal static readonly int[] digitFitInInt = new int[37]
		{
			-1, -1, 31, 19, 15, 13, 11, 11, 10, 9,
			9, 8, 8, 8, 8, 7, 7, 7, 7, 7,
			7, 7, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 5
		};

		internal static readonly int[] bigRadices = new int[35]
		{
			-2147483648, 1162261467, 1073741824, 1220703125, 362797056, 1977326743, 1073741824, 387420489, 1000000000, 214358881,
			429981696, 815730721, 1475789056, 170859375, 268435456, 410338673, 612220032, 893871739, 1280000000, 1801088541,
			113379904, 148035889, 191102976, 244140625, 308915776, 387420489, 481890304, 594823321, 729000000, 887503681,
			1073741824, 1291467969, 1544804416, 1838265625, 60466176
		};

		public static string BigInteger2String(BigInteger val, int radix)
		{
			int sign = val.Sign;
			int numberLength = val.numberLength;
			int[] digits = val.Digits;
			if (sign == 0)
			{
				return "0";
			}
			if (numberLength == 1)
			{
				int num = digits[numberLength - 1];
				long num2 = num & 0xFFFFFFFFu;
				if (sign < 0)
				{
					return "-" + Convert.ToString(num2, radix);
				}
				return Convert.ToString(num2, radix);
			}
			if (radix == 10 || radix < 2 || radix > 36)
			{
				return val.ToString();
			}
			double num3 = System.Math.Log(radix) / System.Math.Log(2.0);
			int num4 = (int)((double)val.Abs().BitLength / num3 + (double)((sign < 0) ? 1 : 0)) + 1;
			char[] array = new char[num4];
			int i = num4;
			if (radix != 16)
			{
				int[] array2 = new int[numberLength];
				Array.Copy(digits, 0, array2, 0, numberLength);
				int num5 = numberLength;
				int num6 = digitFitInInt[radix];
				int divisor = bigRadices[radix - 2];
				do
				{
					int num7 = Division.DivideArrayByInt(array2, array2, num5, divisor);
					int num8 = i;
					do
					{
						array[--i] = CharHelper.forDigit(num7 % radix, radix);
					}
					while ((num7 /= radix) != 0 && i != 0);
					int num9 = num6 - num8 + i;
					int j;
					for (j = 0; j < num9; j++)
					{
						if (i <= 0)
						{
							break;
						}
						array[--i] = '0';
					}
					j = num5 - 1;
					while (j > 0 && array2[j] == 0)
					{
						j--;
					}
					num5 = j + 1;
				}
				while (num5 != 1 || array2[0] != 0);
			}
			else
			{
				for (int k = 0; k < numberLength; k++)
				{
					for (int l = 0; l < 8; l++)
					{
						if (i <= 0)
						{
							break;
						}
						int num7 = (digits[k] >> (l << 2)) & 0xF;
						array[--i] = CharHelper.forDigit(num7, 16);
					}
				}
			}
			for (; array[i] == '0'; i++)
			{
			}
			if (sign == -1)
			{
				array[--i] = '-';
			}
			return new string(array, i, num4 - i);
		}

		public static string ToDecimalScaledString(BigInteger val, int scale)
		{
			int sign = val.Sign;
			int numberLength = val.numberLength;
			int[] digits = val.Digits;
			if (sign == 0)
			{
				switch (scale)
				{
				case 0:
					return "0";
				case 1:
					return "0.0";
				case 2:
					return "0.00";
				case 3:
					return "0.000";
				case 4:
					return "0.0000";
				case 5:
					return "0.00000";
				case 6:
					return "0.000000";
				default:
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (scale < 0)
					{
						stringBuilder.Append("0E+");
					}
					else
					{
						stringBuilder.Append("0E");
					}
					stringBuilder.Append(-scale);
					return stringBuilder.ToString();
				}
				}
			}
			int num = numberLength * 10 + 1 + 7;
			char[] array = new char[num + 1];
			int i = num;
			if (numberLength == 1)
			{
				int num2 = digits[0];
				if (num2 < 0)
				{
					long num3 = num2 & 0xFFFFFFFFu;
					do
					{
						long num4 = num3;
						num3 /= 10;
						array[--i] = (char)(48 + (int)(num4 - num3 * 10));
					}
					while (num3 != 0);
				}
				else
				{
					int num5 = num2;
					do
					{
						int num6 = num5;
						num5 /= 10;
						array[--i] = (char)(48 + (num6 - num5 * 10));
					}
					while (num5 != 0);
				}
			}
			else
			{
				int[] array2 = new int[numberLength];
				int num7 = numberLength;
				Array.Copy(digits, 0, array2, 0, num7);
				while (true)
				{
					long num8 = 0L;
					for (int num9 = num7 - 1; num9 >= 0; num9--)
					{
						long a = (num8 << 32) + (array2[num9] & 0xFFFFFFFFu);
						long num10 = DivideLongByBillion(a);
						array2[num9] = (int)num10;
						num8 = (int)(num10 >> 32);
					}
					int num11 = (int)num8;
					int num12 = i;
					do
					{
						array[--i] = (char)(48 + num11 % 10);
					}
					while ((num11 /= 10) != 0 && i != 0);
					int num13 = 9 - num12 + i;
					for (int j = 0; j < num13; j++)
					{
						if (i <= 0)
						{
							break;
						}
						array[--i] = '0';
					}
					int num14 = num7 - 1;
					while (array2[num14] == 0)
					{
						if (num14 == 0)
						{
							goto end_IL_0157;
						}
						num14--;
					}
					num7 = num14 + 1;
					continue;
					end_IL_0157:
					break;
				}
				for (; array[i] == '0'; i++)
				{
				}
			}
			bool flag = sign < 0;
			int num15 = num - i - scale - 1;
			if (scale == 0)
			{
				if (flag)
				{
					array[--i] = '-';
				}
				return new string(array, i, num - i);
			}
			if (scale > 0 && num15 >= -6)
			{
				if (num15 >= 0)
				{
					int num16 = i + num15;
					for (int num17 = num - 1; num17 >= num16; num17--)
					{
						array[num17 + 1] = array[num17];
					}
					array[++num16] = '.';
					if (flag)
					{
						array[--i] = '-';
					}
					return new string(array, i, num - i + 1);
				}
				for (int k = 2; k < -num15 + 1; k++)
				{
					array[--i] = '0';
				}
				array[--i] = '.';
				array[--i] = '0';
				if (flag)
				{
					array[--i] = '-';
				}
				return new string(array, i, num - i);
			}
			int num18 = i + 1;
			int num19 = num;
			StringBuilder stringBuilder2 = new StringBuilder(16 + num19 - num18);
			if (flag)
			{
				stringBuilder2.Append('-');
			}
			if (num19 - num18 >= 1)
			{
				stringBuilder2.Append(array[i]);
				stringBuilder2.Append('.');
				stringBuilder2.Append(array, i + 1, num - i - 1);
			}
			else
			{
				stringBuilder2.Append(array, i, num - i);
			}
			stringBuilder2.Append('E');
			if (num15 > 0)
			{
				stringBuilder2.Append('+');
			}
			stringBuilder2.Append(Convert.ToString(num15));
			return stringBuilder2.ToString();
		}

		public static string ToDecimalScaledString(long value, int scale)
		{
			bool flag = value < 0;
			if (flag)
			{
				value = -value;
			}
			if (value == 0)
			{
				switch (scale)
				{
				case 0:
					return "0";
				case 1:
					return "0.0";
				case 2:
					return "0.00";
				case 3:
					return "0.000";
				case 4:
					return "0.0000";
				case 5:
					return "0.00000";
				case 6:
					return "0.000000";
				default:
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (scale < 0)
					{
						stringBuilder.Append("0E+");
					}
					else
					{
						stringBuilder.Append("0E");
					}
					stringBuilder.Append((scale != int.MinValue) ? Convert.ToString(-scale) : "2147483648");
					return stringBuilder.ToString();
				}
				}
			}
			int num = 18;
			char[] array = new char[num + 1];
			int num2 = num;
			long num3 = value;
			do
			{
				long num4 = num3;
				num3 /= 10;
				array[--num2] = (char)(48 + (num4 - num3 * 10));
			}
			while (num3 != 0);
			long num5 = (long)num - (long)num2 - scale - 1;
			if (scale == 0)
			{
				if (flag)
				{
					array[--num2] = '-';
				}
				return new string(array, num2, num - num2);
			}
			if (scale > 0 && num5 >= -6)
			{
				if (num5 >= 0)
				{
					int num6 = num2 + (int)num5;
					for (int num7 = num - 1; num7 >= num6; num7--)
					{
						array[num7 + 1] = array[num7];
					}
					array[++num6] = '.';
					if (flag)
					{
						array[--num2] = '-';
					}
					return new string(array, num2, num - num2 + 1);
				}
				for (int i = 2; i < -num5 + 1; i++)
				{
					array[--num2] = '0';
				}
				array[--num2] = '.';
				array[--num2] = '0';
				if (flag)
				{
					array[--num2] = '-';
				}
				return new string(array, num2, num - num2);
			}
			int num8 = num2 + 1;
			int num9 = num;
			StringBuilder stringBuilder2 = new StringBuilder(16 + num9 - num8);
			if (flag)
			{
				stringBuilder2.Append('-');
			}
			if (num9 - num8 >= 1)
			{
				stringBuilder2.Append(array[num2]);
				stringBuilder2.Append('.');
				stringBuilder2.Append(array, num2 + 1, num - num2 - 1);
			}
			else
			{
				stringBuilder2.Append(array, num2, num - num2);
			}
			stringBuilder2.Append('E');
			if (num5 > 0)
			{
				stringBuilder2.Append('+');
			}
			stringBuilder2.Append(Convert.ToString(num5));
			return stringBuilder2.ToString();
		}

		public static long DivideLongByBillion(long a)
		{
			long num2;
			long num3;
			if (a >= 0)
			{
				long num = 1000000000L;
				num2 = a / num;
				num3 = a % num;
			}
			else
			{
				long num4 = Utils.URShift(a, 1);
				long num5 = Utils.URShift(1000000000L, 1);
				num2 = num4 / num5;
				num3 = num4 % num5;
				num3 = (num3 << 1) + (a & 1);
			}
			return (num3 << 32) | (num2 & 0xFFFFFFFFu);
		}

		public static double BigInteger2Double(BigInteger val)
		{
			if (val.numberLength < 2 || (val.numberLength == 2 && val.Digits[1] > 0))
			{
				return val.ToInt64();
			}
			if (val.numberLength > 32)
			{
				return (val.Sign <= 0) ? double.NegativeInfinity : double.PositiveInfinity;
			}
			int bitLength = val.Abs().BitLength;
			long num = bitLength - 1;
			int num2 = bitLength - 54;
			long num3 = val.Abs().ShiftRight(num2).ToInt64();
			long num4 = num3 & 0x1FFFFFFFFFFFFFL;
			if (num == 1023)
			{
				switch (num4)
				{
				case 9007199254740991L:
					return (val.Sign <= 0) ? double.NegativeInfinity : double.PositiveInfinity;
				case 9007199254740990L:
					return (val.Sign <= 0) ? double.MinValue : double.MaxValue;
				}
			}
			if ((num4 & 1) == 1 && ((num4 & 2) == 2 || BitLevel.NonZeroDroppedBits(num2, val.Digits)))
			{
				num4 += 2;
			}
			num4 >>= 1;
			long num5 = ((val.Sign >= 0) ? 0 : long.MinValue);
			num = (1023 + num << 52) & 0x7FF0000000000000L;
			long value = num5 | num | num4;
			return BitConverter.Int64BitsToDouble(value);
		}
	}
}
