using System;
using System.Runtime.CompilerServices;

namespace Google.Protobuf.Reflection
{
	public sealed class GeneratedClrTypeInfo
	{
		private static readonly string[] EmptyNames = new string[0];

		private static readonly GeneratedClrTypeInfo[] EmptyCodeInfo = new GeneratedClrTypeInfo[0];

		[CompilerGenerated]
		private readonly MessageParser _003CParser_003Ek__BackingField;

		[CompilerGenerated]
		private readonly string[] _003CPropertyNames_003Ek__BackingField;

		[CompilerGenerated]
		private readonly string[] _003COneofNames_003Ek__BackingField;

		[CompilerGenerated]
		private readonly GeneratedClrTypeInfo[] _003CNestedTypes_003Ek__BackingField;

		[CompilerGenerated]
		private readonly Type[] _003CNestedEnums_003Ek__BackingField;

		public Type ClrType { get; private set; }

		public MessageParser Parser
		{
			[CompilerGenerated]
			get
			{
				return _003CParser_003Ek__BackingField;
			}
		}

		public string[] PropertyNames
		{
			[CompilerGenerated]
			get
			{
				return _003CPropertyNames_003Ek__BackingField;
			}
		}

		public string[] OneofNames
		{
			[CompilerGenerated]
			get
			{
				return _003COneofNames_003Ek__BackingField;
			}
		}

		public GeneratedClrTypeInfo[] NestedTypes
		{
			[CompilerGenerated]
			get
			{
				return _003CNestedTypes_003Ek__BackingField;
			}
		}

		public Type[] NestedEnums
		{
			[CompilerGenerated]
			get
			{
				return _003CNestedEnums_003Ek__BackingField;
			}
		}

		public GeneratedClrTypeInfo(Type clrType, MessageParser parser, string[] propertyNames, string[] oneofNames, Type[] nestedEnums, GeneratedClrTypeInfo[] nestedTypes)
		{
			_003CNestedTypes_003Ek__BackingField = nestedTypes ?? EmptyCodeInfo;
			_003CNestedEnums_003Ek__BackingField = nestedEnums ?? ReflectionUtil.EmptyTypes;
			ClrType = clrType;
			_003CParser_003Ek__BackingField = parser;
			_003CPropertyNames_003Ek__BackingField = propertyNames ?? EmptyNames;
			_003COneofNames_003Ek__BackingField = oneofNames ?? EmptyNames;
		}

		public GeneratedClrTypeInfo(Type[] nestedEnums, GeneratedClrTypeInfo[] nestedTypes)
			: this(null, null, null, null, nestedEnums, nestedTypes)
		{
		}
	}
}
