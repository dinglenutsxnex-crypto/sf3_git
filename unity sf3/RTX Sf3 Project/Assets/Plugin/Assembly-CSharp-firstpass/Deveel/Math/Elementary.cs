using System;

namespace Deveel.Math
{
	internal class Elementary
	{
		private Elementary()
		{
		}

		internal static int compareArrays(int[] a, int[] b, int size)
		{
			int num = size - 1;
			while (num >= 0 && a[num] == b[num])
			{
				num--;
			}
			return (num < 0) ? BigInteger.EQUALS : (((a[num] & 0xFFFFFFFFu) >= (b[num] & 0xFFFFFFFFu)) ? BigInteger.GREATER : BigInteger.LESS);
		}

		internal static BigInteger add(BigInteger op1, BigInteger op2)
		{
			int sign = op1.Sign;
			int sign2 = op2.Sign;
			if (sign == 0)
			{
				return op2;
			}
			if (sign2 == 0)
			{
				return op1;
			}
			int numberLength = op1.numberLength;
			int numberLength2 = op2.numberLength;
			if (numberLength + numberLength2 == 2)
			{
				long num = op1.Digits[0] & 0xFFFFFFFFu;
				long num2 = op2.Digits[0] & 0xFFFFFFFFu;
				if (sign == sign2)
				{
					long num3 = num + num2;
					int num4 = (int)num3;
					int num5 = (int)Utils.URShift(num3, 32);
					return (num5 != 0) ? new BigInteger(sign, 2, new int[2] { num4, num5 }) : new BigInteger(sign, num4);
				}
				return BigInteger.ValueOf((sign >= 0) ? (num - num2) : (num2 - num));
			}
			int sign3;
			int[] array;
			if (sign == sign2)
			{
				sign3 = sign;
				array = ((numberLength < numberLength2) ? add(op2.Digits, numberLength2, op1.Digits, numberLength) : add(op1.Digits, numberLength, op2.Digits, numberLength2));
			}
			else
			{
				int num6 = ((numberLength == numberLength2) ? compareArrays(op1.Digits, op2.Digits, numberLength) : ((numberLength > numberLength2) ? 1 : (-1)));
				if (num6 == BigInteger.EQUALS)
				{
					return BigInteger.Zero;
				}
				if (num6 == BigInteger.GREATER)
				{
					sign3 = sign;
					array = subtract(op1.Digits, numberLength, op2.Digits, numberLength2);
				}
				else
				{
					sign3 = sign2;
					array = subtract(op2.Digits, numberLength2, op1.Digits, numberLength);
				}
			}
			BigInteger bigInteger = new BigInteger(sign3, array.Length, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		private static void add(int[] res, int[] a, int aSize, int[] b, int bSize)
		{
			long num = (a[0] & 0xFFFFFFFFu) + (b[0] & 0xFFFFFFFFu);
			res[0] = (int)num;
			num >>= 32;
			int i;
			if (aSize >= bSize)
			{
				for (i = 1; i < bSize; i++)
				{
					num += (a[i] & 0xFFFFFFFFu) + (b[i] & 0xFFFFFFFFu);
					res[i] = (int)num;
					num >>= 32;
				}
				for (; i < aSize; i++)
				{
					num += a[i] & 0xFFFFFFFFu;
					res[i] = (int)num;
					num >>= 32;
				}
			}
			else
			{
				for (i = 1; i < aSize; i++)
				{
					num += (a[i] & 0xFFFFFFFFu) + (b[i] & 0xFFFFFFFFu);
					res[i] = (int)num;
					num >>= 32;
				}
				for (; i < bSize; i++)
				{
					num += b[i] & 0xFFFFFFFFu;
					res[i] = (int)num;
					num >>= 32;
				}
			}
			if (num != 0)
			{
				res[i] = (int)num;
			}
		}

		internal static BigInteger subtract(BigInteger op1, BigInteger op2)
		{
			int sign = op1.Sign;
			int sign2 = op2.Sign;
			if (sign2 == 0)
			{
				return op1;
			}
			if (sign == 0)
			{
				return op2.Negate();
			}
			int numberLength = op1.numberLength;
			int numberLength2 = op2.numberLength;
			if (numberLength + numberLength2 == 2)
			{
				long num = op1.Digits[0] & 0xFFFFFFFFu;
				long num2 = op2.Digits[0] & 0xFFFFFFFFu;
				if (sign < 0)
				{
					num = -num;
				}
				if (sign2 < 0)
				{
					num2 = -num2;
				}
				return BigInteger.ValueOf(num - num2);
			}
			int num3 = ((numberLength == numberLength2) ? compareArrays(op1.Digits, op2.Digits, numberLength) : ((numberLength > numberLength2) ? 1 : (-1)));
			int sign3;
			int[] array;
			if (num3 == BigInteger.LESS)
			{
				sign3 = -sign2;
				array = ((sign != sign2) ? add(op2.Digits, numberLength2, op1.Digits, numberLength) : subtract(op2.Digits, numberLength2, op1.Digits, numberLength));
			}
			else
			{
				sign3 = sign;
				if (sign == sign2)
				{
					if (num3 == BigInteger.EQUALS)
					{
						return BigInteger.Zero;
					}
					array = subtract(op1.Digits, numberLength, op2.Digits, numberLength2);
				}
				else
				{
					array = add(op1.Digits, numberLength, op2.Digits, numberLength2);
				}
			}
			BigInteger bigInteger = new BigInteger(sign3, array.Length, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		private static void subtract(int[] res, int[] a, int aSize, int[] b, int bSize)
		{
			long num = 0L;
			int i;
			for (i = 0; i < bSize; i++)
			{
				num += (a[i] & 0xFFFFFFFFu) - (b[i] & 0xFFFFFFFFu);
				res[i] = (int)num;
				num >>= 32;
			}
			for (; i < aSize; i++)
			{
				num += a[i] & 0xFFFFFFFFu;
				res[i] = (int)num;
				num >>= 32;
			}
		}

		private static int[] add(int[] a, int aSize, int[] b, int bSize)
		{
			int[] array = new int[aSize + 1];
			add(array, a, aSize, b, bSize);
			return array;
		}

		internal static void inplaceAdd(BigInteger op1, BigInteger op2)
		{
			add(op1.Digits, op1.Digits, op1.numberLength, op2.Digits, op2.numberLength);
			op1.numberLength = System.Math.Min(System.Math.Max(op1.numberLength, op2.numberLength) + 1, op1.Digits.Length);
			op1.CutOffLeadingZeroes();
			op1.UnCache();
		}

		internal static int inplaceAdd(int[] a, int aSize, int addend)
		{
			long num = addend & 0xFFFFFFFFu;
			int num2 = 0;
			while (num != 0 && num2 < aSize)
			{
				num += a[num2] & 0xFFFFFFFFu;
				a[num2] = (int)num;
				num >>= 32;
				num2++;
			}
			return (int)num;
		}

		internal static void inplaceAdd(BigInteger op1, int addend)
		{
			int num = inplaceAdd(op1.Digits, op1.numberLength, addend);
			if (num == 1)
			{
				op1.Digits[op1.numberLength] = 1;
				op1.numberLength++;
			}
			op1.UnCache();
		}

		internal static void inplaceSubtract(BigInteger op1, BigInteger op2)
		{
			subtract(op1.Digits, op1.Digits, op1.numberLength, op2.Digits, op2.numberLength);
			op1.CutOffLeadingZeroes();
			op1.UnCache();
		}

		private static void inverseSubtract(int[] res, int[] a, int aSize, int[] b, int bSize)
		{
			long num = 0L;
			if (aSize < bSize)
			{
				int i;
				for (i = 0; i < aSize; i++)
				{
					num += (b[i] & 0xFFFFFFFFu) - (a[i] & 0xFFFFFFFFu);
					res[i] = (int)num;
					num >>= 32;
				}
				for (; i < bSize; i++)
				{
					num += b[i] & 0xFFFFFFFFu;
					res[i] = (int)num;
					num >>= 32;
				}
			}
			else
			{
				int i;
				for (i = 0; i < bSize; i++)
				{
					num += (b[i] & 0xFFFFFFFFu) - (a[i] & 0xFFFFFFFFu);
					res[i] = (int)num;
					num >>= 32;
				}
				for (; i < aSize; i++)
				{
					num -= a[i] & 0xFFFFFFFFu;
					res[i] = (int)num;
					num >>= 32;
				}
			}
		}

		private static int[] subtract(int[] a, int aSize, int[] b, int bSize)
		{
			int[] array = new int[aSize];
			subtract(array, a, aSize, b, bSize);
			return array;
		}

		internal static void completeInPlaceSubtract(BigInteger op1, BigInteger op2)
		{
			int sign = op1.CompareTo(op2);
			if (op1.Sign == 0)
			{
				Array.Copy(op2.Digits, 0, op1.Digits, 0, op2.numberLength);
				op1.Sign = -op2.Sign;
			}
			else if (op1.Sign != op2.Sign)
			{
				add(op1.Digits, op1.Digits, op1.numberLength, op2.Digits, op2.numberLength);
				op1.Sign = sign;
			}
			else
			{
				int num = unsignedArraysCompare(op1.Digits, op2.Digits, op1.numberLength, op2.numberLength);
				if (num > 0)
				{
					subtract(op1.Digits, op1.Digits, op1.numberLength, op2.Digits, op2.numberLength);
				}
				else
				{
					inverseSubtract(op1.Digits, op1.Digits, op1.numberLength, op2.Digits, op2.numberLength);
					op1.Sign = -op1.Sign;
				}
			}
			op1.numberLength = System.Math.Max(op1.numberLength, op2.numberLength) + 1;
			op1.CutOffLeadingZeroes();
			op1.UnCache();
		}

		internal static void completeInPlaceAdd(BigInteger op1, BigInteger op2)
		{
			if (op1.Sign == 0)
			{
				Array.Copy(op2.Digits, 0, op1.Digits, 0, op2.numberLength);
			}
			else
			{
				if (op2.Sign == 0)
				{
					return;
				}
				if (op1.Sign == op2.Sign)
				{
					add(op1.Digits, op1.Digits, op1.numberLength, op2.Digits, op2.numberLength);
				}
				else
				{
					int num = unsignedArraysCompare(op1.Digits, op2.Digits, op1.numberLength, op2.numberLength);
					if (num > 0)
					{
						subtract(op1.Digits, op1.Digits, op1.numberLength, op2.Digits, op2.numberLength);
					}
					else
					{
						inverseSubtract(op1.Digits, op1.Digits, op1.numberLength, op2.Digits, op2.numberLength);
						op1.Sign = -op1.Sign;
					}
				}
			}
			op1.numberLength = System.Math.Max(op1.numberLength, op2.numberLength) + 1;
			op1.CutOffLeadingZeroes();
			op1.UnCache();
		}

		private static int unsignedArraysCompare(int[] a, int[] b, int aSize, int bSize)
		{
			if (aSize > bSize)
			{
				return 1;
			}
			if (aSize < bSize)
			{
				return -1;
			}
			int num = aSize - 1;
			while (num >= 0 && a[num] == b[num])
			{
				num--;
			}
			return (num < 0) ? BigInteger.EQUALS : (((a[num] & 0xFFFFFFFFu) >= (b[num] & 0xFFFFFFFFu)) ? BigInteger.GREATER : BigInteger.LESS);
		}
	}
}
