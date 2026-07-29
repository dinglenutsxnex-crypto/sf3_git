using System;
using System.Diagnostics;
using Google.Protobuf.Collections;

namespace Google.Protobuf.Reflection
{
	internal sealed class GeneratedCodeInfo : IMessage<GeneratedCodeInfo>, IMessage, IEquatable<GeneratedCodeInfo>, IDeepCloneable<GeneratedCodeInfo>
	{
		[DebuggerNonUserCode]
		public static class Types
		{
			internal sealed class Annotation : IMessage<Annotation>, IMessage, IEquatable<Annotation>, IDeepCloneable<Annotation>
			{
				private static readonly MessageParser<Annotation> _parser = new MessageParser<Annotation>(() => new Annotation());

				public const int PathFieldNumber = 1;

				private static readonly FieldCodec<int> _repeated_path_codec = FieldCodec.ForInt32(10u);

				private readonly RepeatedField<int> path_ = new RepeatedField<int>();

				public const int SourceFileFieldNumber = 2;

				private string sourceFile_ = "";

				public const int BeginFieldNumber = 3;

				private int begin_;

				public const int EndFieldNumber = 4;

				private int end_;

				[DebuggerNonUserCode]
				public static MessageParser<Annotation> Parser
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
						return GeneratedCodeInfo.Descriptor.NestedTypes[0];
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
				public RepeatedField<int> Path
				{
					get
					{
						return path_;
					}
				}

				[DebuggerNonUserCode]
				public string SourceFile
				{
					get
					{
						return sourceFile_;
					}
					set
					{
						sourceFile_ = ProtoPreconditions.CheckNotNull(value, "value");
					}
				}

				[DebuggerNonUserCode]
				public int Begin
				{
					get
					{
						return begin_;
					}
					set
					{
						begin_ = value;
					}
				}

				[DebuggerNonUserCode]
				public int End
				{
					get
					{
						return end_;
					}
					set
					{
						end_ = value;
					}
				}

				[DebuggerNonUserCode]
				public Annotation()
				{
				}

				[DebuggerNonUserCode]
				public Annotation(Annotation other)
					: this()
				{
					path_ = other.path_.Clone();
					sourceFile_ = other.sourceFile_;
					begin_ = other.begin_;
					end_ = other.end_;
				}

				[DebuggerNonUserCode]
				public Annotation Clone()
				{
					return new Annotation(this);
				}

				[DebuggerNonUserCode]
				public override bool Equals(object other)
				{
					return Equals(other as Annotation);
				}

				[DebuggerNonUserCode]
				public bool Equals(Annotation other)
				{
					if (other == null)
					{
						return false;
					}
					if (other == this)
					{
						return true;
					}
					if (!path_.Equals(other.path_))
					{
						return false;
					}
					if (SourceFile != other.SourceFile)
					{
						return false;
					}
					if (Begin != other.Begin)
					{
						return false;
					}
					if (End != other.End)
					{
						return false;
					}
					return true;
				}

				[DebuggerNonUserCode]
				public override int GetHashCode()
				{
					int num = 1;
					num ^= path_.GetHashCode();
					if (SourceFile.Length != 0)
					{
						num ^= SourceFile.GetHashCode();
					}
					if (Begin != 0)
					{
						num ^= Begin.GetHashCode();
					}
					if (End != 0)
					{
						num ^= End.GetHashCode();
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
					path_.WriteTo(output, _repeated_path_codec);
					if (SourceFile.Length != 0)
					{
						output.WriteRawTag(18);
						output.WriteString(SourceFile);
					}
					if (Begin != 0)
					{
						output.WriteRawTag(24);
						output.WriteInt32(Begin);
					}
					if (End != 0)
					{
						output.WriteRawTag(32);
						output.WriteInt32(End);
					}
				}

				[DebuggerNonUserCode]
				public int CalculateSize()
				{
					int num = 0;
					num += path_.CalculateSize(_repeated_path_codec);
					if (SourceFile.Length != 0)
					{
						num += 1 + CodedOutputStream.ComputeStringSize(SourceFile);
					}
					if (Begin != 0)
					{
						num += 1 + CodedOutputStream.ComputeInt32Size(Begin);
					}
					if (End != 0)
					{
						num += 1 + CodedOutputStream.ComputeInt32Size(End);
					}
					return num;
				}

				[DebuggerNonUserCode]
				public void MergeFrom(Annotation other)
				{
					if (other != null)
					{
						path_.Add(other.path_);
						if (other.SourceFile.Length != 0)
						{
							SourceFile = other.SourceFile;
						}
						if (other.Begin != 0)
						{
							Begin = other.Begin;
						}
						if (other.End != 0)
						{
							End = other.End;
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
						case 8u:
						case 10u:
							path_.AddEntriesFrom(input, _repeated_path_codec);
							break;
						case 18u:
							SourceFile = input.ReadString();
							break;
						case 24u:
							Begin = input.ReadInt32();
							break;
						case 32u:
							End = input.ReadInt32();
							break;
						}
					}
				}
			}
		}

		private static readonly MessageParser<GeneratedCodeInfo> _parser = new MessageParser<GeneratedCodeInfo>(() => new GeneratedCodeInfo());

		public const int AnnotationFieldNumber = 1;

		private static readonly FieldCodec<Types.Annotation> _repeated_annotation_codec = FieldCodec.ForMessage(10u, Types.Annotation.Parser);

		private readonly RepeatedField<Types.Annotation> annotation_ = new RepeatedField<Types.Annotation>();

		[DebuggerNonUserCode]
		public static MessageParser<GeneratedCodeInfo> Parser
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
				return DescriptorReflection.Descriptor.MessageTypes[19];
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
		public RepeatedField<Types.Annotation> Annotation
		{
			get
			{
				return annotation_;
			}
		}

		[DebuggerNonUserCode]
		public GeneratedCodeInfo()
		{
		}

		[DebuggerNonUserCode]
		public GeneratedCodeInfo(GeneratedCodeInfo other)
			: this()
		{
			annotation_ = other.annotation_.Clone();
		}

		[DebuggerNonUserCode]
		public GeneratedCodeInfo Clone()
		{
			return new GeneratedCodeInfo(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as GeneratedCodeInfo);
		}

		[DebuggerNonUserCode]
		public bool Equals(GeneratedCodeInfo other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (!annotation_.Equals(other.annotation_))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			return 1 ^ annotation_.GetHashCode();
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			annotation_.WriteTo(output, _repeated_annotation_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			return 0 + annotation_.CalculateSize(_repeated_annotation_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(GeneratedCodeInfo other)
		{
			if (other != null)
			{
				annotation_.Add(other.annotation_);
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
					annotation_.AddEntriesFrom(input, _repeated_annotation_codec);
				}
			}
		}
	}
}
