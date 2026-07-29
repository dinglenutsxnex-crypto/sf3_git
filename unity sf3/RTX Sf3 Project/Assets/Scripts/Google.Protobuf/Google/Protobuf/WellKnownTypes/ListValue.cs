using System;
using System.Diagnostics;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace Google.Protobuf.WellKnownTypes
{
	public sealed class ListValue : IMessage<ListValue>, IMessage, IEquatable<ListValue>, IDeepCloneable<ListValue>
	{
		private static readonly MessageParser<ListValue> _parser = new MessageParser<ListValue>(() => new ListValue());

		public const int ValuesFieldNumber = 1;

		private static readonly FieldCodec<Value> _repeated_values_codec = FieldCodec.ForMessage(10u, Value.Parser);

		private readonly RepeatedField<Value> values_ = new RepeatedField<Value>();

		[DebuggerNonUserCode]
		public static MessageParser<ListValue> Parser
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
				return StructReflection.Descriptor.MessageTypes[2];
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
		public RepeatedField<Value> Values
		{
			get
			{
				return values_;
			}
		}

		[DebuggerNonUserCode]
		public ListValue()
		{
		}

		[DebuggerNonUserCode]
		public ListValue(ListValue other)
			: this()
		{
			values_ = other.values_.Clone();
		}

		[DebuggerNonUserCode]
		public ListValue Clone()
		{
			return new ListValue(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as ListValue);
		}

		[DebuggerNonUserCode]
		public bool Equals(ListValue other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (!values_.Equals(other.values_))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			return 1 ^ values_.GetHashCode();
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			values_.WriteTo(output, _repeated_values_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			return 0 + values_.CalculateSize(_repeated_values_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(ListValue other)
		{
			if (other != null)
			{
				values_.Add(other.values_);
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
					values_.AddEntriesFrom(input, _repeated_values_codec);
				}
			}
		}
	}
}
