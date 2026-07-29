using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;

namespace Google.Protobuf.WellKnownTypes
{
	public sealed class FieldMask : IMessage<FieldMask>, IMessage, IEquatable<FieldMask>, IDeepCloneable<FieldMask>, ICustomDiagnosticMessage
	{
		private static readonly MessageParser<FieldMask> _parser = new MessageParser<FieldMask>(() => new FieldMask());

		public const int PathsFieldNumber = 1;

		private static readonly FieldCodec<string> _repeated_paths_codec = FieldCodec.ForString(10u);

		private readonly RepeatedField<string> paths_ = new RepeatedField<string>();

		[DebuggerNonUserCode]
		public static MessageParser<FieldMask> Parser
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
				return FieldMaskReflection.Descriptor.MessageTypes[0];
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
		public RepeatedField<string> Paths
		{
			get
			{
				return paths_;
			}
		}

		[DebuggerNonUserCode]
		public FieldMask()
		{
		}

		[DebuggerNonUserCode]
		public FieldMask(FieldMask other)
			: this()
		{
			paths_ = other.paths_.Clone();
		}

		[DebuggerNonUserCode]
		public FieldMask Clone()
		{
			return new FieldMask(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as FieldMask);
		}

		[DebuggerNonUserCode]
		public bool Equals(FieldMask other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (!paths_.Equals(other.paths_))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			return 1 ^ paths_.GetHashCode();
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			paths_.WriteTo(output, _repeated_paths_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			return 0 + paths_.CalculateSize(_repeated_paths_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(FieldMask other)
		{
			if (other != null)
			{
				paths_.Add(other.paths_);
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
					paths_.AddEntriesFrom(input, _repeated_paths_codec);
				}
			}
		}

		internal static string ToJson(IList<string> paths, bool diagnosticOnly)
		{
			string text = paths.FirstOrDefault((string p) => !ValidatePath(p));
			if (text == null)
			{
				StringWriter stringWriter = new StringWriter();
				IEnumerable<string> source = paths.Select(JsonFormatter.ToJsonName);
				JsonFormatter.WriteString(stringWriter, string.Join(",", source.ToArray()));
				return stringWriter.ToString();
			}
			if (diagnosticOnly)
			{
				StringWriter stringWriter2 = new StringWriter();
				stringWriter2.Write("{ \"@warning\": \"Invalid FieldMask\", \"paths\": ");
				JsonFormatter.Default.WriteList(stringWriter2, (IList)paths);
				stringWriter2.Write(" }");
				return stringWriter2.ToString();
			}
			throw new InvalidOperationException(string.Format("Invalid field mask to be converted to JSON: {0}", text));
		}

		private static bool ValidatePath(string input)
		{
			for (int i = 0; i < input.Length; i++)
			{
				char c = input[i];
				if (c >= 'A' && c <= 'Z')
				{
					return false;
				}
				if (c == '_' && i < input.Length - 1)
				{
					char c2 = input[i + 1];
					if (c2 < 'a' || c2 > 'z')
					{
						return false;
					}
				}
			}
			return true;
		}

		public string ToDiagnosticString()
		{
			return ToJson(Paths, true);
		}
	}
}
