using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Deveel.Math
{
	[Serializable]
	[DebuggerDisplay("{ToString()}")]
	public sealed class BigDecimal : IComparable<BigDecimal>, IEquatable<BigDecimal>, ISerializable, IConvertible
	{
		public static readonly BigDecimal Zero;

		public static readonly BigDecimal One;

		public static readonly BigDecimal Ten;

		private const double Log10Of2 = 0.3010299956639812;

		[NonSerialized]
		private string toStringImage;

		private static readonly BigInteger[] FivePow;

		private static readonly BigInteger[] TenPow;

		private static readonly long[] LongTenPow;

		private static readonly long[] LongFivePow;

		private static readonly int[] LongFivePowBitLength;

		private static readonly int[] LongTenPowBitLength;

		private const int BiScaledByZeroLength = 11;

		private static readonly BigDecimal[] BiScaledByZero;

		private static readonly BigDecimal[] ZeroScaledBy;

		private static readonly char[] ChZeros;

		private BigInteger intVal;

		[NonSerialized]
		private int _bitLength;

		[NonSerialized]
		private long smallValue;

		private int _scale;

		[NonSerialized]
		private int _precision;

		public int Sign
		{
			get
			{
				if (_bitLength < 64)
				{
					return System.Math.Sign(smallValue);
				}
				return GetUnscaledValue().Sign;
			}
		}

		public bool IsZero
		{
			get
			{
				return _bitLength == 0 && smallValue != -1;
			}
		}

		public int Scale
		{
			get
			{
				return _scale;
			}
		}

		public int Precision
		{
			get
			{
				if (_precision > 0)
				{
					return _precision;
				}
				int bitLength = _bitLength;
				int num = 1;
				double value = 1.0;
				if (bitLength < 1024)
				{
					if (bitLength >= 64)
					{
						value = GetUnscaledValue().ToDouble();
					}
					else if (bitLength >= 1)
					{
						value = smallValue;
					}
					num += (int)System.Math.Log10(System.Math.Abs(value));
				}
				else
				{
					num += (int)((double)(bitLength - 1) * 0.3010299956639812);
					if (GetUnscaledValue().Divide(Multiplication.PowerOf10(num)).Sign != 0)
					{
						num++;
					}
				}
				_precision = num;
				return _precision;
			}
		}

		public BigInteger UnscaledValue
		{
			get
			{
				return GetUnscaledValue();
			}
		}

		static BigDecimal()
		{
			Zero = new BigDecimal(0, 0);
			One = new BigDecimal(1, 0);
			Ten = new BigDecimal(10, 0);
			LongTenPow = new long[19]
			{
				1L, 10L, 100L, 1000L, 10000L, 100000L, 1000000L, 10000000L, 100000000L, 1000000000L,
				10000000000L, 100000000000L, 1000000000000L, 10000000000000L, 100000000000000L, 1000000000000000L, 10000000000000000L, 100000000000000000L, 1000000000000000000L
			};
			LongFivePow = new long[28]
			{
				1L, 5L, 25L, 125L, 625L, 3125L, 15625L, 78125L, 390625L, 1953125L,
				9765625L, 48828125L, 244140625L, 1220703125L, 6103515625L, 30517578125L, 152587890625L, 762939453125L, 3814697265625L, 19073486328125L,
				95367431640625L, 476837158203125L, 2384185791015625L, 11920928955078125L, 59604644775390625L, 298023223876953125L, 1490116119384765625L, 7450580596923828125L
			};
			LongFivePowBitLength = new int[LongFivePow.Length];
			LongTenPowBitLength = new int[LongTenPow.Length];
			BiScaledByZero = new BigDecimal[11];
			ZeroScaledBy = new BigDecimal[11];
			ChZeros = new char[100];
			int i;
			for (i = 0; i < ZeroScaledBy.Length; i++)
			{
				BiScaledByZero[i] = new BigDecimal(i, 0);
				ZeroScaledBy[i] = new BigDecimal(0, i);
				ChZeros[i] = '0';
			}
			for (; i < ChZeros.Length; i++)
			{
				ChZeros[i] = '0';
			}
			for (int j = 0; j < LongFivePowBitLength.Length; j++)
			{
				LongFivePowBitLength[j] = BitLength(LongFivePow[j]);
			}
			for (int k = 0; k < LongTenPowBitLength.Length; k++)
			{
				LongTenPowBitLength[k] = BitLength(LongTenPow[k]);
			}
			TenPow = Multiplication.BigTenPows;
			FivePow = Multiplication.BigFivePows;
		}

		private BigDecimal(long smallValue, int scale)
		{
			this.smallValue = smallValue;
			_scale = scale;
			_bitLength = BitLength(smallValue);
		}

		private BigDecimal(int smallValue, int scale)
		{
			this.smallValue = smallValue;
			_scale = scale;
			_bitLength = BitLength(smallValue);
		}

		private BigDecimal()
		{
		}

		public BigDecimal(double val)
		{
			if (double.IsInfinity(val) || double.IsNaN(val))
			{
				throw new FormatException("Infinite or NaN");
			}
			long num = BitConverter.DoubleToInt64Bits(val);
			_scale = 1075 - (int)((num >> 52) & 0x7FF);
			long num2 = ((_scale != 1075) ? ((num & 0xFFFFFFFFFFFFFL) | 0x10000000000000L) : ((num & 0xFFFFFFFFFFFFFL) << 1));
			if (num2 == 0)
			{
				_scale = 0;
				_precision = 1;
			}
			if (_scale > 0)
			{
				int num3 = System.Math.Min(_scale, Utils.NumberOfTrailingZeros(num2));
				num2 = Utils.URShift(num2, num3);
				_scale -= num3;
			}
			if (num >> 63 != 0)
			{
				num2 = -num2;
			}
			int num4 = BitLength(num2);
			if (_scale < 0)
			{
				_bitLength = ((num4 != 0) ? (num4 - _scale) : 0);
				if (_bitLength < 64)
				{
					smallValue = num2 << -_scale;
				}
				else
				{
					intVal = BigInteger.ValueOf(num2).ShiftLeft(-_scale);
				}
				_scale = 0;
			}
			else if (_scale > 0)
			{
				if (_scale < LongFivePow.Length && num4 + LongFivePowBitLength[_scale] < 64)
				{
					smallValue = num2 * LongFivePow[_scale];
					_bitLength = BitLength(smallValue);
				}
				else
				{
					SetUnscaledValue(Multiplication.MultiplyByFivePow(BigInteger.ValueOf(num2), _scale));
				}
			}
			else
			{
				smallValue = num2;
				_bitLength = num4;
			}
		}

		public BigDecimal(double val, MathContext mc)
			: this(val)
		{
			InplaceRound(mc);
		}

		public BigDecimal(BigInteger val)
			: this(val, 0)
		{
		}

		public BigDecimal(BigInteger val, MathContext mc)
			: this(val)
		{
			InplaceRound(mc);
		}

		public BigDecimal(BigInteger unscaledValue, int scale)
		{
			if (unscaledValue == null)
			{
				throw new NullReferenceException();
			}
			_scale = scale;
			SetUnscaledValue(unscaledValue);
		}

		public BigDecimal(BigInteger unscaledValue, int scale, MathContext mc)
			: this(unscaledValue, scale)
		{
			InplaceRound(mc);
		}

		public BigDecimal(int val)
			: this(val, 0)
		{
		}

		public BigDecimal(int val, MathContext mc)
			: this(val, 0)
		{
			InplaceRound(mc);
		}

		public BigDecimal(long val)
			: this(val, 0)
		{
		}

		public BigDecimal(long val, MathContext mc)
			: this(val)
		{
			InplaceRound(mc);
		}

		private BigDecimal(SerializationInfo info, StreamingContext context)
		{
			intVal = (BigInteger)info.GetValue("intVal", typeof(BigInteger));
			_scale = info.GetInt32("scale");
			_bitLength = intVal.BitLength;
			if (_bitLength < 64)
			{
				smallValue = intVal.ToInt64();
			}
		}

		public static BigDecimal ValueOf(long unscaledVal, int scale)
		{
			if (scale == 0)
			{
				return ValueOf(unscaledVal);
			}
			if (unscaledVal == 0 && scale >= 0 && scale < ZeroScaledBy.Length)
			{
				return ZeroScaledBy[scale];
			}
			return new BigDecimal(unscaledVal, scale);
		}

		public static BigDecimal ValueOf(long unscaledVal)
		{
			if (unscaledVal >= 0 && unscaledVal < 11)
			{
				return BiScaledByZero[(int)unscaledVal];
			}
			return new BigDecimal(unscaledVal, 0);
		}

		public BigDecimal Add(BigDecimal augend)
		{
			int num = _scale - augend._scale;
			if (IsZero)
			{
				if (num <= 0)
				{
					return augend;
				}
				if (augend.IsZero)
				{
					return this;
				}
			}
			else if (augend.IsZero && num >= 0)
			{
				return this;
			}
			if (num == 0)
			{
				if (System.Math.Max(_bitLength, augend._bitLength) + 1 < 64)
				{
					return ValueOf(smallValue + augend.smallValue, _scale);
				}
				return new BigDecimal(GetUnscaledValue().Add(augend.GetUnscaledValue()), _scale);
			}
			if (num > 0)
			{
				return AddAndMult10(this, augend, num);
			}
			return AddAndMult10(augend, this, -num);
		}

		private static BigDecimal AddAndMult10(BigDecimal thisValue, BigDecimal augend, int diffScale)
		{
			if (diffScale < LongTenPow.Length && System.Math.Max(thisValue._bitLength, augend._bitLength + LongTenPowBitLength[diffScale]) + 1 < 64)
			{
				return ValueOf(thisValue.smallValue + augend.smallValue * LongTenPow[diffScale], thisValue._scale);
			}
			return new BigDecimal(thisValue.GetUnscaledValue().Add(Multiplication.MultiplyByTenPow(augend.GetUnscaledValue(), diffScale)), thisValue._scale);
		}

		public BigDecimal Add(BigDecimal augend, MathContext mc)
		{
			long num = (long)_scale - (long)augend._scale;
			if (augend.IsZero || IsZero || mc.Precision == 0)
			{
				return Add(augend).Round(mc);
			}
			BigDecimal bigDecimal2;
			BigDecimal bigDecimal;
			if (AproxPrecision() < num - 1)
			{
				bigDecimal = augend;
				bigDecimal2 = this;
			}
			else
			{
				if (augend.AproxPrecision() >= -num - 1)
				{
					return Add(augend).Round(mc);
				}
				bigDecimal = this;
				bigDecimal2 = augend;
			}
			if (mc.Precision >= bigDecimal.AproxPrecision())
			{
				return Add(augend).Round(mc);
			}
			int sign = bigDecimal.Sign;
			BigInteger unscaledValue;
			if (sign == bigDecimal2.Sign)
			{
				unscaledValue = Multiplication.MultiplyByPositiveInt(bigDecimal.GetUnscaledValue(), 10).Add(BigInteger.ValueOf(sign));
			}
			else
			{
				unscaledValue = bigDecimal.GetUnscaledValue().Subtract(BigInteger.ValueOf(sign));
				unscaledValue = Multiplication.MultiplyByPositiveInt(unscaledValue, 10).Add(BigInteger.ValueOf(sign * 9));
			}
			bigDecimal = new BigDecimal(unscaledValue, bigDecimal._scale + 1);
			return bigDecimal.Round(mc);
		}

		public BigDecimal Subtract(BigDecimal subtrahend)
		{
			if (subtrahend == null)
			{
				throw new ArgumentNullException("subtrahend");
			}
			int num = _scale - subtrahend._scale;
			if (IsZero)
			{
				if (num <= 0)
				{
					return subtrahend.Negate();
				}
				if (subtrahend.IsZero)
				{
					return this;
				}
			}
			else if (subtrahend.IsZero && num >= 0)
			{
				return this;
			}
			if (num == 0)
			{
				if (System.Math.Max(_bitLength, subtrahend._bitLength) + 1 < 64)
				{
					return ValueOf(smallValue - subtrahend.smallValue, _scale);
				}
				return new BigDecimal(GetUnscaledValue().Subtract(subtrahend.GetUnscaledValue()), _scale);
			}
			if (num > 0)
			{
				if (num < LongTenPow.Length && System.Math.Max(_bitLength, subtrahend._bitLength + LongTenPowBitLength[num]) + 1 < 64)
				{
					return ValueOf(smallValue - subtrahend.smallValue * LongTenPow[num], _scale);
				}
				return new BigDecimal(GetUnscaledValue().Subtract(Multiplication.MultiplyByTenPow(subtrahend.GetUnscaledValue(), num)), _scale);
			}
			num = -num;
			if (num < LongTenPow.Length && System.Math.Max(_bitLength + LongTenPowBitLength[num], subtrahend._bitLength) + 1 < 64)
			{
				return ValueOf(smallValue * LongTenPow[num] - subtrahend.smallValue, subtrahend._scale);
			}
			return new BigDecimal(Multiplication.MultiplyByTenPow(GetUnscaledValue(), num).Subtract(subtrahend.GetUnscaledValue()), subtrahend._scale);
		}

		public BigDecimal Subtract(BigDecimal subtrahend, MathContext mc)
		{
			if (subtrahend == null)
			{
				throw new ArgumentNullException("subtrahend");
			}
			if (mc == null)
			{
				throw new ArgumentNullException("mc");
			}
			long num = (long)subtrahend._scale - (long)_scale;
			if (subtrahend.IsZero || IsZero || mc.Precision == 0)
			{
				return Subtract(subtrahend).Round(mc);
			}
			if (subtrahend.AproxPrecision() < num - 1 && mc.Precision < AproxPrecision())
			{
				int sign = Sign;
				BigInteger unscaledValue;
				if (sign != subtrahend.Sign)
				{
					unscaledValue = Multiplication.MultiplyByPositiveInt(GetUnscaledValue(), 10).Add(BigInteger.ValueOf(sign));
				}
				else
				{
					unscaledValue = GetUnscaledValue().Subtract(BigInteger.ValueOf(sign));
					unscaledValue = Multiplication.MultiplyByPositiveInt(unscaledValue, 10).Add(BigInteger.ValueOf(sign * 9));
				}
				BigDecimal bigDecimal = new BigDecimal(unscaledValue, _scale + 1);
				return bigDecimal.Round(mc);
			}
			return Subtract(subtrahend).Round(mc);
		}

		public BigDecimal Multiply(BigDecimal multiplicand)
		{
			long longScale = (long)_scale + (long)multiplicand._scale;
			if (IsZero || multiplicand.IsZero)
			{
				return GetZeroScaledBy(longScale);
			}
			if (_bitLength + multiplicand._bitLength < 64)
			{
				return ValueOf(smallValue * multiplicand.smallValue, ToIntScale(longScale));
			}
			return new BigDecimal(GetUnscaledValue().Multiply(multiplicand.GetUnscaledValue()), ToIntScale(longScale));
		}

		public BigDecimal Multiply(BigDecimal multiplicand, MathContext mc)
		{
			BigDecimal bigDecimal = Multiply(multiplicand);
			bigDecimal.InplaceRound(mc);
			return bigDecimal;
		}

		public BigDecimal Divide(BigDecimal divisor, int scale, RoundingMode roundingMode)
		{
			if (divisor.IsZero)
			{
				throw new ArithmeticException("Division by zero");
			}
			long num = (long)_scale - (long)divisor._scale - scale;
			if (_bitLength < 64 && divisor._bitLength < 64)
			{
				if (num == 0)
				{
					return DividePrimitiveLongs(smallValue, divisor.smallValue, scale, roundingMode);
				}
				if (num > 0)
				{
					if (num < LongTenPow.Length && divisor._bitLength + LongTenPowBitLength[(int)num] < 64)
					{
						return DividePrimitiveLongs(smallValue, divisor.smallValue * LongTenPow[(int)num], scale, roundingMode);
					}
				}
				else if (-num < LongTenPow.Length && _bitLength + LongTenPowBitLength[(int)(-num)] < 64)
				{
					return DividePrimitiveLongs(smallValue * LongTenPow[(int)(-num)], divisor.smallValue, scale, roundingMode);
				}
			}
			BigInteger bigInteger = GetUnscaledValue();
			BigInteger bigInteger2 = divisor.GetUnscaledValue();
			if (num > 0)
			{
				bigInteger2 = Multiplication.MultiplyByTenPow(bigInteger2, (int)num);
			}
			else if (num < 0)
			{
				bigInteger = Multiplication.MultiplyByTenPow(bigInteger, (int)(-num));
			}
			return DivideBigIntegers(bigInteger, bigInteger2, scale, roundingMode);
		}

		private static BigDecimal DivideBigIntegers(BigInteger scaledDividend, BigInteger scaledDivisor, int scale, RoundingMode roundingMode)
		{
			BigInteger remainder;
			BigInteger bigInteger = scaledDividend.DivideAndRemainder(scaledDivisor, out remainder);
			if (remainder.Sign == 0)
			{
				return new BigDecimal(bigInteger, scale);
			}
			int num = scaledDividend.Sign * scaledDivisor.Sign;
			int num2;
			if (scaledDivisor.BitLength < 63)
			{
				long value = remainder.ToInt64();
				long value2 = scaledDivisor.ToInt64();
				num2 = LongCompareTo(System.Math.Abs(value) << 1, System.Math.Abs(value2));
				num2 = RoundingBehavior(bigInteger.TestBit(0) ? 1 : 0, num * (5 + num2), roundingMode);
			}
			else
			{
				num2 = remainder.Abs().ShiftLeftOneBit().CompareTo(scaledDivisor.Abs());
				num2 = RoundingBehavior(bigInteger.TestBit(0) ? 1 : 0, num * (5 + num2), roundingMode);
			}
			if (num2 != 0)
			{
				if (bigInteger.BitLength < 63)
				{
					return ValueOf(bigInteger.ToInt64() + num2, scale);
				}
				bigInteger = bigInteger.Add(BigInteger.ValueOf(num2));
				return new BigDecimal(bigInteger, scale);
			}
			return new BigDecimal(bigInteger, scale);
		}

		private static BigDecimal DividePrimitiveLongs(long scaledDividend, long scaledDivisor, int scale, RoundingMode roundingMode)
		{
			long num = scaledDividend / scaledDivisor;
			long num2 = scaledDividend % scaledDivisor;
			int num3 = System.Math.Sign(scaledDividend) * System.Math.Sign(scaledDivisor);
			if (num2 != 0)
			{
				int num4 = LongCompareTo(System.Math.Abs(num2) << 1, System.Math.Abs(scaledDivisor));
				num += RoundingBehavior((int)num & 1, num3 * (5 + num4), roundingMode);
			}
			return ValueOf(num, scale);
		}

		public BigDecimal Divide(BigDecimal divisor, int roundingMode)
		{
			if (!Enum.IsDefined(typeof(RoundingMode), roundingMode))
			{
				throw new ArgumentException();
			}
			return Divide(divisor, _scale, (RoundingMode)roundingMode);
		}

		public BigDecimal Divide(BigDecimal divisor, RoundingMode roundingMode)
		{
			return Divide(divisor, _scale, roundingMode);
		}

		public BigDecimal Divide(BigDecimal divisor)
		{
			BigInteger unscaledValue = GetUnscaledValue();
			BigInteger unscaledValue2 = divisor.GetUnscaledValue();
			long num = (long)_scale - (long)divisor._scale;
			int num2 = 0;
			int num3 = 1;
			int num4 = FivePow.Length - 1;
			if (divisor.IsZero)
			{
				throw new ArithmeticException("Division by zero");
			}
			if (unscaledValue.Sign == 0)
			{
				return GetZeroScaledBy(num);
			}
			BigInteger divisor2 = unscaledValue.Gcd(unscaledValue2);
			unscaledValue = unscaledValue.Divide(divisor2);
			unscaledValue2 = unscaledValue2.Divide(divisor2);
			int lowestSetBit = unscaledValue2.LowestSetBit;
			unscaledValue2 = unscaledValue2.ShiftRight(lowestSetBit);
			while (true)
			{
				BigInteger remainder;
				BigInteger bigInteger = unscaledValue2.DivideAndRemainder(FivePow[num3], out remainder);
				if (remainder.Sign == 0)
				{
					num2 += num3;
					if (num3 < num4)
					{
						num3++;
					}
					unscaledValue2 = bigInteger;
				}
				else
				{
					if (num3 == 1)
					{
						break;
					}
					num3 = 1;
				}
			}
			if (!unscaledValue2.Abs().Equals(BigInteger.One))
			{
				throw new ArithmeticException("Non-terminating decimal expansion; no exact representable decimal result.");
			}
			if (unscaledValue2.Sign < 0)
			{
				unscaledValue = unscaledValue.Negate();
			}
			int scale = ToIntScale(num + System.Math.Max(lowestSetBit, num2));
			num3 = lowestSetBit - num2;
			unscaledValue = ((num3 <= 0) ? unscaledValue.ShiftLeft(-num3) : Multiplication.MultiplyByFivePow(unscaledValue, num3));
			return new BigDecimal(unscaledValue, scale);
		}

		public BigDecimal Divide(BigDecimal divisor, MathContext mc)
		{
			long num = (long)mc.Precision + 2L + divisor.AproxPrecision() - AproxPrecision();
			long num2 = (long)_scale - (long)divisor._scale;
			long num3 = num2;
			int num4 = 1;
			int num5 = TenPow.Length - 1;
			BigInteger bigInteger = GetUnscaledValue();
			if (mc.Precision == 0 || IsZero || divisor.IsZero)
			{
				return Divide(divisor);
			}
			if (num > 0)
			{
				bigInteger = GetUnscaledValue().Multiply(Multiplication.PowerOf10(num));
				num3 += num;
			}
			BigInteger remainder;
			bigInteger = bigInteger.DivideAndRemainder(divisor.GetUnscaledValue(), out remainder);
			BigInteger bigInteger2 = bigInteger;
			if (remainder.Sign != 0)
			{
				int num6 = remainder.ShiftLeftOneBit().CompareTo(divisor.GetUnscaledValue());
				bigInteger2 = bigInteger2.Multiply(BigInteger.Ten).Add(BigInteger.ValueOf(bigInteger.Sign * (5 + num6)));
				num3++;
			}
			else
			{
				while (!bigInteger2.TestBit(0))
				{
					bigInteger = bigInteger2.DivideAndRemainder(TenPow[num4], out remainder);
					if (remainder.Sign == 0 && num3 - num4 >= num2)
					{
						num3 -= num4;
						if (num4 < num5)
						{
							num4++;
						}
						bigInteger2 = bigInteger;
					}
					else
					{
						if (num4 == 1)
						{
							break;
						}
						num4 = 1;
					}
				}
			}
			return new BigDecimal(bigInteger2, ToIntScale(num3), mc);
		}

		public BigDecimal DivideToIntegralValue(BigDecimal divisor)
		{
			long num = (long)_scale - (long)divisor._scale;
			long num2 = 0L;
			int num3 = 1;
			int num4 = TenPow.Length - 1;
			if (divisor.IsZero)
			{
				throw new ArithmeticException("Division by zero");
			}
			BigInteger bigInteger;
			if (divisor.AproxPrecision() + num > (long)AproxPrecision() + 1L || IsZero)
			{
				bigInteger = BigInteger.Zero;
			}
			else if (num == 0)
			{
				bigInteger = GetUnscaledValue().Divide(divisor.GetUnscaledValue());
			}
			else if (num > 0)
			{
				BigInteger val = Multiplication.PowerOf10(num);
				bigInteger = GetUnscaledValue().Divide(divisor.GetUnscaledValue().Multiply(val));
				bigInteger = bigInteger.Multiply(val);
			}
			else
			{
				BigInteger val = Multiplication.PowerOf10(-num);
				bigInteger = GetUnscaledValue().Multiply(val).Divide(divisor.GetUnscaledValue());
				while (!bigInteger.TestBit(0))
				{
					BigInteger remainder;
					BigInteger bigInteger2 = bigInteger.DivideAndRemainder(TenPow[num3], out remainder);
					if (remainder.Sign == 0 && num2 - num3 >= num)
					{
						num2 -= num3;
						if (num3 < num4)
						{
							num3++;
						}
						bigInteger = bigInteger2;
					}
					else
					{
						if (num3 == 1)
						{
							break;
						}
						num3 = 1;
					}
				}
				num = num2;
			}
			return (bigInteger.Sign != 0) ? new BigDecimal(bigInteger, ToIntScale(num)) : GetZeroScaledBy(num);
		}

		public BigDecimal DivideToIntegralValue(BigDecimal divisor, MathContext mc)
		{
			int precision = mc.Precision;
			int num = Precision - divisor.Precision;
			int num2 = TenPow.Length - 1;
			long num3 = (long)_scale - (long)divisor._scale;
			long num4 = num3;
			long num5 = num - num3 + 1;
			if (precision == 0 || IsZero || divisor.IsZero)
			{
				return DivideToIntegralValue(divisor);
			}
			BigInteger remainder;
			BigInteger bigInteger;
			if (num5 <= 0)
			{
				bigInteger = BigInteger.Zero;
			}
			else if (num3 == 0)
			{
				bigInteger = GetUnscaledValue().Divide(divisor.GetUnscaledValue());
			}
			else if (num3 > 0)
			{
				bigInteger = GetUnscaledValue().Divide(divisor.GetUnscaledValue().Multiply(Multiplication.PowerOf10(num3)));
				num4 = System.Math.Min(num3, System.Math.Max(precision - num5 + 1, 0L));
				bigInteger = bigInteger.Multiply(Multiplication.PowerOf10(num4));
			}
			else
			{
				long num6 = System.Math.Min(-num3, System.Math.Max((long)precision - (long)num, 0L));
				bigInteger = GetUnscaledValue().Multiply(Multiplication.PowerOf10(num6)).DivideAndRemainder(divisor.GetUnscaledValue(), out remainder);
				num4 += num6;
				num6 = -num4;
				if (remainder.Sign != 0 && num6 > 0)
				{
					long num7 = new BigDecimal(remainder).Precision + num6 - divisor.Precision;
					if (num7 == 0)
					{
						remainder = remainder.Multiply(Multiplication.PowerOf10(num6)).Divide(divisor.GetUnscaledValue());
						num7 = System.Math.Abs(remainder.Sign);
					}
					if (num7 > 0)
					{
						throw new ArithmeticException("Division impossible");
					}
				}
			}
			if (bigInteger.Sign == 0)
			{
				return GetZeroScaledBy(num3);
			}
			BigInteger bigInteger2 = bigInteger;
			BigDecimal bigDecimal = new BigDecimal(bigInteger);
			long num8 = bigDecimal.Precision;
			int num9 = 1;
			while (!bigInteger2.TestBit(0))
			{
				bigInteger = bigInteger2.DivideAndRemainder(TenPow[num9], out remainder);
				if (remainder.Sign == 0 && (num8 - num9 >= precision || num4 - num9 >= num3))
				{
					num8 -= num9;
					num4 -= num9;
					if (num9 < num2)
					{
						num9++;
					}
					bigInteger2 = bigInteger;
				}
				else
				{
					if (num9 == 1)
					{
						break;
					}
					num9 = 1;
				}
			}
			if (num8 > precision)
			{
				throw new ArithmeticException("Division impossible");
			}
			bigDecimal._scale = ToIntScale(num4);
			bigDecimal.SetUnscaledValue(bigInteger2);
			return bigDecimal;
		}

		public BigDecimal Remainder(BigDecimal divisor)
		{
			BigDecimal remainder;
			DivideAndRemainder(divisor, out remainder);
			return remainder;
		}

		public BigDecimal Remainder(BigDecimal divisor, MathContext mc)
		{
			BigDecimal remainder;
			DivideAndRemainder(divisor, mc, out remainder);
			return remainder;
		}

		public BigDecimal DivideAndRemainder(BigDecimal divisor, out BigDecimal remainder)
		{
			BigDecimal bigDecimal = DivideToIntegralValue(divisor);
			remainder = Subtract(bigDecimal.Multiply(divisor));
			return bigDecimal;
		}

		public BigDecimal DivideAndRemainder(BigDecimal divisor, MathContext mc, out BigDecimal remainder)
		{
			BigDecimal bigDecimal = DivideToIntegralValue(divisor, mc);
			remainder = Subtract(bigDecimal.Multiply(divisor));
			return bigDecimal;
		}

		public BigDecimal Pow(int n)
		{
			if (n == 0)
			{
				return One;
			}
			if (n < 0 || n > 999999999)
			{
				throw new ArithmeticException("Invalid Operation");
			}
			long longScale = (long)_scale * (long)n;
			return (!IsZero) ? new BigDecimal(GetUnscaledValue().Pow(n), ToIntScale(longScale)) : GetZeroScaledBy(longScale);
		}

		public BigDecimal Pow(int n, MathContext mc)
		{
			int num = System.Math.Abs(n);
			int precision = mc.Precision;
			int num2 = (int)System.Math.Log10(num) + 1;
			MathContext mc2 = mc;
			if (n == 0 || (IsZero && n > 0))
			{
				return Pow(n);
			}
			if (num > 999999999 || (precision == 0 && n < 0) || (precision > 0 && num2 > precision))
			{
				throw new ArithmeticException("Invalid Operation");
			}
			if (precision > 0)
			{
				mc2 = new MathContext(precision + num2 + 1, mc.RoundingMode);
			}
			BigDecimal bigDecimal = Round(mc2);
			for (int num3 = Utils.HighestOneBit(num) >> 1; num3 > 0; num3 >>= 1)
			{
				bigDecimal = bigDecimal.Multiply(bigDecimal, mc2);
				if ((num & num3) == num3)
				{
					bigDecimal = bigDecimal.Multiply(this, mc2);
				}
			}
			if (n < 0)
			{
				bigDecimal = One.Divide(bigDecimal, mc2);
			}
			bigDecimal.InplaceRound(mc);
			return bigDecimal;
		}

		public BigDecimal Abs()
		{
			return (Sign >= 0) ? this : Negate();
		}

		public BigDecimal Abs(MathContext mc)
		{
			return Round(mc).Abs();
		}

		public BigDecimal Negate()
		{
			if (_bitLength < 63 || (_bitLength == 63 && smallValue != long.MinValue))
			{
				return ValueOf(-smallValue, _scale);
			}
			return new BigDecimal(GetUnscaledValue().Negate(), _scale);
		}

		public BigDecimal Negate(MathContext mc)
		{
			return Round(mc).Negate();
		}

		public BigDecimal Plus()
		{
			return this;
		}

		public BigDecimal Plus(MathContext mc)
		{
			return Round(mc);
		}

		public BigDecimal Round(MathContext mc)
		{
			BigDecimal bigDecimal = new BigDecimal(GetUnscaledValue(), _scale);
			bigDecimal.InplaceRound(mc);
			return bigDecimal;
		}

		public BigDecimal SetScale(int newScale, RoundingMode roundingMode)
		{
			long num = (long)newScale - (long)_scale;
			if (num == 0)
			{
				return this;
			}
			if (num > 0)
			{
				if (num < LongTenPow.Length && _bitLength + LongTenPowBitLength[(int)num] < 64)
				{
					return ValueOf(smallValue * LongTenPow[(int)num], newScale);
				}
				return new BigDecimal(Multiplication.MultiplyByTenPow(GetUnscaledValue(), (int)num), newScale);
			}
			if (_bitLength < 64 && -num < LongTenPow.Length)
			{
				return DividePrimitiveLongs(smallValue, LongTenPow[(int)(-num)], newScale, roundingMode);
			}
			return DivideBigIntegers(GetUnscaledValue(), Multiplication.PowerOf10(-num), newScale, roundingMode);
		}

		public BigDecimal SetScale(int newScale, int roundingMode)
		{
			if (roundingMode < 0 || roundingMode > 7)
			{
				throw new ArgumentException("roundingMode");
			}
			return SetScale(newScale, (RoundingMode)roundingMode);
		}

		public BigDecimal SetScale(int newScale)
		{
			return SetScale(newScale, RoundingMode.Unnecessary);
		}

		public BigDecimal MovePointLeft(int n)
		{
			return MovePoint((long)_scale + (long)n);
		}

		private BigDecimal MovePoint(long newScale)
		{
			if (IsZero)
			{
				return GetZeroScaledBy(System.Math.Max(newScale, 0L));
			}
			if (newScale >= 0)
			{
				if (_bitLength < 64)
				{
					return ValueOf(smallValue, ToIntScale(newScale));
				}
				return new BigDecimal(GetUnscaledValue(), ToIntScale(newScale));
			}
			if (-newScale < LongTenPow.Length && _bitLength + LongTenPowBitLength[(int)(-newScale)] < 64)
			{
				return ValueOf(smallValue * LongTenPow[(int)(-newScale)], 0);
			}
			return new BigDecimal(Multiplication.MultiplyByTenPow(GetUnscaledValue(), (int)(-newScale)), 0);
		}

		public BigDecimal MovePointRight(int n)
		{
			return MovePoint((long)_scale - (long)n);
		}

		public BigDecimal ScaleByPowerOfTen(int n)
		{
			long longScale = (long)_scale - (long)n;
			if (_bitLength < 64)
			{
				if (smallValue == 0)
				{
					return GetZeroScaledBy(longScale);
				}
				return ValueOf(smallValue, ToIntScale(longScale));
			}
			return new BigDecimal(GetUnscaledValue(), ToIntScale(longScale));
		}

		public BigDecimal StripTrailingZeros()
		{
			int num = 1;
			int num2 = TenPow.Length - 1;
			long num3 = _scale;
			if (IsZero)
			{
				return Parse("0");
			}
			BigInteger bigInteger = GetUnscaledValue();
			while (!bigInteger.TestBit(0))
			{
				BigInteger remainder;
				BigInteger bigInteger2 = bigInteger.DivideAndRemainder(TenPow[num], out remainder);
				if (remainder.Sign == 0)
				{
					num3 -= num;
					if (num < num2)
					{
						num++;
					}
					bigInteger = bigInteger2;
				}
				else
				{
					if (num == 1)
					{
						break;
					}
					num = 1;
				}
			}
			return new BigDecimal(bigInteger, ToIntScale(num3));
		}

		public BigDecimal Min(BigDecimal val)
		{
			return (CompareTo(val) > 0) ? val : this;
		}

		public BigDecimal Max(BigDecimal val)
		{
			return (CompareTo(val) < 0) ? val : this;
		}

		public int CompareTo(BigDecimal val)
		{
			int sign = Sign;
			int sign2 = val.Sign;
			if (sign == sign2)
			{
				if (_scale == val._scale && _bitLength < 64 && val._bitLength < 64)
				{
					return (smallValue < val.smallValue) ? (-1) : ((smallValue > val.smallValue) ? 1 : 0);
				}
				long num = (long)_scale - (long)val._scale;
				int num2 = AproxPrecision() - val.AproxPrecision();
				if (num2 > num + 1)
				{
					return sign;
				}
				if (num2 < num - 1)
				{
					return -sign;
				}
				BigInteger bigInteger = GetUnscaledValue();
				BigInteger bigInteger2 = val.GetUnscaledValue();
				if (num < 0)
				{
					bigInteger = bigInteger.Multiply(Multiplication.PowerOf10(-num));
				}
				else if (num > 0)
				{
					bigInteger2 = bigInteger2.Multiply(Multiplication.PowerOf10(num));
				}
				return bigInteger.CompareTo(bigInteger2);
			}
			if (sign < sign2)
			{
				return -1;
			}
			return 1;
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			if (!(obj is BigDecimal))
			{
				return false;
			}
			return Equals((BigDecimal)obj);
		}

		public bool Equals(BigDecimal other)
		{
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			if (other == null)
			{
				return false;
			}
			return other._scale == _scale && ((_bitLength >= 64) ? intVal.Equals(other.intVal) : (other.smallValue == smallValue));
		}

		public override int GetHashCode()
		{
			if (_bitLength < 64)
			{
				int num = (int)(smallValue & 0xFFFFFFFFu);
				num = 33 * num + (int)((smallValue >> 32) & 0xFFFFFFFFu);
				return 17 * num + _scale;
			}
			return 17 * intVal.GetHashCode() + _scale;
		}

		public BigDecimal Ulp()
		{
			return ValueOf(1L, _scale);
		}

		private void InplaceRound(MathContext mc)
		{
			int precision = mc.Precision;
			if (AproxPrecision() - precision <= 0 || precision == 0)
			{
				return;
			}
			int num = Precision - precision;
			if (num <= 0)
			{
				return;
			}
			if (_bitLength < 64)
			{
				SmallRound(mc, num);
				return;
			}
			BigInteger bigInteger = Multiplication.PowerOf10(num);
			BigInteger remainder;
			BigInteger bigInteger2 = GetUnscaledValue().DivideAndRemainder(bigInteger, out remainder);
			long num2 = (long)_scale - (long)num;
			if (remainder.Sign != 0)
			{
				int num3 = remainder.Abs().ShiftLeftOneBit().CompareTo(bigInteger);
				num3 = RoundingBehavior(bigInteger2.TestBit(0) ? 1 : 0, remainder.Sign * (5 + num3), mc.RoundingMode);
				if (num3 != 0)
				{
					bigInteger2 = bigInteger2.Add(BigInteger.ValueOf(num3));
				}
				BigDecimal bigDecimal = new BigDecimal(bigInteger2);
				if (bigDecimal.Precision > precision)
				{
					bigInteger2 = bigInteger2.Divide(BigInteger.Ten);
					num2--;
				}
			}
			_scale = ToIntScale(num2);
			_precision = precision;
			SetUnscaledValue(bigInteger2);
		}

		private static int LongCompareTo(long value1, long value2)
		{
			return (value1 > value2) ? 1 : ((value1 < value2) ? (-1) : 0);
		}

		private void SmallRound(MathContext mc, int discardedPrecision)
		{
			long num = LongTenPow[discardedPrecision];
			long num2 = (long)_scale - (long)discardedPrecision;
			long num3 = smallValue;
			long num4 = num3 / num;
			long num5 = num3 % num;
			if (num5 != 0)
			{
				int num6 = LongCompareTo(System.Math.Abs(num5) << 1, num);
				num4 += RoundingBehavior((int)num4 & 1, System.Math.Sign(num5) * (5 + num6), mc.RoundingMode);
				if (System.Math.Log10(System.Math.Abs(num4)) >= (double)mc.Precision)
				{
					num4 /= 10;
					num2--;
				}
			}
			_scale = ToIntScale(num2);
			_precision = mc.Precision;
			smallValue = num4;
			_bitLength = BitLength(num4);
			intVal = null;
		}

		private static int RoundingBehavior(int parityBit, int fraction, RoundingMode roundingMode)
		{
			int result = 0;
			switch (roundingMode)
			{
			case RoundingMode.Unnecessary:
				if (fraction != 0)
				{
					throw new ArithmeticException("Rounding necessary");
				}
				break;
			case RoundingMode.Up:
				result = System.Math.Sign(fraction);
				break;
			case RoundingMode.Ceiling:
				result = System.Math.Max(System.Math.Sign(fraction), 0);
				break;
			case RoundingMode.Floor:
				result = System.Math.Min(System.Math.Sign(fraction), 0);
				break;
			case RoundingMode.HalfUp:
				if (System.Math.Abs(fraction) >= 5)
				{
					result = System.Math.Sign(fraction);
				}
				break;
			case RoundingMode.HalfDown:
				if (System.Math.Abs(fraction) > 5)
				{
					result = System.Math.Sign(fraction);
				}
				break;
			case RoundingMode.HalfEven:
				if (System.Math.Abs(fraction) + parityBit > 5)
				{
					result = System.Math.Sign(fraction);
				}
				break;
			}
			return result;
		}

		private long ValueExact(int bitLengthOfType)
		{
			BigInteger bigInteger = ToBigIntegerExact();
			if (bigInteger.BitLength < bitLengthOfType)
			{
				return bigInteger.ToInt64();
			}
			throw new ArithmeticException("Rounding necessary");
		}

		private int AproxPrecision()
		{
			return ((_precision <= 0) ? ((int)((double)(_bitLength - 1) * 0.3010299956639812)) : _precision) + 1;
		}

		private static int ToIntScale(long longScale)
		{
			if (longScale < int.MinValue)
			{
				throw new ArithmeticException("Overflow");
			}
			if (longScale > int.MaxValue)
			{
				throw new ArithmeticException("Underflow");
			}
			return (int)longScale;
		}

		private static BigDecimal GetZeroScaledBy(long longScale)
		{
			if (longScale == (int)longScale)
			{
				return ValueOf(0L, (int)longScale);
			}
			if (longScale >= 0)
			{
				return new BigDecimal(0, int.MaxValue);
			}
			return new BigDecimal(0, int.MinValue);
		}

		private BigInteger GetUnscaledValue()
		{
			if (intVal == null)
			{
				intVal = BigInteger.ValueOf(smallValue);
			}
			return intVal;
		}

		private void SetUnscaledValue(BigInteger unscaledValue)
		{
			intVal = unscaledValue;
			_bitLength = unscaledValue.BitLength;
			if (_bitLength < 64)
			{
				smallValue = unscaledValue.ToInt64();
			}
		}

		private static int BitLength(long smallValue)
		{
			if (smallValue < 0)
			{
				smallValue = ~smallValue;
			}
			return 64 - Utils.NumberOfLeadingZeros(smallValue);
		}

		private static int BitLength(int smallValue)
		{
			if (smallValue < 0)
			{
				smallValue = ~smallValue;
			}
			return 32 - Utils.NumberOfLeadingZeros(smallValue);
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			GetUnscaledValue();
			info.AddValue("intVal", intVal, typeof(BigInteger));
			info.AddValue("scale", _scale);
		}

		TypeCode IConvertible.GetTypeCode()
		{
			return TypeCode.Object;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			switch (ToInt32())
			{
			case 1:
				return true;
			case 0:
				return false;
			default:
				throw new InvalidCastException();
			}
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			short num = ToInt16Exact();
			return (char)num;
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			int num = ToInt32();
			if (num > 255 || num < 0)
			{
				throw new InvalidCastException();
			}
			return (byte)num;
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return ToInt16Exact();
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return ToInt32();
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return ToInt64();
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return ToSingle();
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return ToDouble();
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			return ToString(provider);
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			if (conversionType == typeof(bool))
			{
				return ((IConvertible)this).ToBoolean(provider);
			}
			if (conversionType == typeof(byte))
			{
				return ((IConvertible)this).ToByte(provider);
			}
			if (conversionType == typeof(short))
			{
				return ToInt16Exact();
			}
			if (conversionType == typeof(int))
			{
				return ToInt32();
			}
			if (conversionType == typeof(long))
			{
				return ToInt64();
			}
			if (conversionType == typeof(float))
			{
				return ToSingle();
			}
			if (conversionType == typeof(double))
			{
				return ToDouble();
			}
			if (conversionType == typeof(BigInteger))
			{
				return ToBigInteger();
			}
			throw new NotSupportedException();
		}

		public override string ToString()
		{
			if (toStringImage != null)
			{
				return toStringImage;
			}
			return ToString(null);
		}

		public string ToString(IFormatProvider provider)
		{
			if (provider == null)
			{
				provider = NumberFormatInfo.InvariantInfo;
			}
			return ToStringInternal(provider);
		}

		private string ToStringInternal(IFormatProvider provider)
		{
			NumberFormatInfo numberFormatInfo = provider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
			if (numberFormatInfo == null)
			{
				numberFormatInfo = NumberFormatInfo.InvariantInfo;
			}
			string numberDecimalSeparator = numberFormatInfo.NumberDecimalSeparator;
			if (numberDecimalSeparator.Length > 1)
			{
				throw new NotSupportedException("Decimal separators with more than one character are not supported (yet).");
			}
			if (_bitLength < 32)
			{
				toStringImage = Conversion.ToDecimalScaledString(smallValue, _scale);
				return toStringImage;
			}
			string text = GetUnscaledValue().ToString();
			if (_scale == 0)
			{
				return text;
			}
			int num = ((GetUnscaledValue().Sign >= 0) ? 1 : 2);
			int num2 = text.Length;
			long num3 = -_scale + num2 - num;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(text);
			if (_scale > 0 && num3 >= -6)
			{
				if (num3 >= 0)
				{
					stringBuilder.Insert(num2 - _scale, numberDecimalSeparator);
				}
				else
				{
					stringBuilder.Insert(num - 1, "0" + numberDecimalSeparator);
					stringBuilder.Insert(num + 1, ChZeros, 0, -(int)num3 - 1);
				}
			}
			else
			{
				if (num2 - num >= 1)
				{
					stringBuilder.Insert(num, numberDecimalSeparator);
					num2++;
				}
				stringBuilder.Insert(num2, new char[1] { 'E' });
				if (num3 > 0)
				{
					stringBuilder.Insert(++num2, new char[1] { '+' });
				}
				stringBuilder.Insert(++num2, Convert.ToString(num3));
			}
			toStringImage = stringBuilder.ToString();
			return toStringImage;
		}

		public string ToEngineeringString()
		{
			return ToEngineeringString(null);
		}

		public string ToEngineeringString(IFormatProvider provider)
		{
			NumberFormatInfo numberFormatInfo = provider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
			if (numberFormatInfo == null)
			{
				numberFormatInfo = NumberFormatInfo.InvariantInfo;
			}
			string numberDecimalSeparator = numberFormatInfo.NumberDecimalSeparator;
			if (numberDecimalSeparator.Length > 1)
			{
				throw new NotSupportedException("Decimal separators with more than one character are not supported (yet).");
			}
			string text = GetUnscaledValue().ToString();
			if (_scale == 0)
			{
				return text;
			}
			int num = ((GetUnscaledValue().Sign >= 0) ? 1 : 2);
			int num2 = text.Length;
			long num3 = -_scale + num2 - num;
			StringBuilder stringBuilder = new StringBuilder(text);
			if (_scale > 0 && num3 >= -6)
			{
				if (num3 >= 0)
				{
					stringBuilder.Insert(num2 - _scale, numberDecimalSeparator);
				}
				else
				{
					stringBuilder.Insert(num - 1, "0" + numberDecimalSeparator);
					stringBuilder.Insert(num + 1, ChZeros, 0, -(int)num3 - 1);
				}
			}
			else
			{
				int num4 = num2 - num;
				int num5 = (int)(num3 % 3);
				if (num5 != 0)
				{
					if (GetUnscaledValue().Sign == 0)
					{
						num5 = ((num5 >= 0) ? (3 - num5) : (-num5));
						num3 += num5;
					}
					else
					{
						num5 = ((num5 >= 0) ? num5 : (num5 + 3));
						num3 -= num5;
						num += num5;
					}
					if (num4 < 3)
					{
						for (int num6 = num5 - num4; num6 > 0; num6--)
						{
							stringBuilder.Insert(num2++, new char[1] { '0' });
						}
					}
				}
				if (num2 - num >= 1)
				{
					stringBuilder.Insert(num, numberDecimalSeparator);
					num2++;
				}
				if (num3 != 0)
				{
					stringBuilder.Insert(num2, new char[1] { 'E' });
					if (num3 > 0)
					{
						stringBuilder.Insert(++num2, new char[1] { '+' });
					}
					stringBuilder.Insert(++num2, Convert.ToString(num3));
				}
			}
			return stringBuilder.ToString();
		}

		public string ToPlainString()
		{
			string text = GetUnscaledValue().ToString();
			if (_scale == 0 || (IsZero && _scale < 0))
			{
				return text;
			}
			int num = ((Sign < 0) ? 1 : 0);
			int i = _scale;
			StringBuilder stringBuilder = new StringBuilder(text.Length + 1 + System.Math.Abs(_scale));
			if (num == 1)
			{
				stringBuilder.Append('-');
			}
			if (_scale > 0)
			{
				i -= text.Length - num;
				if (i >= 0)
				{
					stringBuilder.Append("0.");
					while (i > ChZeros.Length)
					{
						stringBuilder.Append(ChZeros);
						i -= ChZeros.Length;
					}
					stringBuilder.Append(ChZeros, 0, i);
					stringBuilder.Append(text.Substring(num));
				}
				else
				{
					i = num - i;
					stringBuilder.Append(text.Substring(num, i - num));
					stringBuilder.Append('.');
					stringBuilder.Append(text.Substring(i));
				}
			}
			else
			{
				stringBuilder.Append(text.Substring(num));
				for (; i < -ChZeros.Length; i += ChZeros.Length)
				{
					stringBuilder.Append(ChZeros);
				}
				stringBuilder.Append(ChZeros, 0, -i);
			}
			return stringBuilder.ToString();
		}

		private static bool TryParse(char[] inData, int offset, int len, IFormatProvider provider, out BigDecimal value, out Exception exception)
		{
			if (inData == null || inData.Length == 0)
			{
				exception = new FormatException("Cannot parse an empty string.");
				value = null;
				return false;
			}
			NumberFormatInfo numberFormatInfo = provider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
			if (numberFormatInfo == null)
			{
				numberFormatInfo = NumberFormatInfo.CurrentInfo;
			}
			string numberDecimalSeparator = numberFormatInfo.NumberDecimalSeparator;
			if (numberDecimalSeparator.Length > 1)
			{
				exception = new NotSupportedException("More than one decimal separator not yet supported");
				value = null;
				return false;
			}
			char c = numberDecimalSeparator[0];
			int num = offset;
			int num2 = offset + (len - 1);
			if (num2 >= inData.Length || offset < 0 || len <= 0 || num2 < 0)
			{
				exception = new FormatException();
				value = null;
				return false;
			}
			BigDecimal bigDecimal = new BigDecimal();
			try
			{
				StringBuilder stringBuilder = new StringBuilder(len);
				int num3 = 0;
				if (offset <= num2 && inData[offset] == '+')
				{
					offset++;
					num++;
				}
				int num4 = 0;
				bool flag = false;
				while (offset <= num2 && inData[offset] != c && inData[offset] != 'e' && inData[offset] != 'E')
				{
					if (!flag)
					{
						if (inData[offset] == '0')
						{
							num4++;
						}
						else
						{
							flag = true;
						}
					}
					offset++;
				}
				stringBuilder.Append(inData, num, offset - num);
				num3 += offset - num;
				if (offset <= num2 && inData[offset] == c)
				{
					offset++;
					num = offset;
					while (offset <= num2 && inData[offset] != 'e' && inData[offset] != 'E')
					{
						if (!flag)
						{
							if (inData[offset] == '0')
							{
								num4++;
							}
							else
							{
								flag = true;
							}
						}
						offset++;
					}
					bigDecimal._scale = offset - num;
					num3 += bigDecimal._scale;
					stringBuilder.Append(inData, num, bigDecimal._scale);
				}
				else
				{
					bigDecimal._scale = 0;
				}
				if (offset <= num2 && (inData[offset] == 'e' || inData[offset] == 'E'))
				{
					offset++;
					num = offset;
					if (offset <= num2 && inData[offset] == '+')
					{
						offset++;
						if (offset <= num2 && inData[offset] != '-')
						{
							num++;
						}
					}
					string s = new string(inData, num, num2 + 1 - num);
					long num5 = (long)bigDecimal._scale - (long)int.Parse(s, provider);
					bigDecimal._scale = (int)num5;
					if (num5 != bigDecimal._scale)
					{
						throw new FormatException("Scale out of range.");
					}
				}
				if (num3 < 19)
				{
					if (!long.TryParse(stringBuilder.ToString(), NumberStyles.Integer, provider, out bigDecimal.smallValue))
					{
						value = null;
						exception = new FormatException();
						return false;
					}
					bigDecimal._bitLength = BitLength(bigDecimal.smallValue);
				}
				else
				{
					bigDecimal.SetUnscaledValue(BigInteger.Parse(stringBuilder.ToString()));
				}
				bigDecimal._precision = stringBuilder.Length - num4;
				if (stringBuilder[0] == '-')
				{
					bigDecimal._precision--;
				}
				value = bigDecimal;
				exception = null;
				return true;
			}
			catch (Exception ex)
			{
				exception = ex;
				value = null;
				return false;
			}
		}

		public static bool TryParse(char[] chars, int offset, int length, out BigDecimal value)
		{
			return TryParse(chars, offset, length, NumberFormatInfo.InvariantInfo, out value);
		}

		public static bool TryParse(char[] chars, int offset, int length, IFormatProvider provider, out BigDecimal value)
		{
			return TryParse(chars, offset, length, null, provider, out value);
		}

		public static bool TryParse(char[] chars, int offset, int length, MathContext context, out BigDecimal value)
		{
			return TryParse(chars, offset, length, context, null, out value);
		}

		public static bool TryParse(char[] chars, int offset, int length, MathContext context, IFormatProvider provider, out BigDecimal value)
		{
			Exception exception;
			if (!TryParse(chars, offset, length, provider, out value, out exception))
			{
				return false;
			}
			if (context != null)
			{
				value.InplaceRound(context);
			}
			return true;
		}

		public static bool TryParse(char[] chars, out BigDecimal value)
		{
			return TryParse(chars, (MathContext)null, out value);
		}

		public static bool TryParse(char[] chars, MathContext context, out BigDecimal value)
		{
			return TryParse(chars, context, NumberFormatInfo.InvariantInfo, out value);
		}

		public static bool TryParse(char[] chars, IFormatProvider provider, out BigDecimal value)
		{
			return TryParse(chars, null, provider, out value);
		}

		public static bool TryParse(char[] chars, MathContext context, IFormatProvider provider, out BigDecimal value)
		{
			if (chars == null)
			{
				value = null;
				return false;
			}
			return TryParse(chars, 0, chars.Length, context, provider, out value);
		}

		public static BigDecimal Parse(char[] chars, int offset, int length, IFormatProvider provider)
		{
			return Parse(chars, offset, length, null, provider);
		}

		public static BigDecimal Parse(char[] chars, int offset, int length)
		{
			return Parse(chars, offset, length, (MathContext)null);
		}

		public static BigDecimal Parse(char[] chars, int offset, int length, MathContext context)
		{
			return Parse(chars, offset, length, context, NumberFormatInfo.InvariantInfo);
		}

		public static BigDecimal Parse(char[] chars, int offset, int length, MathContext context, IFormatProvider provider)
		{
			BigDecimal value;
			Exception exception;
			if (!TryParse(chars, offset, length, provider, out value, out exception))
			{
				throw exception;
			}
			if (context != null)
			{
				value.InplaceRound(context);
			}
			return value;
		}

		public static BigDecimal Parse(char[] chars, IFormatProvider provider)
		{
			return Parse(chars, null, provider);
		}

		public static BigDecimal Parse(char[] chars)
		{
			return Parse(chars, (MathContext)null);
		}

		public static BigDecimal Parse(char[] chars, MathContext context)
		{
			return Parse(chars, context, NumberFormatInfo.InvariantInfo);
		}

		public static BigDecimal Parse(char[] chars, MathContext context, IFormatProvider provider)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			return Parse(chars, 0, chars.Length, context, provider);
		}

		public static bool TryParse(string s, out BigDecimal value)
		{
			return TryParse(s, (MathContext)null, out value);
		}

		public static bool TryParse(string s, MathContext context, out BigDecimal value)
		{
			return TryParse(s, context, NumberFormatInfo.InvariantInfo, out value);
		}

		public static bool TryParse(string s, IFormatProvider provider, out BigDecimal value)
		{
			return TryParse(s, null, provider, out value);
		}

		public static bool TryParse(string s, MathContext context, IFormatProvider provider, out BigDecimal value)
		{
			if (string.IsNullOrEmpty(s))
			{
				value = null;
				return false;
			}
			char[] array = s.ToCharArray();
			Exception exception;
			if (!TryParse(array, 0, array.Length, provider, out value, out exception))
			{
				return false;
			}
			if (context != null)
			{
				value.InplaceRound(context);
			}
			return true;
		}

		public static BigDecimal Parse(string s, IFormatProvider provider)
		{
			return Parse(s, null, provider);
		}

		public static BigDecimal Parse(string s)
		{
			return Parse(s, (MathContext)null);
		}

		public static BigDecimal Parse(string s, MathContext context)
		{
			return Parse(s, context, NumberFormatInfo.InvariantInfo);
		}

		public static BigDecimal Parse(string s, MathContext context, IFormatProvider provider)
		{
			if (string.IsNullOrEmpty(s))
			{
				throw new FormatException();
			}
			char[] array = s.ToCharArray();
			BigDecimal value;
			Exception exception;
			if (!TryParse(array, 0, array.Length, provider, out value, out exception))
			{
				throw exception;
			}
			if (context != null)
			{
				value.InplaceRound(context);
			}
			return value;
		}

		public BigInteger ToBigInteger()
		{
			if (_scale == 0 || IsZero)
			{
				return GetUnscaledValue();
			}
			if (_scale < 0)
			{
				return GetUnscaledValue().Multiply(Multiplication.PowerOf10(-_scale));
			}
			return GetUnscaledValue().Divide(Multiplication.PowerOf10(_scale));
		}

		public BigInteger ToBigIntegerExact()
		{
			if (_scale == 0 || IsZero)
			{
				return GetUnscaledValue();
			}
			if (_scale < 0)
			{
				return GetUnscaledValue().Multiply(Multiplication.PowerOf10(-_scale));
			}
			if (_scale > AproxPrecision() || _scale > GetUnscaledValue().LowestSetBit)
			{
				throw new ArithmeticException("Rounding necessary");
			}
			BigInteger remainder;
			BigInteger result = GetUnscaledValue().DivideAndRemainder(Multiplication.PowerOf10(_scale), out remainder);
			if (remainder.Sign != 0)
			{
				throw new ArithmeticException("Rounding necessary");
			}
			return result;
		}

		public long ToInt64()
		{
			return (_scale > -64 && _scale <= AproxPrecision()) ? ToBigInteger().ToInt64() : 0;
		}

		public long ToInt64Exact()
		{
			return ValueExact(64);
		}

		public int ToInt32()
		{
			return (_scale > -32 && _scale <= AproxPrecision()) ? ToBigInteger().ToInt32() : 0;
		}

		public int ToInt32Exact()
		{
			return (int)ValueExact(32);
		}

		public short ToInt16Exact()
		{
			return (short)ValueExact(16);
		}

		public byte ToByteExact()
		{
			return (byte)ValueExact(8);
		}

		public float ToSingle()
		{
			float num = Sign;
			long num2 = _bitLength - (long)((double)_scale / 0.3010299956639812);
			if (num2 < -149 || num == 0f)
			{
				return num * 0f;
			}
			if (num2 > 129)
			{
				return num * float.PositiveInfinity;
			}
			return (float)ToDouble();
		}

		public double ToDouble()
		{
			int sign = Sign;
			int num = 1076;
			long num2 = _bitLength - (long)((double)_scale / 0.3010299956639812);
			if (num2 < -1074 || sign == 0)
			{
				return (double)sign * 0.0;
			}
			if (num2 > 1025)
			{
				return (double)sign * double.PositiveInfinity;
			}
			BigInteger bigInteger = GetUnscaledValue().Abs();
			if (_scale <= 0)
			{
				bigInteger = bigInteger.Multiply(Multiplication.PowerOf10(-_scale));
			}
			else
			{
				BigInteger bigInteger2 = Multiplication.PowerOf10(_scale);
				int num3 = 100 - (int)num2;
				if (num3 > 0)
				{
					bigInteger = bigInteger.ShiftLeft(num3);
					num -= num3;
				}
				BigInteger remainder;
				BigInteger bigInteger3 = bigInteger.DivideAndRemainder(bigInteger2, out remainder);
				int num4 = remainder.ShiftLeftOneBit().CompareTo(bigInteger2);
				bigInteger = bigInteger3.ShiftLeft(2).Add(BigInteger.ValueOf(num4 * (num4 + 3) / 2 + 1));
				num -= 2;
			}
			int lowestSetBit = bigInteger.LowestSetBit;
			int num5 = bigInteger.BitLength - 54;
			long num7;
			long num6;
			if (num5 > 0)
			{
				num6 = bigInteger.ShiftRight(num5).ToInt64();
				num7 = num6;
				if (((num6 & 1) == 1 && lowestSetBit < num5) || (num6 & 3) == 3)
				{
					num6 += 2;
				}
			}
			else
			{
				num6 = bigInteger.ToInt64() << -num5;
				num7 = num6;
				if ((num6 & 3) == 3)
				{
					num6 += 2;
				}
			}
			if ((num6 & 0x40000000000000L) == 0)
			{
				num6 >>= 1;
				num += num5;
			}
			else
			{
				num6 >>= 2;
				num += num5 + 1;
			}
			if (num > 2046)
			{
				return (double)sign * double.PositiveInfinity;
			}
			if (num <= 0)
			{
				if (num < -53)
				{
					return (double)sign * 0.0;
				}
				num6 = num7 >> 1;
				num7 = num6 & Utils.URShift(-1L, 63 + num);
				num6 >>= -num;
				if ((num6 & 3) == 3 || ((num6 & 1) == 1 && num7 != 0 && lowestSetBit < num5))
				{
					num6++;
				}
				num = 0;
				num6 >>= 1;
			}
			num6 = (sign & long.MinValue) | ((long)num << 52) | (num6 & 0xFFFFFFFFFFFFFL);
			return BitConverter.Int64BitsToDouble(num6);
		}

		public decimal ToDecimal()
		{
			BigInteger bigInteger = BigInteger.ValueOf(10L).Pow(_scale);
			BigInteger bigInteger2 = GetUnscaledValue().Remainder(bigInteger);
			BigInteger bigInteger3 = GetUnscaledValue().Divide(bigInteger);
			decimal num = (long)bigInteger3;
			decimal num2 = (decimal)(long)bigInteger2 / (decimal)(long)bigInteger;
			return num + num2;
		}

		public static BigDecimal operator +(BigDecimal a, BigDecimal b)
		{
			return a.Add(b, new MathContext(a.Precision));
		}

		public static BigDecimal operator -(BigDecimal a, BigDecimal b)
		{
			return a.Subtract(b, new MathContext(a.Precision));
		}

		public static BigDecimal operator /(BigDecimal a, BigDecimal b)
		{
			return a.Divide(b, new MathContext(a.Precision));
		}

		public static BigDecimal operator %(BigDecimal a, BigDecimal b)
		{
			return a.Remainder(b, new MathContext(a.Precision));
		}

		public static BigDecimal operator *(BigDecimal a, BigDecimal b)
		{
			return a.Multiply(b, new MathContext(a.Precision));
		}

		public static BigDecimal operator +(BigDecimal a)
		{
			return a.Plus();
		}

		public static BigDecimal operator -(BigDecimal a)
		{
			return a.Negate();
		}

		public static bool operator ==(BigDecimal a, BigDecimal b)
		{
			if ((object)a == null && (object)b == null)
			{
				return true;
			}
			if ((object)a == null)
			{
				return false;
			}
			return a.Equals(b);
		}

		public static bool operator !=(BigDecimal a, BigDecimal b)
		{
			return !(a == b);
		}

		public static bool operator >(BigDecimal a, BigDecimal b)
		{
			return a.CompareTo(b) < 0;
		}

		public static bool operator <(BigDecimal a, BigDecimal b)
		{
			return a.CompareTo(b) > 0;
		}

		public static bool operator >=(BigDecimal a, BigDecimal b)
		{
			return a == b || a > b;
		}

		public static bool operator <=(BigDecimal a, BigDecimal b)
		{
			return a == b || a < b;
		}

		public static implicit operator short(BigDecimal d)
		{
			return d.ToInt16Exact();
		}

		public static implicit operator int(BigDecimal d)
		{
			return d.ToInt32();
		}

		public static implicit operator long(BigDecimal d)
		{
			return d.ToInt64();
		}

		public static implicit operator float(BigDecimal d)
		{
			return d.ToSingle();
		}

		public static implicit operator double(BigDecimal d)
		{
			return d.ToDouble();
		}

		public static implicit operator BigInteger(BigDecimal d)
		{
			return d.ToBigInteger();
		}

		public static implicit operator string(BigDecimal d)
		{
			return d.ToString();
		}

		public static implicit operator BigDecimal(long value)
		{
			return new BigDecimal(value);
		}

		public static implicit operator BigDecimal(double value)
		{
			return new BigDecimal(value);
		}

		public static implicit operator BigDecimal(int value)
		{
			return new BigDecimal(value);
		}

		public static implicit operator BigDecimal(BigInteger value)
		{
			return new BigDecimal(value);
		}
	}
}
