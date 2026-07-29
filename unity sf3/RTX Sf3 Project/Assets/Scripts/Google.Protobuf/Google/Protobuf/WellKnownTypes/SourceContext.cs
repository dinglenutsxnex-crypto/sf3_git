using System;
using System.Diagnostics;
using Google.Protobuf.Reflection;

namespace Google.Protobuf.WellKnownTypes
{
	public sealed class SourceContext : IMessage<SourceContext>, IMessage, IEquatable<SourceContext>, IDeepCloneable<SourceContext>
	{
		private static readonly MessageParser<SourceContext> _parser = new MessageParser<SourceContext>(() => new SourceContext());

		public const int FileNameFieldNumber = 1;

		private string fileName_ = "";

		[DebuggerNonUserCode]
		public static MessageParser<SourceContext> Parser
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
				return SourceContextReflection.Descriptor.MessageTypes[0];
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
		public string FileName
		{
			get
			{
				return fileName_;
			}
			set
			{
				fileName_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public SourceContext()
		{
		}

		[DebuggerNonUserCode]
		public SourceContext(SourceContext other)
			: this()
		{
			fileName_ = other.fileName_;
		}

		[DebuggerNonUserCode]
		public SourceContext Clone()
		{
			return new SourceContext(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as SourceContext);
		}

		[DebuggerNonUserCode]
		public bool Equals(SourceContext other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (FileName != other.FileName)
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (FileName.Length != 0)
			{
				num ^= FileName.GetHashCode();
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
			if (FileName.Length != 0)
			{
				output.WriteRawTag(10);
				output.WriteString(FileName);
			}
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (FileName.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(FileName);
			}
			return num;
		}

		[DebuggerNonUserCode]
		public void MergeFrom(SourceContext other)
		{
			if (other != null && other.FileName.Length != 0)
			{
				FileName = other.FileName;
			}
		}

		[DebuggerNonUserCode]
		public void MergeFrom(CodedInputStream input)
		{
			uint num;
			while ((num = input.ReadTag()) != 0)
			{
				if (num != 10)
				{
					input.SkipLastField();
				}
				else
				{
					FileName = input.ReadString();
				}
			}
		}
	}
}
