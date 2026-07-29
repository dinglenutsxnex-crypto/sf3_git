using System;
using System.Diagnostics;
using Google.Protobuf.Reflection;

namespace Google.Protobuf.WellKnownTypes
{
	public sealed class FloatValue : IMessage<FloatValue>, IMessage, IEquatable<FloatValue>, IDeepCloneable<FloatValue>
	{
		private static readonly MessageParser<FloatValue> _parser = new MessageParser<FloatValue>(() => new FloatValue());

		public const int ValueFieldNumber = 1;

		private float value_;

		[DebuggerNonUserCode]
		public static MessageParser<FloatValue> Parser
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
				return WrappersReflection.Descriptor.MessageTypes[1];
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
		public float Value
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
		public FloatValue()
		{
		}

		[DebuggerNonUserCode]
		public FloatValue(FloatValue other)
			: this()
		{
			value_ = other.value_;
		}

		[DebuggerNonUserCode]
		public FloatValue Clone()
		{
			return new FloatValue(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as FloatValue);
		}

		[DebuggerNonUserCode]
		public bool Equals(FloatValue other)
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
			if (Value != 0f)
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
			if (Value != 0f)
			{
				output.WriteRawTag(13);
				output.WriteFloat(Value);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Value != 0f)
			{
				num += 5;
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(FloatValue other)
		{
			if (other != null && other.Value != 0f)
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
				if (num != 13)
				{
					input.SkipLastField();
				}
				else
				{
					Value = input.ReadFloat();
				}
			}
		}
	}
}
