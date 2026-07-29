namespace Google.Protobuf.Reflection
{
	public abstract class DescriptorBase : IDescriptor
	{
		private readonly FileDescriptor file;

		private readonly string fullName;

		private readonly int index;

		public int Index
		{
			get
			{
				return index;
			}
		}

		public abstract string Name { get; }

		public string FullName
		{
			get
			{
				return fullName;
			}
		}

		public FileDescriptor File
		{
			get
			{
				return file;
			}
		}

		internal DescriptorBase(FileDescriptor file, string fullName, int index)
		{
			this.file = file;
			this.fullName = fullName;
			this.index = index;
		}
	}
}
