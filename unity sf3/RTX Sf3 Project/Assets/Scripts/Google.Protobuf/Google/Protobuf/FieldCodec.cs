using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Google.Protobuf
{
	public static class FieldCodec
	{
		private static class WrapperCodecs
		{
			private static readonly Dictionary<Type, object> Codecs = new Dictionary<Type, object>
			{
				{
					typeof(bool),
					ForBool(WireFormat.MakeTag(1, WireFormat.WireType.Varint))
				},
				{
					typeof(int),
					ForInt32(WireFormat.MakeTag(1, WireFormat.WireType.Varint))
				},
				{
					typeof(long),
					ForInt64(WireFormat.MakeTag(1, WireFormat.WireType.Varint))
				},
				{
					typeof(uint),
					ForUInt32(WireFormat.MakeTag(1, WireFormat.WireType.Varint))
				},
				{
					typeof(ulong),
					ForUInt64(WireFormat.MakeTag(1, WireFormat.WireType.Varint))
				},
				{
					typeof(float),
					ForFloat(WireFormat.MakeTag(1, WireFormat.WireType.Fixed32))
				},
				{
					typeof(double),
					ForDouble(WireFormat.MakeTag(1, WireFormat.WireType.Fixed64))
				},
				{
					typeof(string),
					ForString(WireFormat.MakeTag(1, WireFormat.WireType.LengthDelimited))
				},
				{
					typeof(ByteString),
					ForBytes(WireFormat.MakeTag(1, WireFormat.WireType.LengthDelimited))
				}
			};

			internal static FieldCodec<T> GetCodec<T>()
			{
				object value;
				if (!Codecs.TryGetValue(typeof(T), out value))
				{
					throw new InvalidOperationException("Invalid type argument requested for wrapper codec: " + typeof(T));
				}
				return (FieldCodec<T>)value;
			}

			internal static T Read<T>(CodedInputStream input, FieldCodec<T> codec)
			{
				int byteLimit = input.ReadLength();
				int oldLimit = input.PushLimit(byteLimit);
				T result = codec.DefaultValue;
				uint num;
				while ((num = input.ReadTag()) != 0)
				{
					if (num == codec.Tag)
					{
						result = codec.Read(input);
					}
					else
					{
						input.SkipLastField();
					}
				}
				input.CheckReadEndOfStreamTag();
				input.PopLimit(oldLimit);
				return result;
			}

			internal static void Write<T>(CodedOutputStream output, T value, FieldCodec<T> codec)
			{
				output.WriteLength(codec.CalculateSizeWithTag(value));
				codec.WriteTagAndValue(output, value);
			}

			internal static int CalculateSize<T>(T value, FieldCodec<T> codec)
			{
				int num = codec.CalculateSizeWithTag(value);
				return CodedOutputStream.ComputeLengthSize(num) + num;
			}
		}

		public static FieldCodec<string> ForString(uint tag)
		{
			return new FieldCodec<string>((CodedInputStream input) => input.ReadString(), delegate(CodedOutputStream output, string value)
			{
				output.WriteString(value);
			}, CodedOutputStream.ComputeStringSize, tag);
		}

		public static FieldCodec<ByteString> ForBytes(uint tag)
		{
			return new FieldCodec<ByteString>((CodedInputStream input) => input.ReadBytes(), delegate(CodedOutputStream output, ByteString value)
			{
				output.WriteBytes(value);
			}, CodedOutputStream.ComputeBytesSize, tag);
		}

		public static FieldCodec<bool> ForBool(uint tag)
		{
			return new FieldCodec<bool>((CodedInputStream input) => input.ReadBool(), delegate(CodedOutputStream output, bool value)
			{
				output.WriteBool(value);
			}, CodedOutputStream.ComputeBoolSize, tag);
		}

		public static FieldCodec<int> ForInt32(uint tag)
		{
			return new FieldCodec<int>((CodedInputStream input) => input.ReadInt32(), delegate(CodedOutputStream output, int value)
			{
				output.WriteInt32(value);
			}, CodedOutputStream.ComputeInt32Size, tag);
		}

		public static FieldCodec<int> ForSInt32(uint tag)
		{
			return new FieldCodec<int>((CodedInputStream input) => input.ReadSInt32(), delegate(CodedOutputStream output, int value)
			{
				output.WriteSInt32(value);
			}, CodedOutputStream.ComputeSInt32Size, tag);
		}

		public static FieldCodec<uint> ForFixed32(uint tag)
		{
			return new FieldCodec<uint>((CodedInputStream input) => input.ReadFixed32(), delegate(CodedOutputStream output, uint value)
			{
				output.WriteFixed32(value);
			}, 4, tag);
		}

		public static FieldCodec<int> ForSFixed32(uint tag)
		{
			return new FieldCodec<int>((CodedInputStream input) => input.ReadSFixed32(), delegate(CodedOutputStream output, int value)
			{
				output.WriteSFixed32(value);
			}, 4, tag);
		}

		public static FieldCodec<uint> ForUInt32(uint tag)
		{
			return new FieldCodec<uint>((CodedInputStream input) => input.ReadUInt32(), delegate(CodedOutputStream output, uint value)
			{
				output.WriteUInt32(value);
			}, CodedOutputStream.ComputeUInt32Size, tag);
		}

		public static FieldCodec<long> ForInt64(uint tag)
		{
			return new FieldCodec<long>((CodedInputStream input) => input.ReadInt64(), delegate(CodedOutputStream output, long value)
			{
				output.WriteInt64(value);
			}, CodedOutputStream.ComputeInt64Size, tag);
		}

		public static FieldCodec<long> ForSInt64(uint tag)
		{
			return new FieldCodec<long>((CodedInputStream input) => input.ReadSInt64(), delegate(CodedOutputStream output, long value)
			{
				output.WriteSInt64(value);
			}, CodedOutputStream.ComputeSInt64Size, tag);
		}

		public static FieldCodec<ulong> ForFixed64(uint tag)
		{
			return new FieldCodec<ulong>((CodedInputStream input) => input.ReadFixed64(), delegate(CodedOutputStream output, ulong value)
			{
				output.WriteFixed64(value);
			}, 8, tag);
		}

		public static FieldCodec<long> ForSFixed64(uint tag)
		{
			return new FieldCodec<long>((CodedInputStream input) => input.ReadSFixed64(), delegate(CodedOutputStream output, long value)
			{
				output.WriteSFixed64(value);
			}, 8, tag);
		}

		public static FieldCodec<ulong> ForUInt64(uint tag)
		{
			return new FieldCodec<ulong>((CodedInputStream input) => input.ReadUInt64(), delegate(CodedOutputStream output, ulong value)
			{
				output.WriteUInt64(value);
			}, CodedOutputStream.ComputeUInt64Size, tag);
		}

		public static FieldCodec<float> ForFloat(uint tag)
		{
			return new FieldCodec<float>((CodedInputStream input) => input.ReadFloat(), delegate(CodedOutputStream output, float value)
			{
				output.WriteFloat(value);
			}, CodedOutputStream.ComputeFloatSize, tag);
		}

		public static FieldCodec<double> ForDouble(uint tag)
		{
			return new FieldCodec<double>((CodedInputStream input) => input.ReadDouble(), delegate(CodedOutputStream output, double value)
			{
				output.WriteDouble(value);
			}, CodedOutputStream.ComputeDoubleSize, tag);
		}

		public static FieldCodec<T> ForEnum<T>(uint tag, Func<T, int> toInt32, Func<int, T> fromInt32)
		{
			return new FieldCodec<T>((CodedInputStream input) => fromInt32(input.ReadEnum()), delegate(CodedOutputStream output, T value)
			{
				output.WriteEnum(toInt32(value));
			}, (T value) => CodedOutputStream.ComputeEnumSize(toInt32(value)), tag);
		}

		public static FieldCodec<T> ForMessage<T>(uint tag, MessageParser<T> parser) where T : IMessage<T>
		{
			return new FieldCodec<T>(delegate(CodedInputStream input)
			{
				T val = parser.CreateTemplate();
				input.ReadMessage(val);
				return val;
			}, delegate(CodedOutputStream output, T value)
			{
				output.WriteMessage(value);
			}, (T message) => CodedOutputStream.ComputeMessageSize(message), tag);
		}

		public static FieldCodec<T> ForClassWrapper<T>(uint tag) where T : class
		{
			FieldCodec<T> nestedCodec = WrapperCodecs.GetCodec<T>();
			return new FieldCodec<T>((CodedInputStream input) => WrapperCodecs.Read(input, nestedCodec), delegate(CodedOutputStream output, T value)
			{
				WrapperCodecs.Write(output, value, nestedCodec);
			}, (T value) => WrapperCodecs.CalculateSize(value, nestedCodec), tag, null);
		}

		public static FieldCodec<T?> ForStructWrapper<T>(uint tag) where T : struct
		{
			FieldCodec<T> nestedCodec = WrapperCodecs.GetCodec<T>();
			return new FieldCodec<T?>((CodedInputStream input) => WrapperCodecs.Read(input, nestedCodec), delegate(CodedOutputStream output, T? value)
			{
				WrapperCodecs.Write(output, value.Value, nestedCodec);
			}, (T? value) => value.HasValue ? WrapperCodecs.CalculateSize(value.Value, nestedCodec) : 0, tag, null);
		}
	}
	public sealed class FieldCodec<T>
	{
		private static readonly T DefaultDefault;

		private static readonly bool TypeSupportsPacking;

		[CompilerGenerated]
		private readonly bool _003CPackedRepeatedField_003Ek__BackingField;

		[CompilerGenerated]
		private readonly Action<CodedOutputStream, T> _003CValueWriter_003Ek__BackingField;

		[CompilerGenerated]
		private readonly Func<T, int> _003CValueSizeCalculator_003Ek__BackingField;

		[CompilerGenerated]
		private readonly Func<CodedInputStream, T> _003CValueReader_003Ek__BackingField;

		[CompilerGenerated]
		private readonly int _003CFixedSize_003Ek__BackingField;

		[CompilerGenerated]
		private readonly uint _003CTag_003Ek__BackingField;

		[CompilerGenerated]
		private readonly T _003CDefaultValue_003Ek__BackingField;

		private readonly int tagSize;

		internal bool PackedRepeatedField
		{
			[CompilerGenerated]
			get
			{
				return _003CPackedRepeatedField_003Ek__BackingField;
			}
		}

		internal Action<CodedOutputStream, T> ValueWriter
		{
			[CompilerGenerated]
			get
			{
				return _003CValueWriter_003Ek__BackingField;
			}
		}

		internal Func<T, int> ValueSizeCalculator
		{
			[CompilerGenerated]
			get
			{
				return _003CValueSizeCalculator_003Ek__BackingField;
			}
		}

		internal Func<CodedInputStream, T> ValueReader
		{
			[CompilerGenerated]
			get
			{
				return _003CValueReader_003Ek__BackingField;
			}
		}

		internal int FixedSize
		{
			[CompilerGenerated]
			get
			{
				return _003CFixedSize_003Ek__BackingField;
			}
		}

		internal uint Tag
		{
			[CompilerGenerated]
			get
			{
				return _003CTag_003Ek__BackingField;
			}
		}

		internal T DefaultValue
		{
			[CompilerGenerated]
			get
			{
				return _003CDefaultValue_003Ek__BackingField;
			}
		}

		static FieldCodec()
		{
			TypeSupportsPacking = default(T) != null;
			if (typeof(T) == typeof(string))
			{
				DefaultDefault = (T)(object)"";
			}
			else if (typeof(T) == typeof(ByteString))
			{
				DefaultDefault = (T)(object)ByteString.Empty;
			}
		}

		internal static bool IsPackedRepeatedField(uint tag)
		{
			if (TypeSupportsPacking)
			{
				return WireFormat.GetTagWireType(tag) == WireFormat.WireType.LengthDelimited;
			}
			return false;
		}

		internal FieldCodec(Func<CodedInputStream, T> reader, Action<CodedOutputStream, T> writer, int fixedSize, uint tag)
			: this(reader, writer, (Func<T, int>)((T _) => fixedSize), tag)
		{
			_003CFixedSize_003Ek__BackingField = fixedSize;
		}

		internal FieldCodec(Func<CodedInputStream, T> reader, Action<CodedOutputStream, T> writer, Func<T, int> sizeCalculator, uint tag)
			: this(reader, writer, sizeCalculator, tag, DefaultDefault)
		{
		}

		internal FieldCodec(Func<CodedInputStream, T> reader, Action<CodedOutputStream, T> writer, Func<T, int> sizeCalculator, uint tag, T defaultValue)
		{
			_003CValueReader_003Ek__BackingField = reader;
			_003CValueWriter_003Ek__BackingField = writer;
			_003CValueSizeCalculator_003Ek__BackingField = sizeCalculator;
			_003CFixedSize_003Ek__BackingField = 0;
			_003CTag_003Ek__BackingField = tag;
			_003CDefaultValue_003Ek__BackingField = defaultValue;
			tagSize = CodedOutputStream.ComputeRawVarint32Size(tag);
			_003CPackedRepeatedField_003Ek__BackingField = IsPackedRepeatedField(tag);
		}

		public void WriteTagAndValue(CodedOutputStream output, T value)
		{
			if (!IsDefault(value))
			{
				output.WriteTag(Tag);
				ValueWriter(output, value);
			}
		}

		public T Read(CodedInputStream input)
		{
			return ValueReader(input);
		}

		public int CalculateSizeWithTag(T value)
		{
			if (!IsDefault(value))
			{
				return ValueSizeCalculator(value) + tagSize;
			}
			return 0;
		}

		private bool IsDefault(T value)
		{
			return EqualityComparer<T>.Default.Equals(value, DefaultValue);
		}
	}
}
