using System;
using System.Diagnostics;
using Google.Protobuf.Reflection;

namespace Google.Protobuf.WellKnownTypes
{
	public sealed class Mixin : IMessage<Mixin>, IMessage, IEquatable<Mixin>, IDeepCloneable<Mixin>
	{
		private static readonly MessageParser<Mixin> _parser = new MessageParser<Mixin>(() => new Mixin());

		public const int NameFieldNumber = 1;

		private string name_ = "";

		public const int RootFieldNumber = 2;

		private string root_ = "";

		[DebuggerNonUserCode]
		public static MessageParser<Mixin> Parser
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
				return ApiReflection.Descriptor.MessageTypes[2];
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
		public string Root
		{
			get
			{
				return root_;
			}
			set
			{
				root_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public Mixin()
		{
		}

		[DebuggerNonUserCode]
		public Mixin(Mixin other)
			: this()
		{
			name_ = other.name_;
			root_ = other.root_;
		}

		[DebuggerNonUserCode]
		public Mixin Clone()
		{
			return new Mixin(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as Mixin);
		}

		[DebuggerNonUserCode]
		public bool Equals(Mixin other)
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
			if (Root != other.Root)
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
			if (Root.Length != 0)
			{
				num ^= Root.GetHashCode();
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
			if (Root.Length != 0)
			{
				output.WriteRawTag(18);
				output.WriteString(Root);
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
			if (Root.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(Root);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(Mixin other)
		{
			if (other != null)
			{
				if (other.Name.Length != 0)
				{
					Name = other.Name;
				}
				if (other.Root.Length != 0)
				{
					Root = other.Root;
				}
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
					Root = input.ReadString();
					break;
				}
			}
		}
	}
}
