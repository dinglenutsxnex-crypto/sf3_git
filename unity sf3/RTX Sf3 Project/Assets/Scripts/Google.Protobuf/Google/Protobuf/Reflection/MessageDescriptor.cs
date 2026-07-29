using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Google.Protobuf.Collections;

namespace Google.Protobuf.Reflection
{
	public sealed class MessageDescriptor : DescriptorBase
	{
		public sealed class FieldCollection
		{
			private readonly MessageDescriptor messageDescriptor;

			public FieldDescriptor this[int number]
			{
				get
				{
					FieldDescriptor fieldDescriptor = messageDescriptor.FindFieldByNumber(number);
					if (fieldDescriptor == null)
					{
						throw new KeyNotFoundException("No such field number");
					}
					return fieldDescriptor;
				}
			}

			public FieldDescriptor this[string name]
			{
				get
				{
					FieldDescriptor fieldDescriptor = messageDescriptor.FindFieldByName(name);
					if (fieldDescriptor == null)
					{
						throw new KeyNotFoundException("No such field name");
					}
					return fieldDescriptor;
				}
			}

			internal FieldCollection(MessageDescriptor messageDescriptor)
			{
				this.messageDescriptor = messageDescriptor;
			}

			public IList<FieldDescriptor> InDeclarationOrder()
			{
				return messageDescriptor.fieldsInDeclarationOrder;
			}

			public IList<FieldDescriptor> InFieldNumberOrder()
			{
				return messageDescriptor.fieldsInNumberOrder;
			}

			internal IDictionary<string, FieldDescriptor> ByJsonName()
			{
				return messageDescriptor.jsonFieldMap;
			}
		}

		private static readonly HashSet<string> WellKnownTypeNames = new HashSet<string> { "google/protobuf/any.proto", "google/protobuf/api.proto", "google/protobuf/duration.proto", "google/protobuf/empty.proto", "google/protobuf/wrappers.proto", "google/protobuf/timestamp.proto", "google/protobuf/field_mask.proto", "google/protobuf/source_context.proto", "google/protobuf/struct.proto", "google/protobuf/type.proto" };

		private readonly IList<FieldDescriptor> fieldsInDeclarationOrder;

		private readonly IList<FieldDescriptor> fieldsInNumberOrder;

		private readonly IDictionary<string, FieldDescriptor> jsonFieldMap;

		[CompilerGenerated]
		private readonly DescriptorProto _003CProto_003Ek__BackingField;

		[CompilerGenerated]
		private readonly Type _003CClrType_003Ek__BackingField;

		[CompilerGenerated]
		private readonly MessageParser _003CParser_003Ek__BackingField;

		[CompilerGenerated]
		private readonly MessageDescriptor _003CContainingType_003Ek__BackingField;

		[CompilerGenerated]
		private readonly FieldCollection _003CFields_003Ek__BackingField;

		[CompilerGenerated]
		private readonly IList<MessageDescriptor> _003CNestedTypes_003Ek__BackingField;

		[CompilerGenerated]
		private readonly IList<EnumDescriptor> _003CEnumTypes_003Ek__BackingField;

		[CompilerGenerated]
		private readonly IList<OneofDescriptor> _003COneofs_003Ek__BackingField;

		public override string Name
		{
			get
			{
				return Proto.Name;
			}
		}

		internal DescriptorProto Proto
		{
			[CompilerGenerated]
			get
			{
				return _003CProto_003Ek__BackingField;
			}
		}

		public Type ClrType
		{
			[CompilerGenerated]
			get
			{
				return _003CClrType_003Ek__BackingField;
			}
		}

		public MessageParser Parser
		{
			[CompilerGenerated]
			get
			{
				return _003CParser_003Ek__BackingField;
			}
		}

		internal bool IsWellKnownType
		{
			get
			{
				if (base.File.Package == "google.protobuf")
				{
					return WellKnownTypeNames.Contains(base.File.Name);
				}
				return false;
			}
		}

		internal bool IsWrapperType
		{
			get
			{
				if (base.File.Package == "google.protobuf")
				{
					return base.File.Name == "google/protobuf/wrappers.proto";
				}
				return false;
			}
		}

		public MessageDescriptor ContainingType
		{
			[CompilerGenerated]
			get
			{
				return _003CContainingType_003Ek__BackingField;
			}
		}

		public FieldCollection Fields
		{
			[CompilerGenerated]
			get
			{
				return _003CFields_003Ek__BackingField;
			}
		}

		public IList<MessageDescriptor> NestedTypes
		{
			[CompilerGenerated]
			get
			{
				return _003CNestedTypes_003Ek__BackingField;
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

		public IList<OneofDescriptor> Oneofs
		{
			[CompilerGenerated]
			get
			{
				return _003COneofs_003Ek__BackingField;
			}
		}

		internal MessageDescriptor(DescriptorProto proto, FileDescriptor file, MessageDescriptor parent, int typeIndex, GeneratedClrTypeInfo generatedCodeInfo)
			: base(file, file.ComputeFullName(parent, proto.Name), typeIndex)
		{
			MessageDescriptor messageDescriptor = this;
			_003CProto_003Ek__BackingField = proto;
			GeneratedClrTypeInfo generatedClrTypeInfo = generatedCodeInfo;
			_003CParser_003Ek__BackingField = ((generatedClrTypeInfo != null) ? generatedClrTypeInfo.Parser : null);
			GeneratedClrTypeInfo generatedClrTypeInfo2 = generatedCodeInfo;
			_003CClrType_003Ek__BackingField = ((generatedClrTypeInfo2 != null) ? generatedClrTypeInfo2.ClrType : null);
			_003CContainingType_003Ek__BackingField = parent;
			_003COneofs_003Ek__BackingField = DescriptorUtil.ConvertAndMakeReadOnly(proto.OneofDecl, (OneofDescriptorProto oneof, int index) => new OneofDescriptor(oneof, file, messageDescriptor, index, generatedCodeInfo.OneofNames[index]));
			_003CNestedTypes_003Ek__BackingField = DescriptorUtil.ConvertAndMakeReadOnly(proto.NestedType, (DescriptorProto type, int index) => new MessageDescriptor(type, file, messageDescriptor, index, generatedCodeInfo.NestedTypes[index]));
			_003CEnumTypes_003Ek__BackingField = DescriptorUtil.ConvertAndMakeReadOnly(proto.EnumType, (EnumDescriptorProto type, int index) => new EnumDescriptor(type, file, messageDescriptor, index, generatedCodeInfo.NestedEnums[index]));
			fieldsInDeclarationOrder = DescriptorUtil.ConvertAndMakeReadOnly(proto.Field, delegate(FieldDescriptorProto field, int index)
			{
				FileDescriptor fileDescriptor = file;
				MessageDescriptor parent2 = messageDescriptor;
				GeneratedClrTypeInfo generatedClrTypeInfo3 = generatedCodeInfo;
				return new FieldDescriptor(field, fileDescriptor, parent2, index, (generatedClrTypeInfo3 != null) ? generatedClrTypeInfo3.PropertyNames[index] : null);
			});
			fieldsInNumberOrder = new ReadOnlyCollection<FieldDescriptor>(fieldsInDeclarationOrder.OrderBy((FieldDescriptor field) => field.FieldNumber).ToArray());
			jsonFieldMap = CreateJsonFieldMap(fieldsInNumberOrder);
			file.DescriptorPool.AddSymbol(this);
			_003CFields_003Ek__BackingField = new FieldCollection(this);
		}

		private static Collections.ReadOnlyDictionary<string, FieldDescriptor> CreateJsonFieldMap(IList<FieldDescriptor> fields)
		{
			Dictionary<string, FieldDescriptor> dictionary = new Dictionary<string, FieldDescriptor>();
			foreach (FieldDescriptor field in fields)
			{
				dictionary[field.Name] = field;
				dictionary[field.JsonName] = field;
			}
			return new Collections.ReadOnlyDictionary<string, FieldDescriptor>(dictionary);
		}

		public FieldDescriptor FindFieldByName(string name)
		{
			return base.File.DescriptorPool.FindSymbol<FieldDescriptor>(base.FullName + "." + name);
		}

		public FieldDescriptor FindFieldByNumber(int number)
		{
			return base.File.DescriptorPool.FindFieldByNumber(this, number);
		}

		public T FindDescriptor<T>(string name) where T : class, IDescriptor
		{
			return base.File.DescriptorPool.FindSymbol<T>(base.FullName + "." + name);
		}

		internal void CrossLink()
		{
			foreach (MessageDescriptor nestedType in NestedTypes)
			{
				nestedType.CrossLink();
			}
			foreach (FieldDescriptor item in fieldsInDeclarationOrder)
			{
				item.CrossLink();
			}
			foreach (OneofDescriptor oneof in Oneofs)
			{
				oneof.CrossLink();
			}
		}
	}
}
