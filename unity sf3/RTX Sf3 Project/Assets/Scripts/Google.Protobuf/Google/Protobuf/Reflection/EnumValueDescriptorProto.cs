using System;
using System.Diagnostics;

namespace Google.Protobuf.Reflection
{
	internal sealed class EnumValueDescriptorProto : IMessage<EnumValueDescriptorProto>, IMessage, IEquatable<EnumValueDescriptorProto>, IDeepCloneable<EnumValueDescriptorProto>
	{
		private static readonly MessageParser<EnumValueDescriptorProto> _parser = new MessageParser<EnumValueDescriptorProto>(() => new EnumValueDescriptorProto());

		public const int NameFieldNumber = 1;

		private string name_ = "";

		public const int NumberFieldNumber = 2;

		private int number_;

		public const int OptionsFieldNumber = 3;

		private EnumValueOptions options_;

		[DebuggerNonUserCode]
		public static MessageParser<EnumValueDescriptorProto> Parser
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
				return DescriptorReflection.Descriptor.MessageTypes[6];
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
		public EnumValueOptions Options
		{
			get
			{
				return options_;
			}
			set
			{
				options_ = value;
			}
		}

		[DebuggerNonUserCode]
		public EnumValueDescriptorProto()
		{
		}

		[DebuggerNonUserCode]
		public EnumValueDescriptorProto(EnumValueDescriptorProto other)
			: this()
		{
			name_ = other.name_;
			number_ = other.number_;
			Options = ((other.options_ != null) ? other.Options.Clone() : null);
		}

		[DebuggerNonUserCode]
		public EnumValueDescriptorProto Clone()
		{
			return new EnumValueDescriptorProto(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as EnumValueDescriptorProto);
		}

		[DebuggerNonUserCode]
		public bool Equals(EnumValueDescriptorProto other)
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
			if (!object.Equals(Options, other.Options))
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
			if (options_ != null)
			{
				num ^= Options.GetHashCode();
			}
			return num;
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
			if (options_ != null)
			{
				output.WriteRawTag(26);
				output.WriteMessage(Options);
			}
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
			if (options_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Options);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(EnumValueDescriptorProto other)
		{
			if (other == null)
			{
				return;
			}
			if (other.Name.Length != 0)
			{
				Name = other.Name;
			}
			if (other.Number != 0)
			{
				Number = other.Number;
			}
			if (other.options_ != null)
			{
				if (options_ == null)
				{
					options_ = new EnumValueOptions();
				}
				Options.MergeFrom(other.Options);
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
					if (options_ == null)
					{
						options_ = new EnumValueOptions();
					}
					input.ReadMessage(options_);
					break;
				}
			}
		}
	}
}
