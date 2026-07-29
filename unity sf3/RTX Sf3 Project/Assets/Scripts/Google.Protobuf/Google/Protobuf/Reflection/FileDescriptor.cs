using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Google.Protobuf.Reflection
{
	public sealed class FileDescriptor : IDescriptor
	{
		[CompilerGenerated]
		private readonly FileDescriptorProto _003CProto_003Ek__BackingField;

		[CompilerGenerated]
		private readonly IList<MessageDescriptor> _003CMessageTypes_003Ek__BackingField;

		[CompilerGenerated]
		private readonly IList<EnumDescriptor> _003CEnumTypes_003Ek__BackingField;

		[CompilerGenerated]
		private readonly IList<ServiceDescriptor> _003CServices_003Ek__BackingField;

		[CompilerGenerated]
		private readonly IList<FileDescriptor> _003CDependencies_003Ek__BackingField;

		[CompilerGenerated]
		private readonly IList<FileDescriptor> _003CPublicDependencies_003Ek__BackingField;

		[CompilerGenerated]
		private readonly ByteString _003CSerializedData_003Ek__BackingField;

		[CompilerGenerated]
		private readonly DescriptorPool _003CDescriptorPool_003Ek__BackingField;

		internal FileDescriptorProto Proto
		{
			[CompilerGenerated]
			get
			{
				return _003CProto_003Ek__BackingField;
			}
		}

		public string Name
		{
			get
			{
				return Proto.Name;
			}
		}

		public string Package
		{
			get
			{
				return Proto.Package;
			}
		}

		public IList<MessageDescriptor> MessageTypes
		{
			[CompilerGenerated]
			get
			{
				return _003CMessageTypes_003Ek__BackingField;
			}
		}

		public IList<EnumDescriptor> EnumTypes
		{
			[CompilerGenerated]
			get
			{
				return _003CEnumTypes_003Ek__BackingField;
			}
		}

		public IList<ServiceDescriptor> Services
		{
			[CompilerGenerated]
			get
			{
				return _003CServices_003Ek__BackingField;
			}
		}

		public IList<FileDescriptor> Dependencies
		{
			[CompilerGenerated]
			get
			{
				return _003CDependencies_003Ek__BackingField;
			}
		}

		public IList<FileDescriptor> PublicDependencies
		{
			[CompilerGenerated]
			get
			{
				return _003CPublicDependencies_003Ek__BackingField;
			}
		}

		public ByteString SerializedData
		{
			[CompilerGenerated]
			get
			{
				return _003CSerializedData_003Ek__BackingField;
			}
		}

		string IDescriptor.FullName
		{
			get
			{
				return Name;
			}
		}

		FileDescriptor IDescriptor.File
		{
			get
			{
				return this;
			}
		}

		internal DescriptorPool DescriptorPool
		{
			[CompilerGenerated]
			get
			{
				return _003CDescriptorPool_003Ek__BackingField;
			}
		}

		public static FileDescriptor DescriptorProtoFileDescriptor
		{
			get
			{
				return DescriptorReflection.Descriptor;
			}
		}

		private FileDescriptor(ByteString descriptorData, FileDescriptorProto proto, FileDescriptor[] dependencies, DescriptorPool pool, bool allowUnknownDependencies, GeneratedClrTypeInfo generatedCodeInfo)
		{
			FileDescriptor file = this;
			_003CSerializedData_003Ek__BackingField = descriptorData;
			_003CDescriptorPool_003Ek__BackingField = pool;
			_003CProto_003Ek__BackingField = proto;
			_003CDependencies_003Ek__BackingField = new ReadOnlyCollection<FileDescriptor>((FileDescriptor[])dependencies.Clone());
			_003CPublicDependencies_003Ek__BackingField = DeterminePublicDependencies(this, proto, dependencies, allowUnknownDependencies);
			pool.AddPackage(Package, this);
			_003CMessageTypes_003Ek__BackingField = DescriptorUtil.ConvertAndMakeReadOnly(proto.MessageType, (DescriptorProto message, int index) => new MessageDescriptor(message, file, null, index, generatedCodeInfo.NestedTypes[index]));
			_003CEnumTypes_003Ek__BackingField = DescriptorUtil.ConvertAndMakeReadOnly(proto.EnumType, (EnumDescriptorProto enumType, int index) => new EnumDescriptor(enumType, file, null, index, generatedCodeInfo.NestedEnums[index]));
			_003CServices_003Ek__BackingField = DescriptorUtil.ConvertAndMakeReadOnly(proto.Service, (ServiceDescriptorProto service, int index) => new ServiceDescriptor(service, this, index));
		}

		internal string ComputeFullName(MessageDescriptor parent, string name)
		{
			if (parent != null)
			{
				return parent.FullName + "." + name;
			}
			if (Package.Length > 0)
			{
				return Package + "." + name;
			}
			return name;
		}

		private static IList<FileDescriptor> DeterminePublicDependencies(FileDescriptor @this, FileDescriptorProto proto, FileDescriptor[] dependencies, bool allowUnknownDependencies)
		{
			Dictionary<string, FileDescriptor> dictionary = new Dictionary<string, FileDescriptor>();
			foreach (FileDescriptor fileDescriptor in dependencies)
			{
				dictionary[fileDescriptor.Name] = fileDescriptor;
			}
			List<FileDescriptor> list = new List<FileDescriptor>();
			for (int j = 0; j < proto.PublicDependency.Count; j++)
			{
				int num = proto.PublicDependency[j];
				if (num < 0 || num >= proto.Dependency.Count)
				{
					throw new DescriptorValidationException(@this, "Invalid public dependency index.");
				}
				string text = proto.Dependency[num];
				FileDescriptor fileDescriptor2 = dictionary[text];
				if (fileDescriptor2 == null)
				{
					if (!allowUnknownDependencies)
					{
						throw new DescriptorValidationException(@this, "Invalid public dependency: " + text);
					}
				}
				else
				{
					list.Add(fileDescriptor2);
				}
			}
			return new ReadOnlyCollection<FileDescriptor>(list);
		}

		public T FindTypeByName<T>(string name) where T : class, IDescriptor
		{
			if (name.IndexOf('.') != -1)
			{
				return null;
			}
			if (Package.Length > 0)
			{
				name = Package + "." + name;
			}
			T val = DescriptorPool.FindSymbol<T>(name);
			if (val != null && val.File == this)
			{
				return val;
			}
			return null;
		}

		private static FileDescriptor BuildFrom(ByteString descriptorData, FileDescriptorProto proto, FileDescriptor[] dependencies, bool allowUnknownDependencies, GeneratedClrTypeInfo generatedCodeInfo)
		{
			if (dependencies == null)
			{
				dependencies = new FileDescriptor[0];
			}
			DescriptorPool pool = new DescriptorPool(dependencies);
			FileDescriptor fileDescriptor = new FileDescriptor(descriptorData, proto, dependencies, pool, allowUnknownDependencies, generatedCodeInfo);
			if (dependencies.Length != proto.Dependency.Count)
			{
				throw new DescriptorValidationException(fileDescriptor, "Dependencies passed to FileDescriptor.BuildFrom() don't match those listed in the FileDescriptorProto.");
			}
			fileDescriptor.CrossLink();
			return fileDescriptor;
		}

		private void CrossLink()
		{
			foreach (MessageDescriptor messageType in MessageTypes)
			{
				messageType.CrossLink();
			}
			foreach (ServiceDescriptor service in Services)
			{
				service.CrossLink();
			}
		}

		public static FileDescriptor FromGeneratedCode(byte[] descriptorData, FileDescriptor[] dependencies, GeneratedClrTypeInfo generatedCodeInfo)
		{
			FileDescriptorProto fileDescriptorProto;
			try
			{
				fileDescriptorProto = FileDescriptorProto.Parser.ParseFrom(descriptorData);
			}
			catch (InvalidProtocolBufferException innerException)
			{
				throw new ArgumentException("Failed to parse protocol buffer descriptor for generated code.", innerException);
			}
			try
			{
				return BuildFrom(ByteString.CopyFrom(descriptorData), fileDescriptorProto, dependencies, true, generatedCodeInfo);
			}
			catch (DescriptorValidationException innerException2)
			{
				throw new ArgumentException(string.Format("Invalid embedded descriptor for \"{0}\".", fileDescriptorProto.Name), innerException2);
			}
		}

		public override string ToString()
		{
			return string.Format("FileDescriptor for {0}", Name);
		}
	}
}
