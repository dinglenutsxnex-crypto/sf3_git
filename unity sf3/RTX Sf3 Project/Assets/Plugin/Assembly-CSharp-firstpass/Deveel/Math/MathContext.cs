using System;
using System.Runtime.Serialization;
using System.Text;

namespace Deveel.Math
{
	[Serializable]
	public sealed class MathContext : IEquatable<MathContext>
	{
		public static readonly MathContext Decimal128 = new MathContext(34, RoundingMode.HalfEven);

		public static readonly MathContext Decimal32 = new MathContext(7, RoundingMode.HalfEven);

		public static readonly MathContext Decimal64 = new MathContext(16, RoundingMode.HalfEven);

		public static readonly MathContext Unlimited = new MathContext(0, RoundingMode.HalfUp);

		private readonly int precision;

		private readonly RoundingMode roundingMode;

		private static readonly char[] chPrecision = new char[10] { 'p', 'r', 'e', 'c', 'i', 's', 'i', 'o', 'n', '=' };

		private static readonly char[] chRoundingMode = new char[13]
		{
			'r', 'o', 'u', 'n', 'd', 'i', 'n', 'g', 'M', 'o',
			'd', 'e', '='
		};

		public int Precision
		{
			get
			{
				return precision;
			}
		}

		public RoundingMode RoundingMode
		{
			get
			{
				return roundingMode;
			}
		}

		public MathContext(int precision)
			: this(precision, RoundingMode.HalfUp)
		{
		}

		public MathContext(int precision, RoundingMode roundingMode)
		{
			if (precision < 0)
			{
				throw new ArgumentException("Digits < 0");
			}
			this.precision = precision;
			this.roundingMode = roundingMode;
		}

		public bool Equals(MathContext other)
		{
			return other.precision == precision && other.roundingMode == roundingMode;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is MathContext))
			{
				return false;
			}
			return Equals((MathContext)obj);
		}

		public override int GetHashCode()
		{
			return (precision << 3) | (int)roundingMode;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(45);
			stringBuilder.Append(chPrecision);
			stringBuilder.Append(precision);
			stringBuilder.Append(' ');
			stringBuilder.Append(chRoundingMode);
			stringBuilder.Append(roundingMode);
			return stringBuilder.ToString();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (precision < 0)
			{
				throw new SerializationException("bad precision value");
			}
		}

		private static bool TryParse(string s, out MathContext context, out Exception exception)
		{
			char[] array = s.ToCharArray();
			if (array.Length < 27 || array.Length > 45)
			{
				exception = new FormatException("bad string format");
				context = null;
				return false;
			}
			int i;
			for (i = 0; i < chPrecision.Length && array[i] == chPrecision[i]; i++)
			{
			}
			if (i < chPrecision.Length)
			{
				throw new FormatException("bad string format");
			}
			int num = CharHelper.toDigit(array[i], 10);
			if (num == -1)
			{
				exception = new FormatException("bad string format");
				context = null;
				return false;
			}
			int num2 = num;
			i++;
			int j;
			while (true)
			{
				num = CharHelper.toDigit(array[i], 10);
				if (num == -1)
				{
					if (array[i] == ' ')
					{
						i++;
						j = 0;
						break;
					}
					exception = new ArgumentException("bad string format");
					context = null;
					return false;
				}
				num2 = num2 * 10 + num;
				if (num2 < 0)
				{
					exception = new ArgumentException("bad string format");
					context = null;
					return false;
				}
				i++;
			}
			for (; j < chRoundingMode.Length && array[i] == chRoundingMode[j]; j++)
			{
				i++;
			}
			if (j < chRoundingMode.Length)
			{
				throw new FormatException("bad string format");
			}
			RoundingMode roundingMode;
			try
			{
				roundingMode = (RoundingMode)Enum.Parse(typeof(RoundingMode), new string(array, i, array.Length - i), true);
			}
			catch (Exception ex)
			{
				exception = ex;
				context = null;
				return false;
			}
			exception = null;
			context = new MathContext(num2, roundingMode);
			return true;
		}

		public static bool TryParse(string s, out MathContext context)
		{
			Exception exception;
			return TryParse(s, out context, out exception);
		}

		public static MathContext Parse(string s)
		{
			MathContext context;
			Exception exception;
			if (!TryParse(s, out context, out exception))
			{
				throw exception;
			}
			return context;
		}
	}
}
