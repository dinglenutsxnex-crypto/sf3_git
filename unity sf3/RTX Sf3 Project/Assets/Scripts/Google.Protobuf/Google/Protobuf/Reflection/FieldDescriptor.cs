using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Google.Protobuf.Reflection
{
	public sealed class FieldDescriptor : DescriptorBase, IComparable<FieldDescriptor>
	{
		private EnumDescriptor enumType;

		private MessageDescriptor messageType;

		private FieldType fieldType;

		private readonly string propertyName;

		private IFieldAccessor accessor;

		[CompilerGenerated]
		private readonly MessageDescriptor _003CContainingType_003Ek__BackingField;

		[CompilerGenerated]
		private readonly OneofDescriptor _003CContainingOneof_003Ek__BackingField;

		[CompilerGenerated]
		private readonly string _003CJsonName_003Ek__BackingField;

		[CompilerGenerated]
		private readonly FieldDescriptorProto _003CProto_003Ek__BackingField;

		public MessageDescriptor ContainingType
		{
			[CompilerGenerated]
			get
			{
				return _003CContainingType_003Ek__BackingField;
			}
		}

		public OneofDescriptor ContainingOneof
		{
			[CompilerGenerated]
			get
			{
				return _003CContainingOneof_003Ek__BackingField;
			}
		}

		public string JsonName
		{
			[CompilerGenerated]
			get
			{
				return _003CJsonName_003Ek__BackingField;
			}
		}

		internal FieldDescriptorProto Proto
		{
			[CompilerGenerated]
			get
			{
				return _003CProto_003Ek__BackingField;
			}
		}

		public override string Name
		{
			get
			{
				return Proto.Name;
			}
		}

		public IFieldAccessor Accessor
		{
			get
			{
				return accessor;
			}
		}

		public bool IsRepeated
		{
			get
			{
				return Proto.Label == FieldDescriptorProto.Types.Label.Repeated;
			}
		}

		public bool IsMap
		{
			get
			{
				if (fieldType == FieldType.Message && messageType.Proto.Options != null)
				{
					return messageType.Proto.Options.MapEntry;
				}
				return false;
			}
		}

		public bool IsPacked
		{
			get
			{
				if (Proto.Options != null)
				{
					return Proto.Options.Packed;
				}
				return true;
			}
		}

		public FieldType FieldType
		{
			get
			{
				return fieldType;
			}
		}

		public int FieldNumber
		{
			get
			{
				return Proto.Number;
			}
		}

		public EnumDescriptor EnumType
		{
			get
			{
				if (fieldType != FieldType.Enum)
				{
					throw new InvalidOperationException("EnumType is only valid for enum fields.");
				}
				return enumType;
			}
		}

		public MessageDescriptor MessageType
		{
			get
			{
				if (fieldType != FieldType.Message)
				{
					throw new InvalidOperationException("MessageType is only valid for message fields.");
				}
				return messageType;
			}
		}

		internal FieldDescriptor(FieldDescriptorProto proto, FileDescriptor file, MessageDescriptor parent, int index, string propertyName)
			: base(file, file.ComputeFullName(parent, proto.Name), index)
		{
			_003CProto_003Ek__BackingField = proto;
			if (proto.Type != 0)
			{
				fieldType = GetFieldTypeFromProtoType(proto.Type);
			}
			if (FieldNumber <= 0)
			{
				throw new DescriptorValidationException(this, "Field numbers must be positive integers.");
			}
			_003CContainingType_003Ek__BackingField = parent;
			if (proto.OneofIndex != -1)
			{
				if (proto.OneofIndex < 0 || proto.OneofIndex >= parent.Proto.OneofDecl.Count)
				{
					throw new DescriptorValidationException(this, string.Format("FieldDescriptorProto.oneof_index is out of range for type {0}", parent.Name));
				}
				_003CContainingOneof_003Ek__BackingField = parent.Oneofs[proto.OneofIndex];
			}
			file.DescriptorPool.AddSymbol(this);
			this.propertyName = propertyName;
			_003CJsonName_003Ek__BackingField = ((Proto.JsonName == "") ? JsonFormatter.ToJsonName(Proto.Name) : Proto.JsonName);
		}

		private static FieldType GetFieldTypeFromProtoType(FieldDescriptorProto.Types.Type type)
		{
			switch (type)
			{
			case FieldDescriptorProto.Types.Type.Double:
				return FieldType.Double;
			case FieldDescriptorProto.Types.Type.Float:
				return FieldType.Float;
			case FieldDescriptorProto.Types.Type.Int64:
				return FieldType.Int64;
			case FieldDescriptorProto.Types.Type.Uint64:
				return FieldType.UInt64;
			case FieldDescriptorProto.Types.Type.Int32:
				return FieldType.Int32;
			case FieldDescriptorProto.Types.Type.Fixed64:
				return FieldType.Fixed64;
			case FieldDescriptorProto.Types.Type.Fixed32:
				return FieldType.Fixed32;
			case FieldDescriptorProto.Types.Type.Bool:
				return FieldType.Bool;
			case FieldDescriptorProto.Types.Type.String:
				return FieldType.String;
			case FieldDescriptorProto.Types.Type.Group:
				return FieldType.Group;
			case FieldDescriptorProto.Types.Type.Message:
				return FieldType.Message;
			case FieldDescriptorProto.Types.Type.Bytes:
				return FieldType.Bytes;
			case FieldDescriptorProto.Types.Type.Uint32:
				return FieldType.UInt32;
			case FieldDescriptorProto.Types.Type.Enum:
				return FieldType.Enum;
			case FieldDescriptorProto.Types.Type.Sfixed32:
				return FieldType.SFixed32;
			case FieldDescriptorProto.Types.Type.Sfixed64:
				return FieldType.SFixed64;
			case FieldDescriptorProto.Types.Type.Sint32:
				return FieldType.SInt32;
			case FieldDescriptorProto.Types.Type.Sint64:
				return FieldType.SInt64;
			default:
				throw new ArgumentException("Invalid type specified");
			}
		}

		public int CompareTo(FieldDescriptor other)
		{
			if (other.ContainingType != ContainingType)
			{
				throw new ArgumentException("FieldDescriptors can only be compared to other FieldDescriptors for fields of the same message type.");
			}
			return FieldNumber - other.FieldNumber;
		}

		internal void CrossLink()
		{
			if (Proto.TypeName != "")
			{
				IDescriptor descriptor = base.File.DescriptorPool.LookupSymbol(Proto.TypeName, this);
				if (Proto.Type != 0)
				{
					if (descriptor is MessageDescriptor)
					{
						fieldType = FieldType.Message;
					}
					else
					{
						if (!(descriptor is EnumDescriptor))
						{
							throw new DescriptorValidationException(this, string.Format("\"{0}\" is not a type.", Proto.TypeName));
						}
						fieldType = FieldType.Enum;
					}
				}
				if (fieldType == FieldType.Message)
				{
					if (!(descriptor is MessageDescriptor))
					{
						throw new DescriptorValidationException(this, string.Format("\"{0}\" is not a message type.", Proto.TypeName));
					}
					messageType = (MessageDescriptor)descriptor;
					if (Proto.DefaultValue != "")
					{
						throw new DescriptorValidationException(this, "Messages can't have default values.");
					}
				}
				else
				{
					if (fieldType != FieldType.Enum)
					{
						throw new DescriptorValidationException(this, "Field with primitive type has type_name.");
					}
					if (!(descriptor is EnumDescriptor))
					{
						throw new DescriptorValidationException(this, string.Format("\"{0}\" is not an enum type.", Proto.TypeName));
					}
					enumType = (EnumDescriptor)descriptor;
				}
			}
			else if (fieldType == FieldType.Message || fieldType == FieldType.Enum)
			{
				throw new DescriptorValidationException(this, "Field with message or enum type missing type_name.");
			}
			base.File.DescriptorPool.AddFieldByNumber(this);
			if (ContainingType != null && ContainingType.Proto.Options != null && ContainingType.Proto.Options.MessageSetWireFormat)
			{
				throw new DescriptorValidationException(this, "MessageSet format is not supported.");
			}
			accessor = CreateAccessor();
		}

		private IFieldAccessor CreateAccessor()
		{
			if (propertyName == null)
			{
				return null;
			}
			PropertyInfo property = ContainingType.ClrType.GetProperty(propertyName);
			if (property == null)
			{
				throw new DescriptorValidationException(this, string.Format("Property {0} not found in {1}", propertyName, ContainingType.ClrType));
			}
			if (!IsMap)
			{
				if (!IsRepeated)
				{
					return new SingleFieldAccessor(property, this);
				}
				return new RepeatedFieldAccessor(property, this);
			}
			return new MapFieldAccessor(property, this);
		}
	}
}
