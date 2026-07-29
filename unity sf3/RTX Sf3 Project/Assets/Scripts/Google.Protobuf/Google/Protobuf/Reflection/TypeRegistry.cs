using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Google.Protobuf.Reflection
{
	public sealed class TypeRegistry
	{
		private class Builder
		{
			private readonly Dictionary<string, MessageDescriptor> types;

			private readonly HashSet<string> fileDescriptorNames;

			internal Builder()
			{
				types = new Dictionary<string, MessageDescriptor>();
				fileDescriptorNames = new HashSet<string>();
			}

			internal void AddFile(FileDescriptor fileDescriptor)
			{
				if (!fileDescriptorNames.Add(fileDescriptor.Name))
				{
					return;
				}
				foreach (FileDescriptor dependency in fileDescriptor.Dependencies)
				{
					AddFile(dependency);
				}
				foreach (MessageDescriptor messageType in fileDescriptor.MessageTypes)
				{
					AddMessage(messageType);
				}
			}

			private void AddMessage(MessageDescriptor messageDescriptor)
			{
				foreach (MessageDescriptor nestedType in messageDescriptor.NestedTypes)
				{
					AddMessage(nestedType);
				}
				types[messageDescriptor.FullName] = messageDescriptor;
			}

			internal TypeRegistry Build()
			{
				return new TypeRegistry(types);
			}
		}

		[CompilerGenerated]
		private static readonly TypeRegistry _003CEmpty_003Ek__BackingField = new TypeRegistry(new Dictionary<string, MessageDescriptor>());

		private readonly Dictionary<string, MessageDescriptor> fullNameToMessageMap;

		public static TypeRegistry Empty
		{
			[CompilerGenerated]
			get
			{
				return _003CEmpty_003Ek__BackingField;
			}
		}

		private TypeRegistry(Dictionary<string, MessageDescriptor> fullNameToMessageMap)
		{
			this.fullNameToMessageMap = fullNameToMessageMap;
		}

		public MessageDescriptor Find(string fullName)
		{
			MessageDescriptor value;
			fullNameToMessageMap.TryGetValue(fullName, out value);
			return value;
		}

		public static TypeRegistry FromFiles(params FileDescriptor[] fileDescriptors)
		{
			return FromFiles((IEnumerable<FileDescriptor>)fileDescriptors);
		}

		public static TypeRegistry FromFiles(IEnumerable<FileDescriptor> fileDescriptors)
		{
			ProtoPreconditions.CheckNotNull(fileDescriptors, "fileDescriptors");
			Builder builder = new Builder();
			foreach (FileDescriptor fileDescriptor in fileDescriptors)
			{
				builder.AddFile(fileDescriptor);
			}
			return builder.Build();
		}

		public static TypeRegistry FromMessages(params MessageDescriptor[] messageDescriptors)
		{
			return FromMessages((IEnumerable<MessageDescriptor>)messageDescriptors);
		}

		public static TypeRegistry FromMessages(IEnumerable<MessageDescriptor> messageDescriptors)
		{
			ProtoPreconditions.CheckNotNull(messageDescriptors, "messageDescriptors");
			return FromFiles(messageDescriptors.Select((MessageDescriptor md) => md.File));
		}
	}
}
