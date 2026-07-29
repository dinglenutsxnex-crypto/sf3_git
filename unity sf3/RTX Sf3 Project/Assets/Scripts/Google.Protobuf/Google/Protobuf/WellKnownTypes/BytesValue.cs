using System;
using System.Diagnostics;
using Google.Protobuf.Reflection;

namespace Google.Protobuf.WellKnownTypes
{
	public sealed class BytesValue : IMessage<BytesValue>, IMessage, IEquatable<BytesValue>, IDeepCloneable<BytesValue>
	{
		private static readonly MessageParser<BytesValue> _parser = new MessageParser<BytesValue>(() => new BytesValue());

		public const int ValueFieldNumber = 1;

		private ByteString value_ = ByteString.Empty;

		[DebuggerNonUserCode]
		public static MessageParser<BytesValue> Parser
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
				return WrappersReflection.Descriptor.MessageTypes[8];
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
		public ByteString Value
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
		public BytesValue()
		{
		}

		[DebuggerNonUserCode]
		public BytesValue(BytesValue other)
			: this()
		{
			value_ = other.value_;
		}

		[DebuggerNonUserCode]
		public BytesValue Clone()
		{
			return new BytesValue(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as BytesValue);
		}

		[DebuggerNonUserCode]
		public bool Equals(BytesValue other)
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
				output.WriteBytes(Value);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Value.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeBytesSize(Value);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(BytesValue other)
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
					Value = input.ReadBytes();
				}
			}
		}
	}
}
