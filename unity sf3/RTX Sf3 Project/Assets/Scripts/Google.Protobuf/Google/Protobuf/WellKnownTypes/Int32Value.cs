using System;
using System.Diagnostics;
using Google.Protobuf.Reflection;

namespace Google.Protobuf.WellKnownTypes
{
	public sealed class Int32Value : IMessage<Int32Value>, IMessage, IEquatable<Int32Value>, IDeepCloneable<Int32Value>
	{
		private static readonly MessageParser<Int32Value> _parser = new MessageParser<Int32Value>(() => new Int32Value());

		public const int ValueFieldNumber = 1;

		private int value_;

		[DebuggerNonUserCode]
		public static MessageParser<Int32Value> Parser
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
				return WrappersReflection.Descriptor.MessageTypes[4];
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
		public int Value
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
		public Int32Value()
		{
		}

		[DebuggerNonUserCode]
		public Int32Value(Int32Value other)
			: this()
		{
			value_ = other.value_;
		}

		[DebuggerNonUserCode]
		public Int32Value Clone()
		{
			return new Int32Value(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as Int32Value);
		}

		[DebuggerNonUserCode]
		public bool Equals(Int32Value other)
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
				output.WriteInt32(Value);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Value != 0)
			{
				num += 1 + CodedOutputStream.ComputeInt32Size(Value);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(Int32Value other)
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
					Value = input.ReadInt32();
				}
			}
		}
	}
}
