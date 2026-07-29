using System;
using System.Diagnostics;
using Google.Protobuf.Collections;

namespace Google.Protobuf.Reflection
{
	internal sealed class ServiceOptions : IMessage<ServiceOptions>, IMessage, IEquatable<ServiceOptions>, IDeepCloneable<ServiceOptions>
	{
		private static readonly MessageParser<ServiceOptions> _parser = new MessageParser<ServiceOptions>(() => new ServiceOptions());

		public const int DeprecatedFieldNumber = 33;

		private bool deprecated_;

		public const int UninterpretedOptionFieldNumber = 999;

		private static readonly FieldCodec<UninterpretedOption> _repeated_uninterpretedOption_codec = FieldCodec.ForMessage(7994u, Google.Protobuf.Reflection.UninterpretedOption.Parser);

		private readonly RepeatedField<UninterpretedOption> uninterpretedOption_ = new RepeatedField<UninterpretedOption>();

		[DebuggerNonUserCode]
		public static MessageParser<ServiceOptions> Parser
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
				return DescriptorReflection.Descriptor.MessageTypes[15];
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
		public ServiceOptions()
		{
		}

		[DebuggerNonUserCode]
		public ServiceOptions(ServiceOptions other)
			: this()
		{
			deprecated_ = other.deprecated_;
			uninterpretedOption_ = other.uninterpretedOption_.Clone();
		}

		[DebuggerNonUserCode]
		public ServiceOptions Clone()
		{
			return new ServiceOptions(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as ServiceOptions);
		}

		[DebuggerNonUserCode]
		public bool Equals(ServiceOptions other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
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
			if (Deprecated)
			{
				output.WriteRawTag(136, 2);
				output.WriteBool(Deprecated);
			}
			uninterpretedOption_.WriteTo(output, _repeated_uninterpretedOption_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Deprecated)
			{
				num += 3;
			}
			return num + uninterpretedOption_.CalculateSize(_repeated_uninterpretedOption_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(ServiceOptions other)
		{
			if (other != null)
			{
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
				case 264u:
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
