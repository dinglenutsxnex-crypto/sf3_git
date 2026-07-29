using System;
using System.Diagnostics;
using Google.Protobuf.Collections;

namespace Google.Protobuf.Reflection
{
	internal sealed class FileDescriptorSet : IMessage<FileDescriptorSet>, IMessage, IEquatable<FileDescriptorSet>, IDeepCloneable<FileDescriptorSet>
	{
		private static readonly MessageParser<FileDescriptorSet> _parser = new MessageParser<FileDescriptorSet>(() => new FileDescriptorSet());

		public const int FileFieldNumber = 1;

		private static readonly FieldCodec<FileDescriptorProto> _repeated_file_codec = FieldCodec.ForMessage(10u, FileDescriptorProto.Parser);

		private readonly RepeatedField<FileDescriptorProto> file_ = new RepeatedField<FileDescriptorProto>();

		[DebuggerNonUserCode]
		public static MessageParser<FileDescriptorSet> Parser
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
				return DescriptorReflection.Descriptor.MessageTypes[0];
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
		public RepeatedField<FileDescriptorProto> File
		{
			get
			{
				return file_;
			}
		}

		[DebuggerNonUserCode]
		public FileDescriptorSet()
		{
		}

		[DebuggerNonUserCode]
		public FileDescriptorSet(FileDescriptorSet other)
			: this()
		{
			file_ = other.file_.Clone();
		}

		[DebuggerNonUserCode]
		public FileDescriptorSet Clone()
		{
			return new FileDescriptorSet(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as FileDescriptorSet);
		}

		[DebuggerNonUserCode]
		public bool Equals(FileDescriptorSet other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (!file_.Equals(other.file_))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			return 1 ^ file_.GetHashCode();
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			file_.WriteTo(output, _repeated_file_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			return 0 + file_.CalculateSize(_repeated_file_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(FileDescriptorSet other)
		{
			if (other != null)
			{
				file_.Add(other.file_);
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
					file_.AddEntriesFrom(input, _repeated_file_codec);
				}
			}
		}
	}
}
