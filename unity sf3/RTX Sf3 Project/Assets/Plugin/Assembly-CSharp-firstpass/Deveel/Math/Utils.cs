namespace Deveel.Math
{
	internal static class Utils
	{
		public static int URShift(int number, int bits)
		{
			if (number >= 0)
			{
				return number >> bits;
			}
			return (number >> bits) + (2 << ~bits);
		}

		public static int URShift(int number, long bits)
		{
			return URShift(number, (int)bits);
		}

		public static long URShift(long number, int bits)
		{
			if (number >= 0)
			{
				return number >> bits;
			}
			return (number >> bits) + (2L << ~bits);
		}

		public static long URShift(long number, long bits)
		{
			return URShift(number, (int)bits);
		}

		public static int NumberOfLeadingZeros(int value)
		{
			value |= URShift(value, 1);
			value |= URShift(value, 2);
			value |= URShift(value, 4);
			value |= URShift(value, 8);
			value |= URShift(value, 16);
			return BitCount(~value);
		}

		public static int NumberOfLeadingZeros(long value)
		{
			value |= URShift(value, 1);
			value |= URShift(value, 2);
			value |= URShift(value, 4);
			value |= URShift(value, 8);
			value |= URShift(value, 16);
			value |= URShift(value, 32);
			return BitCount(~value);
		}

		public static int NumberOfTrailingZeros(int value)
		{
			return BitCount((value & -value) - 1);
		}

		public static int NumberOfTrailingZeros(long value)
		{
			return BitCount((value & -value) - 1);
		}

		public static int BitCount(int x)
		{
			x = ((x >> 1) & 0x55555555) + (x & 0x55555555);
			x = ((x >> 2) & 0x33333333) + (x & 0x33333333);
			x = ((x >> 4) & 0xF0F0F0F) + (x & 0xF0F0F0F);
			x = ((x >> 8) & 0xFF00FF) + (x & 0xFF00FF);
			return ((x >> 16) & 0xFFFF) + (x & 0xFFFF);
		}

		public static int BitCount(long x)
		{
			x = ((x >> 1) & 0x5555555555555555L) + (x & 0x5555555555555555L);
			x = ((x >> 2) & 0x3333333333333333L) + (x & 0x3333333333333333L);
			int num = (int)(URShift(x, 32) + x);
			num = ((num >> 4) & 0xF0F0F0F) + (num & 0xF0F0F0F);
			num = ((num >> 8) & 0xFF00FF) + (num & 0xFF00FF);
			return ((num >> 16) & 0xFFFF) + (num & 0xFFFF);
		}

		public static int HighestOneBit(int value)
		{
			value |= URShift(value, 1);
			value |= URShift(value, 2);
			value |= URShift(value, 4);
			value |= URShift(value, 8);
			value |= URShift(value, 16);
			return value ^ URShift(value, 1);
		}

		public static long HighestOneBit(long value)
		{
			value |= URShift(value, 1);
			value |= URShift(value, 2);
			value |= URShift(value, 4);
			value |= URShift(value, 8);
			value |= URShift(value, 16);
			value |= URShift(value, 32);
			return value ^ URShift(value, 1);
		}

		public static long DoubleToLong(double d)
		{
			if (d >= 9.223372036854776E+18)
			{
				return long.MaxValue;
			}
			if (d <= -9.223372036854776E+18)
			{
				return long.MinValue;
			}
			return (long)d;
		}
	}
}
