using System;
using System.Diagnostics;
using Google.Protobuf.Reflection;

namespace Google.Protobuf.WellKnownTypes
{
	public sealed class DoubleValue : IMessage<DoubleValue>, IMessage, IEquatable<DoubleValue>, IDeepCloneable<DoubleValue>
	{
		private static readonly MessageParser<DoubleValue> _parser = new MessageParser<DoubleValue>(() => new DoubleValue());

		public const int ValueFieldNumber = 1;

		private double value_;

		[DebuggerNonUserCode]
		public static MessageParser<DoubleValue> Parser
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
				return WrappersReflection.Descriptor.MessageTypes[0];
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
		public double Value
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
		public DoubleValue()
		{
		}

		[DebuggerNonUserCode]
		public DoubleValue(DoubleValue other)
			: this()
		{
			value_ = other.value_;
		}

		[DebuggerNonUserCode]
		public DoubleValue Clone()
		{
			return new DoubleValue(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as DoubleValue);
		}

		[DebuggerNonUserCode]
		public bool Equals(DoubleValue other)
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
			if (Value != 0.0)
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
			if (Value != 0.0)
			{
				output.WriteRawTag(9);
				output.WriteDouble(Value);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Value != 0.0)
			{
				num += 9;
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(DoubleValue other)
		{
			if (other != null && other.Value != 0.0)
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
				if (num != 9)
				{
					input.SkipLastField();
				}
				else
				{
					Value = input.ReadDouble();
				}
			}
		}
	}
}
