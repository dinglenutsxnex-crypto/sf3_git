using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Google.Protobuf.Reflection
{
	public sealed class OneofDescriptor : DescriptorBase
	{
		private readonly OneofDescriptorProto proto;

		private MessageDescriptor containingType;

		private IList<FieldDescriptor> fields;

		private readonly OneofAccessor accessor;

		public override string Name
		{
			get
			{
				return proto.Name;
			}
		}

		public MessageDescriptor ContainingType
		{
			get
			{
				return containingType;
			}
		}

		public IList<FieldDescriptor> Fields
		{
			get
			{
				return fields;
			}
		}

		public OneofAccessor Accessor
		{
			get
			{
				return accessor;
			}
		}

		internal OneofDescriptor(OneofDescriptorProto proto, FileDescriptor file, MessageDescriptor parent, int index, string clrName)
			: base(file, file.ComputeFullName(parent, proto.Name), index)
		{
			this.proto = proto;
			containingType = parent;
			file.DescriptorPool.AddSymbol(this);
			accessor = CreateAccessor(clrName);
		}

		internal void CrossLink()
		{
			List<FieldDescriptor> list = new List<FieldDescriptor>();
			foreach (FieldDescriptor item in ContainingType.Fields.InDeclarationOrder())
			{
				if (item.ContainingOneof == this)
				{
					list.Add(item);
				}
			}
			fields = new ReadOnlyCollection<FieldDescriptor>(list);
		}

		private OneofAccessor CreateAccessor(string clrName)
		{
			PropertyInfo property = containingType.ClrType.GetProperty(clrName + "Case");
			if (property == null)
			{
				throw new DescriptorValidationException(this, string.Format("Property {0}Case not found in {1}", clrName, containingType.ClrType));
			}
			MethodInfo method = containingType.ClrType.GetMethod("Clear" + clrName);
			if (method == null)
			{
				throw new DescriptorValidationException(this, string.Format("Method Clear{0} not found in {1}", clrName, containingType.ClrType));
			}
			return new OneofAccessor(property, method, this);
		}
	}
}
