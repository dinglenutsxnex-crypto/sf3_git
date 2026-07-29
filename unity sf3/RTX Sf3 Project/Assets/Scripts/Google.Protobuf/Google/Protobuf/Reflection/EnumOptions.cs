using System;
using System.Diagnostics;
using Google.Protobuf.Collections;

namespace Google.Protobuf.Reflection
{
	internal sealed class EnumOptions : IMessage<EnumOptions>, IMessage, IEquatable<EnumOptions>, IDeepCloneable<EnumOptions>
	{
		private static readonly MessageParser<EnumOptions> _parser = new MessageParser<EnumOptions>(() => new EnumOptions());

		public const int AllowAliasFieldNumber = 2;

		private bool allowAlias_;

		public const int DeprecatedFieldNumber = 3;

		private bool deprecated_;

		public const int UninterpretedOptionFieldNumber = 999;

		private static readonly FieldCodec<UninterpretedOption> _repeated_uninterpretedOption_codec = FieldCodec.ForMessage(7994u, Google.Protobuf.Reflection.UninterpretedOption.Parser);

		private readonly RepeatedField<UninterpretedOption> uninterpretedOption_ = new RepeatedField<UninterpretedOption>();

		[DebuggerNonUserCode]
		public static MessageParser<EnumOptions> Parser
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
				return DescriptorReflection.Descriptor.MessageTypes[13];
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
		public bool AllowAlias
		{
			get
			{
				return allowAlias_;
			}
			set
			{
				allowAlias_ = value;
			}
		}

		[DebuggerNonUserCode]
		public bool Deprecated
		{
			get
			{
				return deprecated_;
			}
			set
			{
				deprecated_ = value;
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<UninterpretedOption> UninterpretedOption
		{
			get
			{
				return uninterpretedOption_;
			}
		}

		[DebuggerNonUserCode]
		public EnumOptions()
		{
		}

		[DebuggerNonUserCode]
		public EnumOptions(EnumOptions other)
			: this()
		{
			allowAlias_ = other.allowAlias_;
			deprecated_ = other.deprecated_;
			uninterpretedOption_ = other.uninterpretedOption_.Clone();
		}

		[DebuggerNonUserCode]
		public EnumOptions Clone()
		{
			return new EnumOptions(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as EnumOptions);
		}

		[DebuggerNonUserCode]
		public bool Equals(EnumOptions other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (AllowAlias != other.AllowAlias)
			{
				return false;
			}
			if (Deprecated != other.Deprecated)
			{
				return false;
			}
			if (!uninterpretedOption_.Equals(other.uninterpretedOption_))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (AllowAlias)
			{
				num ^= AllowAlias.GetHashCode();
			}
			if (Deprecated)
			{
				num ^= Deprecated.GetHashCode();
			}
			return num ^ uninterpretedOption_.GetHashCode();
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			if (AllowAlias)
			{
				output.WriteRawTag(16);
				output.WriteBool(AllowAlias);
			}
			if (Deprecated)
			{
				output.WriteRawTag(24);
				output.WriteBool(Deprecated);
			}
			uninterpretedOption_.WriteTo(output, _repeated_uninterpretedOption_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (AllowAlias)
			{
				num += 2;
			}
			if (Deprecated)
			{
				num += 2;
			}
			return num + uninterpretedOption_.CalculateSize(_repeated_uninterpretedOption_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(EnumOptions other)
		{
			if (other != null)
			{
				if (other.AllowAlias)
				{
					AllowAlias = other.AllowAlias;
				}
				if (other.Deprecated)
				{
					Deprecated = other.Deprecated;
				}
				uninterpretedOption_.Add(other.uninterpretedOption_);
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
				case 16u:
					AllowAlias = input.ReadBool();
					break;
				case 24u:
					Deprecated = input.ReadBool();
					break;
				case 7994u:
					uninterpretedOption_.AddEntriesFrom(input, _repeated_uninterpretedOption_codec);
					break;
				}
			}
		}
	}
}
