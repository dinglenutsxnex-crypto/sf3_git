using System;

namespace Deveel.Math
{
	internal sealed class CharHelper
	{
		public const int MIN_RADIX = 2;

		public const int MAX_RADIX = 36;

		private CharHelper()
		{
		}

		public static char forDigit(int digit, int radix)
		{
			if (radix < 2 || radix > 36)
			{
				throw new ArgumentOutOfRangeException("radix");
			}
			if (digit < 0 || digit >= radix)
			{
				throw new ArgumentOutOfRangeException("digit");
			}
			if (digit < 10)
			{
				return (char)(digit + 48);
			}
			return (char)(digit - 10 + 97);
		}

		public static int toDigit(char ch, int radix)
		{
			if (radix < 2 || radix > 36)
			{
				return -1;
			}
			int num = -1;
			ch = char.ToLowerInvariant(ch);
			if (ch >= '0' && ch <= '9')
			{
				num = ch - 48;
			}
			else if (ch >= 'a' && ch <= 'z')
			{
				num = ch - 97 + 10;
			}
			return (num >= radix) ? (-1) : num;
		}
	}
}
