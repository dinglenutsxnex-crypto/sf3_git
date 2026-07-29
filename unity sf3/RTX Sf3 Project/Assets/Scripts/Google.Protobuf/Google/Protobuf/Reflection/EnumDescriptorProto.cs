using System;
using System.Diagnostics;
using Google.Protobuf.Collections;

namespace Google.Protobuf.Reflection
{
	internal sealed class EnumDescriptorProto : IMessage<EnumDescriptorProto>, IMessage, IEquatable<EnumDescriptorProto>, IDeepCloneable<EnumDescriptorProto>
	{
		private static readonly MessageParser<EnumDescriptorProto> _parser = new MessageParser<EnumDescriptorProto>(() => new EnumDescriptorProto());

		public const int NameFieldNumber = 1;

		private string name_ = "";

		public const int ValueFieldNumber = 2;

		private static readonly FieldCodec<EnumValueDescriptorProto> _repeated_value_codec = FieldCodec.ForMessage(18u, EnumValueDescriptorProto.Parser);

		private readonly RepeatedField<EnumValueDescriptorProto> value_ = new RepeatedField<EnumValueDescriptorProto>();

		public const int OptionsFieldNumber = 3;

		private EnumOptions options_;

		[DebuggerNonUserCode]
		public static MessageParser<EnumDescriptorProto> Parser
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
				return DescriptorReflection.Descriptor.MessageTypes[5];
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
		public RepeatedField<EnumValueDescriptorProto> Value
		{
			get
			{
				return value_;
			}
		}

		[DebuggerNonUserCode]
		public EnumOptions Options
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
		public EnumDescriptorProto()
		{
		}

		[DebuggerNonUserCode]
		public EnumDescriptorProto(EnumDescriptorProto other)
			: this()
		{
			name_ = other.name_;
			value_ = other.value_.Clone();
			Options = ((other.options_ != null) ? other.Options.Clone() : null);
		}

		[DebuggerNonUserCode]
		public EnumDescriptorProto Clone()
		{
			return new EnumDescriptorProto(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as EnumDescriptorProto);
		}

		[DebuggerNonUserCode]
		public bool Equals(EnumDescriptorProto other)
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
			if (!value_.Equals(other.value_))
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
			num ^= value_.GetHashCode();
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
			value_.WriteTo(output, _repeated_value_codec);
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
			num += value_.CalculateSize(_repeated_value_codec);
			if (options_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Options);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(EnumDescriptorProto other)
		{
			if (other == null)
			{
				return;
			}
			if (other.Name.Length != 0)
			{
				Name = other.Name;
			}
			value_.Add(other.value_);
			if (other.options_ != null)
			{
				if (options_ == null)
				{
					options_ = new EnumOptions();
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
				case 18u:
					value_.AddEntriesFrom(input, _repeated_value_codec);
					break;
				case 26u:
					if (options_ == null)
					{
						options_ = new EnumOptions();
					}
					input.ReadMessage(options_);
					break;
				}
			}
		}
	}
}
