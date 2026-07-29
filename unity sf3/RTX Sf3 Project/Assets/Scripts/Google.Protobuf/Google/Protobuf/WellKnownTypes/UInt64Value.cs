using System;
using System.Diagnostics;
using Google.Protobuf.Reflection;

namespace Google.Protobuf.WellKnownTypes
{
	public sealed class UInt64Value : IMessage<UInt64Value>, IMessage, IEquatable<UInt64Value>, IDeepCloneable<UInt64Value>
	{
		private static readonly MessageParser<UInt64Value> _parser = new MessageParser<UInt64Value>(() => new UInt64Value());

		public const int ValueFieldNumber = 1;

		private ulong value_;

		[DebuggerNonUserCode]
		public static MessageParser<UInt64Value> Parser
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
				return WrappersReflection.Descriptor.MessageTypes[3];
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
		public ulong Value
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
		public UInt64Value()
		{
		}

		[DebuggerNonUserCode]
		public UInt64Value(UInt64Value other)
			: this()
		{
			value_ = other.value_;
		}

		[DebuggerNonUserCode]
		public UInt64Value Clone()
		{
			return new UInt64Value(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as UInt64Value);
		}

		[DebuggerNonUserCode]
		public bool Equals(UInt64Value other)
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
				output.WriteUInt64(Value);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Value != 0L)
			{
				num += 1 + CodedOutputStream.ComputeUInt64Size(Value);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(UInt64Value other)
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
					Value = input.ReadUInt64();
				}
			}
		}
	}
}
