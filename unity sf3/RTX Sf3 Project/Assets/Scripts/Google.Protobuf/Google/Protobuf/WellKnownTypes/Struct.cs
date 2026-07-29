using System;
using System.Diagnostics;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace Google.Protobuf.WellKnownTypes
{
	public sealed class Struct : IMessage<Struct>, IMessage, IEquatable<Struct>, IDeepCloneable<Struct>
	{
		private static readonly MessageParser<Struct> _parser = new MessageParser<Struct>(() => new Struct());

		public const int FieldsFieldNumber = 1;

		private static readonly MapField<string, Value>.Codec _map_fields_codec = new MapField<string, Value>.Codec(FieldCodec.ForString(10u), FieldCodec.ForMessage(18u, Value.Parser), 10u);

		private readonly MapField<string, Value> fields_ = new MapField<string, Value>();

		[DebuggerNonUserCode]
		public static MessageParser<Struct> Parser
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
				return StructReflection.Descriptor.MessageTypes[0];
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
		public MapField<string, Value> Fields
		{
			get
			{
				return fields_;
			}
		}

		[DebuggerNonUserCode]
		public Struct()
		{
		}

		[DebuggerNonUserCode]
		public Struct(Struct other)
			: this()
		{
			fields_ = other.fields_.Clone();
		}

		[DebuggerNonUserCode]
		public Struct Clone()
		{
			return new Struct(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as Struct);
		}

		[DebuggerNonUserCode]
		public bool Equals(Struct other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (!Fields.Equals(other.Fields))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			return 1 ^ Fields.GetHashCode();
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			fields_.WriteTo(output, _map_fields_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			return 0 + fields_.CalculateSize(_map_fields_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(Struct other)
		{
			if (other != null)
			{
				fields_.Add(other.fields_);
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
					fields_.AddEntriesFrom(input, _map_fields_codec);
				}
			}
		}
	}
}
