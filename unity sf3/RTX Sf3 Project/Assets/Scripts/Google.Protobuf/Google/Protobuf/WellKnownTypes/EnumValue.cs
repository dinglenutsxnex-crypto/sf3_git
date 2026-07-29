using System;
using System.Diagnostics;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace Google.Protobuf.WellKnownTypes
{
	public sealed class EnumValue : IMessage<EnumValue>, IMessage, IEquatable<EnumValue>, IDeepCloneable<EnumValue>
	{
		private static readonly MessageParser<EnumValue> _parser = new MessageParser<EnumValue>(() => new EnumValue());

		public const int NameFieldNumber = 1;

		private string name_ = "";

		public const int NumberFieldNumber = 2;

		private int number_;

		public const int OptionsFieldNumber = 3;

		private static readonly FieldCodec<Option> _repeated_options_codec = FieldCodec.ForMessage(26u, Option.Parser);

		private readonly RepeatedField<Option> options_ = new RepeatedField<Option>();

		[DebuggerNonUserCode]
		public static MessageParser<EnumValue> Parser
		{
			get
			{
				return _parser;
			}
		}

		[DebuggerNonUserCode]
		public static MessageDescriptor Descriptor
		{
			get
			{
				return TypeReflection.Descriptor.MessageTypes[3];
			}
		}

		[DebuggerNonUserCode]
		MessageDescriptor IMessage.Descriptor
		{
			get
			{
				return Descriptor;
			}
		}

		[DebuggerNonUserCode]
		public string Name
		{
			get
			{
				return name_;
			}
			set
			{
				name_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public int Number
		{
			get
			{
				return number_;
			}
			set
			{
				number_ = value;
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<Option> Options
		{
			get
			{
				return options_;
			}
		}

		[DebuggerNonUserCode]
		public EnumValue()
		{
		}

		[DebuggerNonUserCode]
		public EnumValue(EnumValue other)
			: this()
		{
			name_ = other.name_;
			number_ = other.number_;
			options_ = other.options_.Clone();
		}

		[DebuggerNonUserCode]
		public EnumValue Clone()
		{
			return new EnumValue(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as EnumValue);
		}

		[DebuggerNonUserCode]
		public bool Equals(EnumValue other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (Name != other.Name)
			{
				return false;
			}
			if (Number != other.Number)
			{
				return false;
			}
			if (!options_.Equals(other.options_))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (Name.Length != 0)
			{
				num ^= Name.GetHashCode();
			}
			if (Number != 0)
			{
				num ^= Number.GetHashCode();
			}
			return num ^ options_.GetHashCode();
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			if (Name.Length != 0)
			{
				output.WriteRawTag(10);
				output.WriteString(Name);
			}
			if (Number != 0)
			{
				output.WriteRawTag(16);
				output.WriteInt32(Number);
			}
			options_.WriteTo(output, _repeated_options_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Name.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(Name);
			}
			if (Number != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(Number);
			}
			return num + options_.CalculateSize(_repeated_options_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(EnumValue other)
		{
			if (other != null)
			{
				if (other.Name.Length != 0)
				{
					Name = other.Name;
				}
				if (other.Number != 0)
				{
					Number = other.Number;
				}
				options_.Add(other.options_);
			}
		}

		[DebuggerNonUserCode]
		public void MergeFrom(CodedInputStream input)
		{
			uint num;
			while ((num = input.ReadTag()) != 0)
			{
				switch (num)
				{
				default:
					input.SkipLastField();
					break;
				case 10u:
					Name = input.ReadString();
					break;
				case 16u:
					Number = input.ReadInt32();
					break;
				case 26u:
					options_.AddEntriesFrom(input, _repeated_options_codec);
					break;
				}
			}
		}
	}
}
