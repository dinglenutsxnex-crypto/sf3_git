namespace Google.Protobuf.Reflection
{
	internal sealed class PackageDescriptor : IDescriptor
	{
		private readonly string name;

		private readonly string fullName;

		private readonly FileDescriptor file;

		public string Name
		{
			get
			{
				return name;
			}
		}

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

		internal PackageDescriptor(string name, string fullName, FileDescriptor file)
		{
			this.file = file;
			this.fullName = fullName;
			this.name = name;
		}
	}
}
