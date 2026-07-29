using System;
using System.Diagnostics;
using Google.Protobuf.Reflection;

namespace Google.Protobuf.WellKnownTypes
{
	public sealed class UInt32Value : IMessage<UInt32Value>, IMessage, IEquatable<UInt32Value>, IDeepCloneable<UInt32Value>
	{
		private static readonly MessageParser<UInt32Value> _parser = new MessageParser<UInt32Value>(() => new UInt32Value());

		public const int ValueFieldNumber = 1;

		private uint value_;

		[DebuggerNonUserCode]
		public static MessageParser<UInt32Value> Parser
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
				return WrappersReflection.Descriptor.MessageTypes[5];
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
		public uint Value
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
		public UInt32Value()
		{
		}

		[DebuggerNonUserCode]
		public UInt32Value(UInt32Value other)
			: this()
		{
			value_ = other.value_;
		}

		[DebuggerNonUserCode]
		public UInt32Value Clone()
		{
			return new UInt32Value(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as UInt32Value);
		}

		[DebuggerNonUserCode]
		public bool Equals(UInt32Value other)
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
			if (Value != 0)
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
			if (Value != 0)
			{
				output.WriteRawTag(8);
				output.WriteUInt32(Value);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Value != 0)
			{
				num += 1 + CodedOutputStream.ComputeUInt32Size(Value);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(UInt32Value other)
		{
			if (other != null && other.Value != 0)
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
					Value = input.ReadUInt32();
				}
			}
		}
	}
}
