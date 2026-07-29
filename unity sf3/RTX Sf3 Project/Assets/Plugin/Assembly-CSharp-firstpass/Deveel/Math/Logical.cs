using System;

namespace Deveel.Math
{
	internal static class Logical
	{
		public static BigInteger Not(BigInteger val)
		{
			if (val.Sign == 0)
			{
				return BigInteger.MinusOne;
			}
			if (val.Equals(BigInteger.MinusOne))
			{
				return BigInteger.Zero;
			}
			int[] array = new int[val.numberLength + 1];
			int i;
			if (val.Sign > 0)
			{
				if (val.Digits[val.numberLength - 1] != -1)
				{
					for (i = 0; val.Digits[i] == -1; i++)
					{
					}
				}
				else
				{
					for (i = 0; i < val.numberLength && val.Digits[i] == -1; i++)
					{
					}
					if (i == val.numberLength)
					{
						array[i] = 1;
						return new BigInteger(-val.Sign, i + 1, array);
					}
				}
			}
			else
			{
				for (i = 0; val.Digits[i] == 0; i++)
				{
					array[i] = -1;
				}
			}
			array[i] = val.Digits[i] + val.Sign;
			for (i++; i < val.numberLength; i++)
			{
				array[i] = val.Digits[i];
			}
			return new BigInteger(-val.Sign, i, array);
		}

		public static BigInteger And(BigInteger val, BigInteger that)
		{
			if (that.Sign == 0 || val.Sign == 0)
			{
				return BigInteger.Zero;
			}
			if (that.Equals(BigInteger.MinusOne))
			{
				return val;
			}
			if (val.Equals(BigInteger.MinusOne))
			{
				return that;
			}
			if (val.Sign > 0)
			{
				if (that.Sign > 0)
				{
					return AndPositive(val, that);
				}
				return AndDiffSigns(val, that);
			}
			if (that.Sign > 0)
			{
				return AndDiffSigns(that, val);
			}
			if (val.numberLength > that.numberLength)
			{
				return AndNegative(val, that);
			}
			return AndNegative(that, val);
		}

		private static BigInteger AndPositive(BigInteger val, BigInteger that)
		{
			int num = System.Math.Min(val.numberLength, that.numberLength);
			int i = System.Math.Max(val.FirstNonzeroDigit, that.FirstNonzeroDigit);
			if (i >= num)
			{
				return BigInteger.Zero;
			}
			int[] array = new int[num];
			for (; i < num; i++)
			{
				array[i] = val.Digits[i] & that.Digits[i];
			}
			BigInteger bigInteger = new BigInteger(1, num, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		private static BigInteger AndDiffSigns(BigInteger positive, BigInteger negative)
		{
			int firstNonzeroDigit = positive.FirstNonzeroDigit;
			int firstNonzeroDigit2 = negative.FirstNonzeroDigit;
			if (firstNonzeroDigit2 >= positive.numberLength)
			{
				return BigInteger.Zero;
			}
			int numberLength = positive.numberLength;
			int[] array = new int[numberLength];
			int i = System.Math.Max(firstNonzeroDigit, firstNonzeroDigit2);
			if (i == firstNonzeroDigit2)
			{
				array[i] = -negative.Digits[i] & positive.Digits[i];
				i++;
			}
			for (int num = System.Math.Min(negative.numberLength, positive.numberLength); i < num; i++)
			{
				array[i] = ~negative.Digits[i] & positive.Digits[i];
			}
			if (i >= negative.numberLength)
			{
				for (; i < positive.numberLength; i++)
				{
					array[i] = positive.Digits[i];
				}
			}
			BigInteger bigInteger = new BigInteger(1, numberLength, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		private static BigInteger AndNegative(BigInteger longer, BigInteger shorter)
		{
			int firstNonzeroDigit = longer.FirstNonzeroDigit;
			int firstNonzeroDigit2 = shorter.FirstNonzeroDigit;
			if (firstNonzeroDigit >= shorter.numberLength)
			{
				return longer;
			}
			int i = System.Math.Max(firstNonzeroDigit2, firstNonzeroDigit);
			int num = ((firstNonzeroDigit2 > firstNonzeroDigit) ? (-shorter.Digits[i] & ~longer.Digits[i]) : ((firstNonzeroDigit2 >= firstNonzeroDigit) ? (-shorter.Digits[i] & -longer.Digits[i]) : (~shorter.Digits[i] & -longer.Digits[i])));
			int num2;
			int[] array;
			if (num == 0)
			{
				for (i++; i < shorter.numberLength; i++)
				{
					if ((num = ~(longer.Digits[i] | shorter.Digits[i])) != 0)
					{
						break;
					}
				}
				if (num == 0)
				{
					for (; i < longer.numberLength; i++)
					{
						if ((num = ~longer.Digits[i]) != 0)
						{
							break;
						}
					}
					if (num == 0)
					{
						num2 = longer.numberLength + 1;
						array = new int[num2];
						array[num2 - 1] = 1;
						return new BigInteger(-1, num2, array);
					}
				}
			}
			num2 = longer.numberLength;
			array = new int[num2];
			array[i] = -num;
			for (i++; i < shorter.numberLength; i++)
			{
				array[i] = longer.Digits[i] | shorter.Digits[i];
			}
			for (; i < longer.numberLength; i++)
			{
				array[i] = longer.Digits[i];
			}
			return new BigInteger(-1, num2, array);
		}

		public static BigInteger AndNot(BigInteger val, BigInteger that)
		{
			if (that.Sign == 0)
			{
				return val;
			}
			if (val.Sign == 0)
			{
				return BigInteger.Zero;
			}
			if (val.Equals(BigInteger.MinusOne))
			{
				return that.Not();
			}
			if (that.Equals(BigInteger.MinusOne))
			{
				return BigInteger.Zero;
			}
			if (val.Sign > 0)
			{
				if (that.Sign > 0)
				{
					return AndNotPositive(val, that);
				}
				return AndNotPositiveNegative(val, that);
			}
			if (that.Sign > 0)
			{
				return AndNotNegativePositive(val, that);
			}
			return AndNotNegative(val, that);
		}

		private static BigInteger AndNotPositive(BigInteger val, BigInteger that)
		{
			int[] array = new int[val.numberLength];
			int num = System.Math.Min(val.numberLength, that.numberLength);
			int i;
			for (i = val.FirstNonzeroDigit; i < num; i++)
			{
				array[i] = val.Digits[i] & ~that.Digits[i];
			}
			for (; i < val.numberLength; i++)
			{
				array[i] = val.Digits[i];
			}
			BigInteger bigInteger = new BigInteger(1, val.numberLength, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		private static BigInteger AndNotPositiveNegative(BigInteger positive, BigInteger negative)
		{
			int firstNonzeroDigit = negative.FirstNonzeroDigit;
			int firstNonzeroDigit2 = positive.FirstNonzeroDigit;
			if (firstNonzeroDigit >= positive.numberLength)
			{
				return positive;
			}
			int num = System.Math.Min(positive.numberLength, negative.numberLength);
			int[] array = new int[num];
			int i;
			for (i = firstNonzeroDigit2; i < firstNonzeroDigit; i++)
			{
				array[i] = positive.Digits[i];
			}
			if (i == firstNonzeroDigit)
			{
				array[i] = positive.Digits[i] & (negative.Digits[i] - 1);
				i++;
			}
			for (; i < num; i++)
			{
				array[i] = positive.Digits[i] & negative.Digits[i];
			}
			BigInteger bigInteger = new BigInteger(1, num, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		private static BigInteger AndNotNegativePositive(BigInteger negative, BigInteger positive)
		{
			int firstNonzeroDigit = negative.FirstNonzeroDigit;
			int firstNonzeroDigit2 = positive.FirstNonzeroDigit;
			if (firstNonzeroDigit >= positive.numberLength)
			{
				return negative;
			}
			int num = System.Math.Max(negative.numberLength, positive.numberLength);
			int i = firstNonzeroDigit;
			int[] array;
			if (firstNonzeroDigit2 > firstNonzeroDigit)
			{
				array = new int[num];
				for (int num2 = System.Math.Min(negative.numberLength, firstNonzeroDigit2); i < num2; i++)
				{
					array[i] = negative.Digits[i];
				}
				if (i == negative.numberLength)
				{
					for (i = firstNonzeroDigit2; i < positive.numberLength; i++)
					{
						array[i] = positive.Digits[i];
					}
				}
			}
			else
			{
				int num3 = -negative.Digits[i] & ~positive.Digits[i];
				if (num3 == 0)
				{
					int num2 = System.Math.Min(positive.numberLength, negative.numberLength);
					for (i++; i < num2; i++)
					{
						if ((num3 = ~(negative.Digits[i] | positive.Digits[i])) != 0)
						{
							break;
						}
					}
					if (num3 == 0)
					{
						for (; i < positive.numberLength; i++)
						{
							if ((num3 = ~positive.Digits[i]) != 0)
							{
								break;
							}
						}
						for (; i < negative.numberLength; i++)
						{
							if ((num3 = ~negative.Digits[i]) != 0)
							{
								break;
							}
						}
						if (num3 == 0)
						{
							num++;
							array = new int[num];
							array[num - 1] = 1;
							return new BigInteger(-1, num, array);
						}
					}
				}
				array = new int[num];
				array[i] = -num3;
				i++;
			}
			for (int num2 = System.Math.Min(positive.numberLength, negative.numberLength); i < num2; i++)
			{
				array[i] = negative.Digits[i] | positive.Digits[i];
			}
			for (; i < negative.numberLength; i++)
			{
				array[i] = negative.Digits[i];
			}
			for (; i < positive.numberLength; i++)
			{
				array[i] = positive.Digits[i];
			}
			return new BigInteger(-1, num, array);
		}

		private static BigInteger AndNotNegative(BigInteger val, BigInteger that)
		{
			int firstNonzeroDigit = val.FirstNonzeroDigit;
			int firstNonzeroDigit2 = that.FirstNonzeroDigit;
			if (firstNonzeroDigit >= that.numberLength)
			{
				return BigInteger.Zero;
			}
			int numberLength = that.numberLength;
			int[] array = new int[numberLength];
			int i = firstNonzeroDigit;
			int num;
			if (firstNonzeroDigit < firstNonzeroDigit2)
			{
				array[i] = -val.Digits[i];
				num = System.Math.Min(val.numberLength, firstNonzeroDigit2);
				for (i++; i < num; i++)
				{
					array[i] = ~val.Digits[i];
				}
				if (i == val.numberLength)
				{
					for (; i < firstNonzeroDigit2; i++)
					{
						array[i] = -1;
					}
					array[i] = that.Digits[i] - 1;
				}
				else
				{
					array[i] = ~val.Digits[i] & (that.Digits[i] - 1);
				}
			}
			else if (firstNonzeroDigit2 < firstNonzeroDigit)
			{
				array[i] = -val.Digits[i] & that.Digits[i];
			}
			else
			{
				array[i] = -val.Digits[i] & (that.Digits[i] - 1);
			}
			num = System.Math.Min(val.numberLength, that.numberLength);
			for (i++; i < num; i++)
			{
				array[i] = ~val.Digits[i] & that.Digits[i];
			}
			for (; i < that.numberLength; i++)
			{
				array[i] = that.Digits[i];
			}
			BigInteger bigInteger = new BigInteger(1, numberLength, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		public static BigInteger Or(BigInteger val, BigInteger that)
		{
			if (that.Equals(BigInteger.MinusOne) || val.Equals(BigInteger.MinusOne))
			{
				return BigInteger.MinusOne;
			}
			if (that.Sign == 0)
			{
				return val;
			}
			if (val.Sign == 0)
			{
				return that;
			}
			if (val.Sign > 0)
			{
				if (that.Sign > 0)
				{
					if (val.numberLength > that.numberLength)
					{
						return OrPositive(val, that);
					}
					return OrPositive(that, val);
				}
				return OrDiffSigns(val, that);
			}
			if (that.Sign > 0)
			{
				return OrDiffSigns(that, val);
			}
			if (that.FirstNonzeroDigit > val.FirstNonzeroDigit)
			{
				return OrNegative(that, val);
			}
			return OrNegative(val, that);
		}

		private static BigInteger OrPositive(BigInteger longer, BigInteger shorter)
		{
			int numberLength = longer.numberLength;
			int[] array = new int[numberLength];
			int num = System.Math.Min(longer.FirstNonzeroDigit, shorter.FirstNonzeroDigit);
			for (num = 0; num < shorter.numberLength; num++)
			{
				array[num] = longer.Digits[num] | shorter.Digits[num];
			}
			for (; num < numberLength; num++)
			{
				array[num] = longer.Digits[num];
			}
			return new BigInteger(1, numberLength, array);
		}

		private static BigInteger OrNegative(BigInteger val, BigInteger that)
		{
			int firstNonzeroDigit = that.FirstNonzeroDigit;
			int firstNonzeroDigit2 = val.FirstNonzeroDigit;
			if (firstNonzeroDigit2 >= that.numberLength)
			{
				return that;
			}
			if (firstNonzeroDigit >= val.numberLength)
			{
				return val;
			}
			int num = System.Math.Min(val.numberLength, that.numberLength);
			int[] array = new int[num];
			int i;
			if (firstNonzeroDigit == firstNonzeroDigit2)
			{
				array[firstNonzeroDigit2] = -(-val.Digits[firstNonzeroDigit2] | -that.Digits[firstNonzeroDigit2]);
				i = firstNonzeroDigit2;
			}
			else
			{
				for (i = firstNonzeroDigit; i < firstNonzeroDigit2; i++)
				{
					array[i] = that.Digits[i];
				}
				array[i] = that.Digits[i] & (val.Digits[i] - 1);
			}
			for (i++; i < num; i++)
			{
				array[i] = val.Digits[i] & that.Digits[i];
			}
			BigInteger bigInteger = new BigInteger(-1, num, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		private static BigInteger OrDiffSigns(BigInteger positive, BigInteger negative)
		{
			int firstNonzeroDigit = negative.FirstNonzeroDigit;
			int firstNonzeroDigit2 = positive.FirstNonzeroDigit;
			if (firstNonzeroDigit2 >= negative.numberLength)
			{
				return negative;
			}
			int numberLength = negative.numberLength;
			int[] array = new int[numberLength];
			int i;
			if (firstNonzeroDigit < firstNonzeroDigit2)
			{
				for (i = firstNonzeroDigit; i < firstNonzeroDigit2; i++)
				{
					array[i] = negative.Digits[i];
				}
			}
			else if (firstNonzeroDigit2 < firstNonzeroDigit)
			{
				i = firstNonzeroDigit2;
				array[i] = -positive.Digits[i];
				int num = System.Math.Min(positive.numberLength, firstNonzeroDigit);
				for (i++; i < num; i++)
				{
					array[i] = ~positive.Digits[i];
				}
				if (i != positive.numberLength)
				{
					array[i] = ~(-negative.Digits[i] | positive.Digits[i]);
				}
				else
				{
					for (; i < firstNonzeroDigit; i++)
					{
						array[i] = -1;
					}
					array[i] = negative.Digits[i] - 1;
				}
				i++;
			}
			else
			{
				i = firstNonzeroDigit2;
				array[i] = -(-negative.Digits[i] | positive.Digits[i]);
				i++;
			}
			for (int num = System.Math.Min(negative.numberLength, positive.numberLength); i < num; i++)
			{
				array[i] = negative.Digits[i] & ~positive.Digits[i];
			}
			for (; i < negative.numberLength; i++)
			{
				array[i] = negative.Digits[i];
			}
			BigInteger bigInteger = new BigInteger(-1, numberLength, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		public static BigInteger Xor(BigInteger val, BigInteger that)
		{
			if (that.Sign == 0)
			{
				return val;
			}
			if (val.Sign == 0)
			{
				return that;
			}
			if (that.Equals(BigInteger.MinusOne))
			{
				return val.Not();
			}
			if (val.Equals(BigInteger.MinusOne))
			{
				return that.Not();
			}
			if (val.Sign > 0)
			{
				if (that.Sign > 0)
				{
					if (val.numberLength > that.numberLength)
					{
						return XorPositive(val, that);
					}
					return XorPositive(that, val);
				}
				return XorDiffSigns(val, that);
			}
			if (that.Sign > 0)
			{
				return XorDiffSigns(that, val);
			}
			if (that.FirstNonzeroDigit > val.FirstNonzeroDigit)
			{
				return XorNegative(that, val);
			}
			return XorNegative(val, that);
		}

		private static BigInteger XorPositive(BigInteger longer, BigInteger shorter)
		{
			int numberLength = longer.numberLength;
			int[] array = new int[numberLength];
			int i;
			for (i = System.Math.Min(longer.FirstNonzeroDigit, shorter.FirstNonzeroDigit); i < shorter.numberLength; i++)
			{
				array[i] = longer.Digits[i] ^ shorter.Digits[i];
			}
			for (; i < longer.numberLength; i++)
			{
				array[i] = longer.Digits[i];
			}
			BigInteger bigInteger = new BigInteger(1, numberLength, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		private static BigInteger XorNegative(BigInteger val, BigInteger that)
		{
			int num = System.Math.Max(val.numberLength, that.numberLength);
			int[] array = new int[num];
			int firstNonzeroDigit = val.FirstNonzeroDigit;
			int firstNonzeroDigit2 = that.FirstNonzeroDigit;
			int i = firstNonzeroDigit2;
			int num2;
			if (firstNonzeroDigit == firstNonzeroDigit2)
			{
				array[i] = -val.Digits[i] ^ -that.Digits[i];
			}
			else
			{
				array[i] = -that.Digits[i];
				num2 = System.Math.Min(that.numberLength, firstNonzeroDigit);
				for (i++; i < num2; i++)
				{
					array[i] = ~that.Digits[i];
				}
				if (i == that.numberLength)
				{
					for (; i < firstNonzeroDigit; i++)
					{
						array[i] = -1;
					}
					array[i] = val.Digits[i] - 1;
				}
				else
				{
					array[i] = -val.Digits[i] ^ ~that.Digits[i];
				}
			}
			num2 = System.Math.Min(val.numberLength, that.numberLength);
			for (i++; i < num2; i++)
			{
				array[i] = val.Digits[i] ^ that.Digits[i];
			}
			for (; i < val.numberLength; i++)
			{
				array[i] = val.Digits[i];
			}
			for (; i < that.numberLength; i++)
			{
				array[i] = that.Digits[i];
			}
			BigInteger bigInteger = new BigInteger(1, num, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		private static BigInteger XorDiffSigns(BigInteger positive, BigInteger negative)
		{
			int num = System.Math.Max(negative.numberLength, positive.numberLength);
			int firstNonzeroDigit = negative.FirstNonzeroDigit;
			int firstNonzeroDigit2 = positive.FirstNonzeroDigit;
			int num2;
			int[] array;
			if (firstNonzeroDigit < firstNonzeroDigit2)
			{
				array = new int[num];
				num2 = firstNonzeroDigit;
				array[num2] = negative.Digits[num2];
				int num3 = System.Math.Min(negative.numberLength, firstNonzeroDigit2);
				for (num2++; num2 < num3; num2++)
				{
					array[num2] = negative.Digits[num2];
				}
				if (num2 == negative.numberLength)
				{
					for (; num2 < positive.numberLength; num2++)
					{
						array[num2] = positive.Digits[num2];
					}
				}
			}
			else if (firstNonzeroDigit2 < firstNonzeroDigit)
			{
				array = new int[num];
				num2 = firstNonzeroDigit2;
				array[num2] = -positive.Digits[num2];
				int num3 = System.Math.Min(positive.numberLength, firstNonzeroDigit);
				for (num2++; num2 < num3; num2++)
				{
					array[num2] = ~positive.Digits[num2];
				}
				if (num2 == firstNonzeroDigit)
				{
					array[num2] = ~(positive.Digits[num2] ^ -negative.Digits[num2]);
					num2++;
				}
				else
				{
					for (; num2 < firstNonzeroDigit; num2++)
					{
						array[num2] = -1;
					}
					for (; num2 < negative.numberLength; num2++)
					{
						array[num2] = negative.Digits[num2];
					}
				}
			}
			else
			{
				num2 = firstNonzeroDigit;
				int num4 = positive.Digits[num2] ^ -negative.Digits[num2];
				if (num4 == 0)
				{
					int num3 = System.Math.Min(positive.numberLength, negative.numberLength);
					for (num2++; num2 < num3; num2++)
					{
						if ((num4 = positive.Digits[num2] ^ ~negative.Digits[num2]) != 0)
						{
							break;
						}
					}
					if (num4 == 0)
					{
						for (; num2 < positive.numberLength; num2++)
						{
							if ((num4 = ~positive.Digits[num2]) != 0)
							{
								break;
							}
						}
						for (; num2 < negative.numberLength; num2++)
						{
							if ((num4 = ~negative.Digits[num2]) != 0)
							{
								break;
							}
						}
						if (num4 == 0)
						{
							num++;
							array = new int[num];
							array[num - 1] = 1;
							return new BigInteger(-1, num, array);
						}
					}
				}
				array = new int[num];
				array[num2] = -num4;
				num2++;
			}
			for (int num3 = System.Math.Min(negative.numberLength, positive.numberLength); num2 < num3; num2++)
			{
				array[num2] = ~(~negative.Digits[num2] ^ positive.Digits[num2]);
			}
			for (; num2 < positive.numberLength; num2++)
			{
				array[num2] = positive.Digits[num2];
			}
			for (; num2 < negative.numberLength; num2++)
			{
				array[num2] = negative.Digits[num2];
			}
			BigInteger bigInteger = new BigInteger(-1, num, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}
	}
}
