using System;
using System.Diagnostics;
using Google.Protobuf.Reflection;

namespace Google.Protobuf.WellKnownTypes
{
	public sealed class BoolValue : IMessage<BoolValue>, IMessage, IEquatable<BoolValue>, IDeepCloneable<BoolValue>
	{
		private static readonly MessageParser<BoolValue> _parser = new MessageParser<BoolValue>(() => new BoolValue());

		public const int ValueFieldNumber = 1;

		private bool value_;

		[DebuggerNonUserCode]
		public static MessageParser<BoolValue> Parser
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
				return WrappersReflection.Descriptor.MessageTypes[6];
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
		public bool Value
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
		public BoolValue()
		{
		}

		[DebuggerNonUserCode]
		public BoolValue(BoolValue other)
			: this()
		{
			value_ = other.value_;
		}

		[DebuggerNonUserCode]
		public BoolValue Clone()
		{
			return new BoolValue(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as BoolValue);
		}

		[DebuggerNonUserCode]
		public bool Equals(BoolValue other)
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
			if (Value)
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
			if (Value)
			{
				output.WriteRawTag(8);
				output.WriteBool(Value);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Value)
			{
				num += 2;
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(BoolValue other)
		{
			if (other != null && other.Value)
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
					Value = input.ReadBool();
				}
			}
		}
	}
}
