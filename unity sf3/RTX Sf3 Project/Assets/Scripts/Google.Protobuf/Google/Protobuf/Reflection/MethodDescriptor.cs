namespace Google.Protobuf.Reflection
{
	public sealed class MethodDescriptor : DescriptorBase
	{
		private readonly MethodDescriptorProto proto;

		private readonly ServiceDescriptor service;

		private MessageDescriptor inputType;

		private MessageDescriptor outputType;

		public ServiceDescriptor Service
		{
			get
			{
				return service;
			}
		}

		public MessageDescriptor InputType
		{
			get
			{
				return inputType;
			}
		}

		public MessageDescriptor OutputType
		{
			get
			{
				return outputType;
			}
		}

		public bool IsClientStreaming
		{
			get
			{
				return proto.ClientStreaming;
			}
		}

		public bool IsServerStreaming
		{
			get
			{
				return proto.ServerStreaming;
			}
		}

		internal MethodDescriptorProto Proto
		{
			get
			{
				return proto;
			}
		}

		public override string Name
		{
			get
			{
				return proto.Name;
			}
		}

		internal MethodDescriptor(MethodDescriptorProto proto, FileDescriptor file, ServiceDescriptor parent, int index)
			: base(file, parent.FullName + "." + proto.Name, index)
		{
			this.proto = proto;
			service = parent;
			file.DescriptorPool.AddSymbol(this);
		}

		internal void CrossLink()
		{
			IDescriptor descriptor = base.File.DescriptorPool.LookupSymbol(Proto.InputType, this);
			if (!(descriptor is MessageDescriptor))
			{
				throw new DescriptorValidationException(this, "\"" + Proto.InputType + "\" is not a message type.");
			}
			inputType = (MessageDescriptor)descriptor;
			descriptor = base.File.DescriptorPool.LookupSymbol(Proto.OutputType, this);
			if (!(descriptor is MessageDescriptor))
			{
				throw new DescriptorValidationException(this, "\"" + Proto.OutputType + "\" is not a message type.");
			}
			outputType = (MessageDescriptor)descriptor;
		}
	}
}
