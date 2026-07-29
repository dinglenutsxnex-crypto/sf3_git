using System;
using System.Runtime.Serialization;

namespace Deveel.Math
{
	[Serializable]
	public sealed class BigInteger : IComparable<BigInteger>, IEquatable<BigInteger>, ISerializable, IConvertible
	{
		[NonSerialized]
		private int[] digits;

		[NonSerialized]
		internal int numberLength;

		[NonSerialized]
		private int sign;

		public static readonly BigInteger Zero;

		public static readonly BigInteger One;

		public static readonly BigInteger Ten;

		internal static readonly BigInteger MinusOne;

		internal static readonly int EQUALS;

		internal static readonly int GREATER;

		internal static readonly int LESS;

		private static readonly BigInteger[] SmallValues;

		private static readonly BigInteger[] TwoPows;

		[NonSerialized]
		private int firstNonzeroDigit = -2;

		[NonSerialized]
		private int hashCode;

		public int Sign
		{
			get
			{
				return sign;
			}
			internal set
			{
				sign = value;
			}
		}

		public int BitLength
		{
			get
			{
				return BitLevel.BitLength(this);
			}
		}

		public int LowestSetBit
		{
			get
			{
				if (sign == 0)
				{
					return -1;
				}
				int num = FirstNonzeroDigit;
				return (num << 5) + Utils.NumberOfTrailingZeros(digits[num]);
			}
		}

		public int BitCount
		{
			get
			{
				return BitLevel.BitCount(this);
			}
		}

		internal int FirstNonzeroDigit
		{
			get
			{
				if (firstNonzeroDigit == -2)
				{
					int i;
					if (sign == 0)
					{
						i = -1;
					}
					else
					{
						for (i = 0; digits[i] == 0; i++)
						{
						}
					}
					firstNonzeroDigit = i;
				}
				return firstNonzeroDigit;
			}
		}

		internal int[] Digits
		{
			get
			{
				return digits;
			}
		}

		internal bool IsOne
		{
			get
			{
				return numberLength == 1 && digits[0] == 1;
			}
		}

		static BigInteger()
		{
			Zero = new BigInteger(0, 0);
			One = new BigInteger(1, 1);
			Ten = new BigInteger(1, 10);
			MinusOne = new BigInteger(-1, 1);
			EQUALS = 0;
			GREATER = 1;
			LESS = -1;
			SmallValues = new BigInteger[11]
			{
				Zero,
				One,
				new BigInteger(1, 2),
				new BigInteger(1, 3),
				new BigInteger(1, 4),
				new BigInteger(1, 5),
				new BigInteger(1, 6),
				new BigInteger(1, 7),
				new BigInteger(1, 8),
				new BigInteger(1, 9),
				Ten
			};
			TwoPows = new BigInteger[32];
			for (int i = 0; i < TwoPows.Length; i++)
			{
				TwoPows[i] = ValueOf(1L << i);
			}
		}

		private BigInteger()
		{
		}

		private BigInteger(SerializationInfo info, StreamingContext context)
		{
			sign = info.GetInt32("sign");
			byte[] byteValues = (byte[])info.GetValue("magnitude", typeof(byte[]));
			PutBytesPositiveToIntegers(byteValues);
			CutOffLeadingZeroes();
		}

		public BigInteger(int numBits, Random rnd)
		{
			if (numBits < 0)
			{
				throw new ArgumentException("numBits must be non-negative");
			}
			if (numBits == 0)
			{
				sign = 0;
				numberLength = 1;
				digits = new int[1];
				return;
			}
			sign = 1;
			numberLength = numBits + 31 >> 5;
			digits = new int[numberLength];
			for (int i = 0; i < numberLength; i++)
			{
				digits[i] = rnd.Next();
			}
			digits[numberLength - 1] = Utils.URShift(digits[numberLength - 1], -numBits & 0x1F);
			CutOffLeadingZeroes();
		}

		public BigInteger(int bitLength, int certainty, Random rnd)
		{
			if (bitLength < 2)
			{
				throw new ArithmeticException("bitLength < 2");
			}
			BigInteger bigInteger = Primality.ConsBigInteger(bitLength, certainty, rnd);
			sign = bigInteger.sign;
			numberLength = bigInteger.numberLength;
			digits = bigInteger.digits;
		}

		public BigInteger(int signum, byte[] magnitude)
		{
			if (magnitude == null)
			{
				throw new ArgumentNullException("magnitude");
			}
			switch (signum)
			{
			default:
				throw new FormatException("Invalid signum value");
			case 0:
			{
				for (int i = 0; i < magnitude.Length; i++)
				{
					if (magnitude[i] != 0)
					{
						throw new FormatException("signum-magnitude mismatch");
					}
				}
				break;
			}
			case -1:
			case 1:
				break;
			}
			if (magnitude.Length == 0)
			{
				sign = 0;
				numberLength = 1;
				digits = new int[1];
			}
			else
			{
				sign = signum;
				PutBytesPositiveToIntegers(magnitude);
				CutOffLeadingZeroes();
			}
		}

		public BigInteger(byte[] val)
		{
			if (val.Length == 0)
			{
				throw new FormatException("Zero length BigInteger");
			}
			if (val[0] > 127)
			{
				sign = -1;
				PutBytesNegativeToIntegers(val);
			}
			else
			{
				sign = 1;
				PutBytesPositiveToIntegers(val);
			}
			CutOffLeadingZeroes();
		}

		internal BigInteger(int sign, int value)
		{
			this.sign = sign;
			numberLength = 1;
			digits = new int[1] { value };
		}

		internal BigInteger(int sign, int numberLength, int[] digits)
		{
			this.sign = sign;
			this.numberLength = numberLength;
			this.digits = digits;
		}

		internal BigInteger(int sign, long val)
		{
			this.sign = sign;
			if ((val & -4294967296L) == 0)
			{
				numberLength = 1;
				digits = new int[1] { (int)val };
			}
			else
			{
				numberLength = 2;
				digits = new int[2]
				{
					(int)val,
					(int)(val >> 32)
				};
			}
		}

		internal BigInteger(int signum, int[] digits)
		{
			if (digits.Length == 0)
			{
				sign = 0;
				numberLength = 1;
				this.digits = new int[1];
			}
			else
			{
				sign = signum;
				numberLength = digits.Length;
				this.digits = digits;
				CutOffLeadingZeroes();
			}
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("sign", sign);
			byte[] value = Abs().ToByteArray();
			info.AddValue("magnitude", value, typeof(byte[]));
		}

		public BigInteger Abs()
		{
			return (sign >= 0) ? this : new BigInteger(1, numberLength, digits);
		}

		public BigInteger Negate()
		{
			return (sign != 0) ? new BigInteger(-sign, numberLength, digits) : this;
		}

		public BigInteger Add(BigInteger val)
		{
			return Elementary.add(this, val);
		}

		public BigInteger Subtract(BigInteger val)
		{
			return Elementary.subtract(this, val);
		}

		public BigInteger ShiftRight(int n)
		{
			if (n == 0 || sign == 0)
			{
				return this;
			}
			return (n <= 0) ? BitLevel.ShiftLeft(this, -n) : BitLevel.ShiftRight(this, n);
		}

		public BigInteger ShiftLeft(int n)
		{
			if (n == 0 || sign == 0)
			{
				return this;
			}
			return (n <= 0) ? BitLevel.ShiftRight(this, -n) : BitLevel.ShiftLeft(this, n);
		}

		internal BigInteger ShiftLeftOneBit()
		{
			return (sign != 0) ? BitLevel.ShiftLeftOneBit(this) : this;
		}

		public bool TestBit(int n)
		{
			if (n == 0)
			{
				return (digits[0] & 1) != 0;
			}
			if (n < 0)
			{
				throw new ArithmeticException("Negative bit address");
			}
			int num = n >> 5;
			if (num >= numberLength)
			{
				return sign < 0;
			}
			int num2 = digits[num];
			n = 1 << n;
			if (sign < 0)
			{
				int num3 = FirstNonzeroDigit;
				if (num < num3)
				{
					return false;
				}
				num2 = ((num3 != num) ? (~num2) : (-num2));
			}
			return (num2 & n) != 0;
		}

		public BigInteger SetBit(int n)
		{
			if (!TestBit(n))
			{
				return BitLevel.FlipBit(this, n);
			}
			return this;
		}

		public BigInteger ClearBit(int n)
		{
			if (TestBit(n))
			{
				return BitLevel.FlipBit(this, n);
			}
			return this;
		}

		public BigInteger FlipBit(int n)
		{
			if (n < 0)
			{
				throw new ArithmeticException("Negative bit address");
			}
			return BitLevel.FlipBit(this, n);
		}

		public BigInteger Not()
		{
			return Logical.Not(this);
		}

		public BigInteger And(BigInteger val)
		{
			return Logical.And(this, val);
		}

		public BigInteger Or(BigInteger val)
		{
			return Logical.Or(this, val);
		}

		public BigInteger XOr(BigInteger val)
		{
			return Logical.Xor(this, val);
		}

		public BigInteger AndNot(BigInteger val)
		{
			return Logical.AndNot(this, val);
		}

		public BigInteger Min(BigInteger val)
		{
			return (CompareTo(val) != LESS) ? val : this;
		}

		public BigInteger Max(BigInteger val)
		{
			return (CompareTo(val) != GREATER) ? val : this;
		}

		public BigInteger Gcd(BigInteger val)
		{
			BigInteger bigInteger = Abs();
			BigInteger bigInteger2 = val.Abs();
			if (bigInteger.Sign == 0)
			{
				return bigInteger2;
			}
			if (bigInteger2.Sign == 0)
			{
				return bigInteger;
			}
			if ((bigInteger.numberLength == 1 || (bigInteger.numberLength == 2 && bigInteger.digits[1] > 0)) && (bigInteger2.numberLength == 1 || (bigInteger2.numberLength == 2 && bigInteger2.digits[1] > 0)))
			{
				return ValueOf(Division.GcdBinary(bigInteger.ToInt64(), bigInteger2.ToInt64()));
			}
			return Division.GcdBinary(bigInteger.Copy(), bigInteger2.Copy());
		}

		public BigInteger Multiply(BigInteger val)
		{
			if (val.sign == 0)
			{
				return Zero;
			}
			if (sign == 0)
			{
				return Zero;
			}
			return Multiplication.Multiply(this, val);
		}

		public BigInteger Pow(int exp)
		{
			if (exp < 0)
			{
				throw new ArithmeticException("Negative exponent");
			}
			switch (exp)
			{
			case 0:
				return One;
			default:
				if (!Equals(One) && !Equals(Zero))
				{
					if (!TestBit(0))
					{
						int i;
						for (i = 1; !TestBit(i); i++)
						{
						}
						return GetPowerOfTwo(i * exp).Multiply(ShiftRight(i).Pow(exp));
					}
					return Multiplication.Pow(this, exp);
				}
				goto case 1;
			case 1:
				return this;
			}
		}

		public BigInteger DivideAndRemainder(BigInteger divisor, out BigInteger remainder)
		{
			int num = divisor.sign;
			if (num == 0)
			{
				throw new ArithmeticException("BigInteger divide by zero");
			}
			int num2 = divisor.numberLength;
			int[] array = divisor.digits;
			if (num2 == 1)
			{
				BigInteger[] array2 = Division.DivideAndRemainderByInteger(this, array[0], num);
				remainder = array2[1];
				return array2[0];
			}
			int[] a = digits;
			int num3 = numberLength;
			int num4 = ((num3 == num2) ? Elementary.compareArrays(a, array, num3) : ((num3 > num2) ? 1 : (-1)));
			if (num4 < 0)
			{
				remainder = this;
				return Zero;
			}
			int num5 = sign;
			int num6 = num3 - num2 + 1;
			int num7 = num2;
			int num8 = ((num5 == num) ? 1 : (-1));
			int[] quot = new int[num6];
			int[] array3 = Division.Divide(quot, num6, a, num3, array, num2);
			BigInteger bigInteger = new BigInteger(num8, num6, quot);
			remainder = new BigInteger(num5, num7, array3);
			bigInteger.CutOffLeadingZeroes();
			remainder.CutOffLeadingZeroes();
			return bigInteger;
		}

		public BigInteger Divide(BigInteger divisor)
		{
			if (divisor.sign == 0)
			{
				throw new ArithmeticException("BigInteger divide by zero");
			}
			int num = divisor.sign;
			if (divisor.IsOne)
			{
				return (divisor.sign <= 0) ? Negate() : this;
			}
			int num2 = sign;
			int num3 = numberLength;
			int num4 = divisor.numberLength;
			if (num3 + num4 == 2)
			{
				long num5 = (digits[0] & 0xFFFFFFFFu) / (divisor.digits[0] & 0xFFFFFFFFu);
				if (num2 != num)
				{
					num5 = -num5;
				}
				return ValueOf(num5);
			}
			int num6 = ((num3 == num4) ? Elementary.compareArrays(digits, divisor.digits, num3) : ((num3 > num4) ? 1 : (-1)));
			if (num6 == EQUALS)
			{
				return (num2 != num) ? MinusOne : One;
			}
			if (num6 == LESS)
			{
				return Zero;
			}
			int num7 = num3 - num4 + 1;
			int[] array = new int[num7];
			int num8 = ((num2 == num) ? 1 : (-1));
			if (num4 == 1)
			{
				Division.DivideArrayByInt(array, digits, num3, divisor.digits[0]);
			}
			else
			{
				Division.Divide(array, num7, digits, num3, divisor.digits, num4);
			}
			BigInteger bigInteger = new BigInteger(num8, num7, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		public BigInteger Remainder(BigInteger divisor)
		{
			if (divisor.sign == 0)
			{
				throw new ArithmeticException("BigInteger divide by zero");
			}
			int num = numberLength;
			int num2 = divisor.numberLength;
			if (((num == num2) ? Elementary.compareArrays(digits, divisor.digits, num) : ((num > num2) ? 1 : (-1))) == LESS)
			{
				return this;
			}
			int num3 = num2;
			int[] array = new int[num3];
			if (num3 == 1)
			{
				array[0] = Division.RemainderArrayByInt(digits, num, divisor.digits[0]);
			}
			else
			{
				int quotLength = num - num2 + 1;
				array = Division.Divide(null, quotLength, digits, num, divisor.digits, num2);
			}
			BigInteger bigInteger = new BigInteger(sign, num3, array);
			bigInteger.CutOffLeadingZeroes();
			return bigInteger;
		}

		public BigInteger ModInverse(BigInteger m)
		{
			if (m.sign <= 0)
			{
				throw new ArithmeticException("BigInteger: modulus not positive");
			}
			if (!TestBit(0) && !m.TestBit(0))
			{
				throw new ArithmeticException("BigInteger not invertible.");
			}
			if (m.IsOne)
			{
				return Zero;
			}
			BigInteger bigInteger = Division.ModInverseMontgomery(Abs().Mod(m), m);
			if (bigInteger.sign == 0)
			{
				throw new ArithmeticException("BigInteger not invertible.");
			}
			return (sign >= 0) ? bigInteger : m.Subtract(bigInteger);
		}

		public BigInteger ModPow(BigInteger exponent, BigInteger m)
		{
			if (m.sign <= 0)
			{
				throw new ArithmeticException("BigInteger: modulus not positive");
			}
			BigInteger bigInteger = this;
			if (m.IsOne | ((exponent.sign > 0) & (bigInteger.sign == 0)))
			{
				return Zero;
			}
			if (bigInteger.sign == 0 && exponent.sign == 0)
			{
				return One;
			}
			if (exponent.sign < 0)
			{
				bigInteger = ModInverse(m);
				exponent = exponent.Negate();
			}
			BigInteger bigInteger2 = ((!m.TestBit(0)) ? Division.EvenModPow(bigInteger.Abs(), exponent, m) : Division.OddModPow(bigInteger.Abs(), exponent, m));
			if (bigInteger.sign < 0 && exponent.TestBit(0))
			{
				bigInteger2 = m.Subtract(One).Multiply(bigInteger2).Mod(m);
			}
			return bigInteger2;
		}

		public BigInteger Mod(BigInteger m)
		{
			if (m.sign <= 0)
			{
				throw new ArithmeticException("BigInteger: modulus not positive");
			}
			BigInteger bigInteger = Remainder(m);
			return (bigInteger.sign >= 0) ? bigInteger : bigInteger.Add(m);
		}

		public bool IsProbablePrime(int certainty)
		{
			return Primality.IsProbablePrime(Abs(), certainty);
		}

		public BigInteger NextProbablePrime()
		{
			if (sign < 0)
			{
				throw new ArithmeticException(string.Format("start < 0: {0}", this));
			}
			return Primality.NextProbablePrime(this);
		}

		public static BigInteger ProbablePrime(int bitLength, Random rnd)
		{
			return new BigInteger(bitLength, 100, rnd);
		}

		public int CompareTo(BigInteger val)
		{
			if (sign > val.sign)
			{
				return GREATER;
			}
			if (sign < val.sign)
			{
				return LESS;
			}
			if (numberLength > val.numberLength)
			{
				return sign;
			}
			if (numberLength < val.numberLength)
			{
				return -val.sign;
			}
			return sign * Elementary.compareArrays(digits, val.digits, numberLength);
		}

		public override int GetHashCode()
		{
			if (hashCode != 0)
			{
				return hashCode;
			}
			for (int i = 0; i < digits.Length; i++)
			{
				hashCode = (int)(hashCode * 33 + (digits[i] & 0xFFFFFFFFu));
			}
			hashCode *= sign;
			return hashCode;
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			if (!(obj is BigInteger))
			{
				return false;
			}
			return Equals((BigInteger)obj);
		}

		public bool Equals(BigInteger other)
		{
			if (other == null)
			{
				return false;
			}
			return sign == other.sign && numberLength == other.numberLength && EqualsArrays(other.digits);
		}

		private bool EqualsArrays(int[] b)
		{
			int num = numberLength - 1;
			while (num >= 0 && digits[num] == b[num])
			{
				num--;
			}
			return num < 0;
		}

		internal void CutOffLeadingZeroes()
		{
			while (numberLength > 0 && digits[--numberLength] == 0)
			{
			}
			if (digits[numberLength++] == 0)
			{
				sign = 0;
			}
		}

		private void PutBytesPositiveToIntegers(byte[] byteValues)
		{
			int num = byteValues.Length;
			int num2 = num & 3;
			numberLength = (num >> 2) + ((num2 != 0) ? 1 : 0);
			digits = new int[numberLength];
			int num3 = 0;
			while (num > num2)
			{
				digits[num3++] = (byteValues[--num] & 0xFF) | ((byteValues[--num] & 0xFF) << 8) | ((byteValues[--num] & 0xFF) << 16) | ((byteValues[--num] & 0xFF) << 24);
			}
			for (int i = 0; i < num; i++)
			{
				digits[num3] = (digits[num3] << 8) | (byteValues[i] & 0xFF);
			}
		}

		private void PutBytesNegativeToIntegers(byte[] byteValues)
		{
			int num = byteValues.Length;
			int num2 = num & 3;
			numberLength = (num >> 2) + ((num2 != 0) ? 1 : 0);
			digits = new int[numberLength];
			int num3 = 0;
			digits[numberLength - 1] = -1;
			while (num > num2)
			{
				digits[num3] = (byteValues[--num] & 0xFF) | ((byteValues[--num] & 0xFF) << 8) | ((byteValues[--num] & 0xFF) << 16) | ((byteValues[--num] & 0xFF) << 24);
				if (digits[num3] != 0)
				{
					digits[num3] = -digits[num3];
					firstNonzeroDigit = num3;
					num3++;
					while (num > num2)
					{
						digits[num3] = (byteValues[--num] & 0xFF) | ((byteValues[--num] & 0xFF) << 8) | ((byteValues[--num] & 0xFF) << 16) | ((byteValues[--num] & 0xFF) << 24);
						digits[num3] = ~digits[num3];
						num3++;
					}
					break;
				}
				num3++;
			}
			if (num2 == 0)
			{
				return;
			}
			if (firstNonzeroDigit != -2)
			{
				for (int i = 0; i < num; i++)
				{
					digits[num3] = (digits[num3] << 8) | (byteValues[i] & 0xFF);
				}
				digits[num3] = ~digits[num3];
			}
			else
			{
				for (int j = 0; j < num; j++)
				{
					digits[num3] = (digits[num3] << 8) | (byteValues[j] & 0xFF);
				}
				digits[num3] = -digits[num3];
			}
		}

		internal BigInteger Copy()
		{
			int[] destinationArray = new int[numberLength];
			Array.Copy(digits, 0, destinationArray, 0, numberLength);
			return new BigInteger(sign, numberLength, destinationArray);
		}

		internal void UnCache()
		{
			firstNonzeroDigit = -2;
		}

		internal static BigInteger GetPowerOfTwo(int exp)
		{
			if (exp < TwoPows.Length)
			{
				return TwoPows[exp];
			}
			int num = exp >> 5;
			int num2 = exp & 0x1F;
			int[] array = new int[num + 1];
			array[num] = 1 << num2;
			return new BigInteger(1, num + 1, array);
		}

		TypeCode IConvertible.GetTypeCode()
		{
			return TypeCode.Object;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new NotSupportedException();
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
			int num = ToInt32();
			if (num > 32767 || num < -32768)
			{
				throw new InvalidCastException();
			}
			return (short)num;
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
			throw new NotImplementedException();
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new NotSupportedException();
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			return ToString();
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			if (conversionType == typeof(byte))
			{
				return ((IConvertible)this).ToByte(provider);
			}
			if (conversionType == typeof(short))
			{
				return ((IConvertible)this).ToInt16(provider);
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
			if (conversionType == typeof(string))
			{
				return ToString();
			}
			if (conversionType == typeof(byte[]))
			{
				return ToByteArray();
			}
			throw new NotSupportedException();
		}

		public static BigInteger ValueOf(long val)
		{
			if (val < 0)
			{
				if (val != -1)
				{
					return new BigInteger(-1, -val);
				}
				return MinusOne;
			}
			if (val <= 10)
			{
				return SmallValues[(int)val];
			}
			return new BigInteger(1, val);
		}

		public byte[] ToByteArray()
		{
			if (sign == 0)
			{
				return new byte[1];
			}
			int bitLength = BitLength;
			int num = FirstNonzeroDigit;
			int num2 = (bitLength >> 3) + 1;
			byte[] array = new byte[num2];
			int num3 = 0;
			int num4 = 0;
			int num5 = 4;
			int num6;
			if (num2 - (numberLength << 2) == 1)
			{
				array[0] = (byte)((sign < 0) ? uint.MaxValue : 0u);
				num6 = 4;
				num3++;
			}
			else
			{
				int num7 = num2 & 3;
				num6 = ((num7 != 0) ? num7 : 4);
			}
			num4 = num;
			num2 -= num << 2;
			if (sign < 0)
			{
				int num8 = -digits[num4];
				num4++;
				if (num4 == numberLength)
				{
					num5 = num6;
				}
				int num9 = 0;
				while (num9 < num5)
				{
					array[--num2] = (byte)num8;
					num9++;
					num8 >>= 8;
				}
				while (num2 > num3)
				{
					num8 = ~digits[num4];
					num4++;
					if (num4 == numberLength)
					{
						num5 = num6;
					}
					int num10 = 0;
					while (num10 < num5)
					{
						array[--num2] = (byte)num8;
						num10++;
						num8 >>= 8;
					}
				}
			}
			else
			{
				while (num2 > num3)
				{
					int num8 = digits[num4];
					num4++;
					if (num4 == numberLength)
					{
						num5 = num6;
					}
					int num11 = 0;
					while (num11 < num5)
					{
						array[--num2] = (byte)num8;
						num11++;
						num8 >>= 8;
					}
				}
			}
			return array;
		}

		public int ToInt32()
		{
			return sign * digits[0];
		}

		public long ToInt64()
		{
			long num = ((numberLength <= 1) ? (digits[0] & 0xFFFFFFFFu) : (((long)digits[1] << 32) | (digits[0] & 0xFFFFFFFFu)));
			return sign * num;
		}

		public float ToSingle()
		{
			return (float)ToDouble();
		}

		public double ToDouble()
		{
			return Conversion.BigInteger2Double(this);
		}

		public override string ToString()
		{
			return Conversion.ToDecimalScaledString(this, 0);
		}

		public string ToString(int radix)
		{
			return Conversion.BigInteger2String(this, radix);
		}

		private static bool TryParse(string s, int radix, out BigInteger value, out Exception exception)
		{
			if (string.IsNullOrEmpty(s))
			{
				exception = new FormatException("Radix out of range");
				value = null;
				return false;
			}
			if (radix < 2 || radix > 36)
			{
				exception = new FormatException("Zero length BigInteger");
				value = null;
				return false;
			}
			int num = s.Length;
			int num2 = num;
			int num3;
			int num4;
			if (s[0] == '-')
			{
				num3 = -1;
				num4 = 1;
				num--;
			}
			else
			{
				num3 = 1;
				num4 = 0;
			}
			int[] array;
			int num12;
			try
			{
				int num5 = Conversion.digitFitInInt[radix];
				int num6 = num / num5;
				int num7 = num % num5;
				if (num7 != 0)
				{
					num6++;
				}
				array = new int[num6];
				int factor = Conversion.bigRadices[radix - 2];
				int num8 = 0;
				int num9 = num4 + ((num7 != 0) ? num7 : num5);
				int num10 = num4;
				while (num10 < num2)
				{
					int addend = Convert.ToInt32(s.Substring(num10, num9 - num10), radix);
					int num11 = Multiplication.MultiplyByInt(array, num8, factor);
					num11 += Elementary.inplaceAdd(array, num8, addend);
					array[num8++] = num11;
					num10 = num9;
					num9 = num10 + num5;
				}
				num12 = num8;
			}
			catch (Exception ex)
			{
				exception = ex;
				value = null;
				return false;
			}
			value = new BigInteger();
			value.sign = num3;
			value.numberLength = num12;
			value.digits = array;
			value.CutOffLeadingZeroes();
			exception = null;
			return true;
		}

		public static BigInteger Parse(string s)
		{
			return Parse(s, 10);
		}

		public static bool TryParse(string s, out BigInteger value)
		{
			return TryParse(s, 10, out value);
		}

		public static bool TryParse(string s, int radix, out BigInteger value)
		{
			Exception exception;
			return TryParse(s, radix, out value, out exception);
		}

		public static BigInteger Parse(string s, int radix)
		{
			BigInteger value;
			Exception exception;
			if (!TryParse(s, radix, out value, out exception))
			{
				throw exception;
			}
			return value;
		}

		public static BigInteger operator +(BigInteger a, BigInteger b)
		{
			if (a == null)
			{
				throw new InvalidOperationException();
			}
			return a.Add(b);
		}

		public static BigInteger operator -(BigInteger a, BigInteger b)
		{
			if (a == null)
			{
				throw new InvalidOperationException();
			}
			return a.Subtract(b);
		}

		public static BigInteger operator *(BigInteger a, BigInteger b)
		{
			return a.Multiply(b);
		}

		public static BigInteger operator /(BigInteger a, BigInteger b)
		{
			return a.Divide(b);
		}

		public static BigInteger operator %(BigInteger a, BigInteger b)
		{
			return a.Mod(b);
		}

		public static BigInteger operator &(BigInteger a, BigInteger b)
		{
			return a.And(b);
		}

		public static BigInteger operator |(BigInteger a, BigInteger b)
		{
			return a.Or(b);
		}

		public static BigInteger operator ^(BigInteger a, BigInteger b)
		{
			return a.XOr(b);
		}

		public static BigInteger operator ~(BigInteger a)
		{
			return a.Not();
		}

		public static BigInteger operator -(BigInteger a)
		{
			return a.Negate();
		}

		public static BigInteger operator >>(BigInteger a, int b)
		{
			return a.ShiftRight(b);
		}

		public static BigInteger operator <<(BigInteger a, int b)
		{
			return a.ShiftLeft(b);
		}

		public static bool operator >(BigInteger a, BigInteger b)
		{
			return a.CompareTo(b) > 0;
		}

		public static bool operator <(BigInteger a, BigInteger b)
		{
			return a.CompareTo(b) < 0;
		}

		public static bool operator ==(BigInteger a, BigInteger b)
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

		public static bool operator !=(BigInteger a, BigInteger b)
		{
			return !(a == b);
		}

		public static bool operator >=(BigInteger a, BigInteger b)
		{
			return a == b || a > b;
		}

		public static bool operator <=(BigInteger a, BigInteger b)
		{
			return a == b || a < b;
		}

		public static implicit operator int(BigInteger i)
		{
			return i.ToInt32();
		}

		public static implicit operator long(BigInteger i)
		{
			return i.ToInt64();
		}

		public static implicit operator float(BigInteger i)
		{
			return i.ToSingle();
		}

		public static implicit operator double(BigInteger i)
		{
			return i.ToDouble();
		}

		public static implicit operator string(BigInteger i)
		{
			return i.ToString();
		}

		public static implicit operator BigInteger(int value)
		{
			return ValueOf(value);
		}

		public static implicit operator BigInteger(long value)
		{
			return ValueOf(value);
		}
	}
}
