using System;
using System.Diagnostics;

namespace Google.Protobuf.Reflection
{
	internal sealed class OneofDescriptorProto : IMessage<OneofDescriptorProto>, IMessage, IEquatable<OneofDescriptorProto>, IDeepCloneable<OneofDescriptorProto>
	{
		private static readonly MessageParser<OneofDescriptorProto> _parser = new MessageParser<OneofDescriptorProto>(() => new OneofDescriptorProto());

		public const int NameFieldNumber = 1;

		private string name_ = "";

		public const int OptionsFieldNumber = 2;

		private OneofOptions options_;

		[DebuggerNonUserCode]
		public static MessageParser<OneofDescriptorProto> Parser
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
				return DescriptorReflection.Descriptor.MessageTypes[4];
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
		public OneofOptions Options
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
		public OneofDescriptorProto()
		{
		}

		[DebuggerNonUserCode]
		public OneofDescriptorProto(OneofDescriptorProto other)
			: this()
		{
			name_ = other.name_;
			Options = ((other.options_ != null) ? other.Options.Clone() : null);
		}

		[DebuggerNonUserCode]
		public OneofDescriptorProto Clone()
		{
			return new OneofDescriptorProto(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as OneofDescriptorProto);
		}

		[DebuggerNonUserCode]
		public bool Equals(OneofDescriptorProto other)
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
			if (options_ != null)
			{
				output.WriteRawTag(18);
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
			if (options_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Options);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(OneofDescriptorProto other)
		{
			if (other == null)
			{
				return;
			}
			if (other.Name.Length != 0)
			{
				Name = other.Name;
			}
			if (other.options_ != null)
			{
				if (options_ == null)
				{
					options_ = new OneofOptions();
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
					if (options_ == null)
					{
						options_ = new OneofOptions();
					}
					input.ReadMessage(options_);
					break;
				}
			}
		}
	}
}
