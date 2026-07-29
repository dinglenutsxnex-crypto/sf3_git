using System;
using System.Diagnostics;
using Google.Protobuf.Collections;

namespace Google.Protobuf.Reflection
{
	internal sealed class ServiceDescriptorProto : IMessage<ServiceDescriptorProto>, IMessage, IEquatable<ServiceDescriptorProto>, IDeepCloneable<ServiceDescriptorProto>
	{
		private static readonly MessageParser<ServiceDescriptorProto> _parser = new MessageParser<ServiceDescriptorProto>(() => new ServiceDescriptorProto());

		public const int NameFieldNumber = 1;

		private string name_ = "";

		public const int MethodFieldNumber = 2;

		private static readonly FieldCodec<MethodDescriptorProto> _repeated_method_codec = FieldCodec.ForMessage(18u, MethodDescriptorProto.Parser);

		private readonly RepeatedField<MethodDescriptorProto> method_ = new RepeatedField<MethodDescriptorProto>();

		public const int OptionsFieldNumber = 3;

		private ServiceOptions options_;

		[DebuggerNonUserCode]
		public static MessageParser<ServiceDescriptorProto> Parser
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
				return DescriptorReflection.Descriptor.MessageTypes[7];
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
		public string Name
		{
			get
			{
				return name_;
			}
			set
			{
				name_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<MethodDescriptorProto> Method
		{
			get
			{
				return method_;
			}
		}

		[DebuggerNonUserCode]
		public ServiceOptions Options
		{
			get
			{
				return options_;
			}
			set
			{
				options_ = value;
			}
		}

		[DebuggerNonUserCode]
		public ServiceDescriptorProto()
		{
		}

		[DebuggerNonUserCode]
		public ServiceDescriptorProto(ServiceDescriptorProto other)
			: this()
		{
			name_ = other.name_;
			method_ = other.method_.Clone();
			Options = ((other.options_ != null) ? other.Options.Clone() : null);
		}

		[DebuggerNonUserCode]
		public ServiceDescriptorProto Clone()
		{
			return new ServiceDescriptorProto(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as ServiceDescriptorProto);
		}

		[DebuggerNonUserCode]
		public bool Equals(ServiceDescriptorProto other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (Name != other.Name)
			{
				return false;
			}
			if (!method_.Equals(other.method_))
			{
				return false;
			}
			if (!object.Equals(Options, other.Options))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (Name.Length != 0)
			{
				num ^= Name.GetHashCode();
			}
			num ^= method_.GetHashCode();
			if (options_ != null)
			{
				num ^= Options.GetHashCode();
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
			if (Name.Length != 0)
			{
				output.WriteRawTag(10);
				output.WriteString(Name);
			}
			method_.WriteTo(output, _repeated_method_codec);
			if (options_ != null)
			{
				output.WriteRawTag(26);
				output.WriteMessage(Options);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (Name.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(Name);
			}
			num += method_.CalculateSize(_repeated_method_codec);
			if (options_ != null)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Options);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(ServiceDescriptorProto other)
		{
			if (other == null)
			{
				return;
			}
			if (other.Name.Length != 0)
			{
				Name = other.Name;
			}
			method_.Add(other.method_);
			if (other.options_ != null)
			{
				if (options_ == null)
				{
					options_ = new ServiceOptions();
				}
				Options.MergeFrom(other.Options);
			}
		}

		[DebuggerNonUserCode]
		public void MergeFrom(CodedInputStream input)
		{
			uint num;
			while ((num = input.ReadTag()) != 0)
			{
				switch (num)
				{
				default:
					input.SkipLastField();
					break;
				case 10u:
					Name = input.ReadString();
					break;
				case 18u:
					method_.AddEntriesFrom(input, _repeated_method_codec);
					break;
				case 26u:
					if (options_ == null)
					{
						options_ = new ServiceOptions();
					}
					input.ReadMessage(options_);
					break;
				}
			}
		}
	}
}
