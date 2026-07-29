using System;
using System.Diagnostics;
using Google.Protobuf.Reflection;

namespace Google.Protobuf.WellKnownTypes
{
	public sealed class StringValue : IMessage<StringValue>, IMessage, IEquatable<StringValue>, IDeepCloneable<StringValue>
	{
		private static readonly MessageParser<StringValue> _parser = new MessageParser<StringValue>(() => new StringValue());

		public const int ValueFieldNumber = 1;

		private string value_ = "";

		[DebuggerNonUserCode]
		public static MessageParser<StringValue> Parser
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
				return WrappersReflection.Descriptor.MessageTypes[7];
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
		public string Value
		{
			get
			{
				return value_;
			}
			set
			{
				value_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public StringValue()
		{
		}

		[DebuggerNonUserCode]
		public StringValue(StringValue other)
			: this()
		{
			value_ = other.value_;
		}

		[DebuggerNonUserCode]
		public StringValue Clone()
		{
			return new StringValue(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as StringValue);
		}

		[DebuggerNonUserCode]
		public bool Equals(StringValue other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (Value != other.Value)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (Value.Length != 0)
			{
				num ^= Value.GetHashCode();
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
			if (Value.Length != 0)
			{
				output.WriteRawTag(10);
				output.WriteString(Value);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Value.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(Value);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(StringValue other)
		{
			if (other != null && other.Value.Length != 0)
			{
				Value = other.Value;
			}
		}

		[DebuggerNonUserCode]
		public void MergeFrom(CodedInputStream input)
		{
			uint num;
			while ((num = input.ReadTag()) != 0)
			{
				if (num != 10)
				{
					input.SkipLastField();
				}
				else
				{
					Value = input.ReadString();
				}
			}
		}
	}
}
