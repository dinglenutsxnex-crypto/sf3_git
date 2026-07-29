using System;
using System.Diagnostics;
using Google.Protobuf.Reflection;

namespace Google.Protobuf.WellKnownTypes
{
	public sealed class Int64Value : IMessage<Int64Value>, IMessage, IEquatable<Int64Value>, IDeepCloneable<Int64Value>
	{
		private static readonly MessageParser<Int64Value> _parser = new MessageParser<Int64Value>(() => new Int64Value());

		public const int ValueFieldNumber = 1;

		private long value_;

		[DebuggerNonUserCode]
		public static MessageParser<Int64Value> Parser
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
				return WrappersReflection.Descriptor.MessageTypes[2];
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
		public long Value
		{
			get
			{
				return value_;
			}
			set
			{
				value_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Int64Value()
		{
		}

		[DebuggerNonUserCode]
		public Int64Value(Int64Value other)
			: this()
		{
			value_ = other.value_;
		}

		[DebuggerNonUserCode]
		public Int64Value Clone()
		{
			return new Int64Value(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as Int64Value);
		}

		[DebuggerNonUserCode]
		public bool Equals(Int64Value other)
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
			if (Value != 0L)
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
			if (Value != 0L)
			{
				output.WriteRawTag(8);
				output.WriteInt64(Value);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Value != 0L)
			{
				num += 1 + CodedOutputStream.ComputeInt64Size(Value);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(Int64Value other)
		{
			if (other != null && other.Value != 0L)
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
				if (num != 8)
				{
					input.SkipLastField();
				}
				else
				{
					Value = input.ReadInt64();
				}
			}
		}
	}
}
