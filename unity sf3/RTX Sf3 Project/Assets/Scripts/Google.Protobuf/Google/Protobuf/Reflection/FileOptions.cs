using System;
using System.Diagnostics;
using Google.Protobuf.Collections;

namespace Google.Protobuf.Reflection
{
	internal sealed class FileOptions : IMessage<FileOptions>, IMessage, IEquatable<FileOptions>, IDeepCloneable<FileOptions>
	{
		[DebuggerNonUserCode]
		public static class Types
		{
			internal enum OptimizeMode
			{
				[OriginalName("SPEED")]
				Speed = 1,
				[OriginalName("CODE_SIZE")]
				CodeSize = 2,
				[OriginalName("LITE_RUNTIME")]
				LiteRuntime = 3
			}
		}

		private static readonly MessageParser<FileOptions> _parser = new MessageParser<FileOptions>(() => new FileOptions());

		public const int JavaPackageFieldNumber = 1;

		private string javaPackage_ = "";

		public const int JavaOuterClassnameFieldNumber = 8;

		private string javaOuterClassname_ = "";

		public const int JavaMultipleFilesFieldNumber = 10;

		private bool javaMultipleFiles_;

		public const int JavaGenerateEqualsAndHashFieldNumber = 20;

		private bool javaGenerateEqualsAndHash_;

		public const int JavaStringCheckUtf8FieldNumber = 27;

		private bool javaStringCheckUtf8_;

		public const int OptimizeForFieldNumber = 9;

		private Types.OptimizeMode optimizeFor_;

		public const int GoPackageFieldNumber = 11;

		private string goPackage_ = "";

		public const int CcGenericServicesFieldNumber = 16;

		private bool ccGenericServices_;

		public const int JavaGenericServicesFieldNumber = 17;

		private bool javaGenericServices_;

		public const int PyGenericServicesFieldNumber = 18;

		private bool pyGenericServices_;

		public const int DeprecatedFieldNumber = 23;

		private bool deprecated_;

		public const int CcEnableArenasFieldNumber = 31;

		private bool ccEnableArenas_;

		public const int ObjcClassPrefixFieldNumber = 36;

		private string objcClassPrefix_ = "";

		public const int CsharpNamespaceFieldNumber = 37;

		private string csharpNamespace_ = "";

		public const int SwiftPrefixFieldNumber = 39;

		private string swiftPrefix_ = "";

		public const int UninterpretedOptionFieldNumber = 999;

		private static readonly FieldCodec<UninterpretedOption> _repeated_uninterpretedOption_codec = FieldCodec.ForMessage(7994u, Google.Protobuf.Reflection.UninterpretedOption.Parser);

		private readonly RepeatedField<UninterpretedOption> uninterpretedOption_ = new RepeatedField<UninterpretedOption>();

		[DebuggerNonUserCode]
		public static MessageParser<FileOptions> Parser
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
				return DescriptorReflection.Descriptor.MessageTypes[9];
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
		public string JavaPackage
		{
			get
			{
				return javaPackage_;
			}
			set
			{
				javaPackage_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public string JavaOuterClassname
		{
			get
			{
				return javaOuterClassname_;
			}
			set
			{
				javaOuterClassname_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public bool JavaMultipleFiles
		{
			get
			{
				return javaMultipleFiles_;
			}
			set
			{
				javaMultipleFiles_ = value;
			}
		}

		[Obsolete]
		[DebuggerNonUserCode]
		public bool JavaGenerateEqualsAndHash
		{
			get
			{
				return javaGenerateEqualsAndHash_;
			}
			set
			{
				javaGenerateEqualsAndHash_ = value;
			}
		}

		[DebuggerNonUserCode]
		public bool JavaStringCheckUtf8
		{
			get
			{
				return javaStringCheckUtf8_;
			}
			set
			{
				javaStringCheckUtf8_ = value;
			}
		}

		[DebuggerNonUserCode]
		public Types.OptimizeMode OptimizeFor
		{
			get
			{
				return optimizeFor_;
			}
			set
			{
				optimizeFor_ = value;
			}
		}

		[DebuggerNonUserCode]
		public string GoPackage
		{
			get
			{
				return goPackage_;
			}
			set
			{
				goPackage_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public bool CcGenericServices
		{
			get
			{
				return ccGenericServices_;
			}
			set
			{
				ccGenericServices_ = value;
			}
		}

		[DebuggerNonUserCode]
		public bool JavaGenericServices
		{
			get
			{
				return javaGenericServices_;
			}
			set
			{
				javaGenericServices_ = value;
			}
		}

		[DebuggerNonUserCode]
		public bool PyGenericServices
		{
			get
			{
				return pyGenericServices_;
			}
			set
			{
				pyGenericServices_ = value;
			}
		}

		[DebuggerNonUserCode]
		public bool Deprecated
		{
			get
			{
				return deprecated_;
			}
			set
			{
				deprecated_ = value;
			}
		}

		[DebuggerNonUserCode]
		public bool CcEnableArenas
		{
			get
			{
				return ccEnableArenas_;
			}
			set
			{
				ccEnableArenas_ = value;
			}
		}

		[DebuggerNonUserCode]
		public string ObjcClassPrefix
		{
			get
			{
				return objcClassPrefix_;
			}
			set
			{
				objcClassPrefix_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public string CsharpNamespace
		{
			get
			{
				return csharpNamespace_;
			}
			set
			{
				csharpNamespace_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public string SwiftPrefix
		{
			get
			{
				return swiftPrefix_;
			}
			set
			{
				swiftPrefix_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		public RepeatedField<UninterpretedOption> UninterpretedOption
		{
			get
			{
				return uninterpretedOption_;
			}
		}

		[DebuggerNonUserCode]
		public FileOptions()
		{
		}

		[DebuggerNonUserCode]
		public FileOptions(FileOptions other)
			: this()
		{
			javaPackage_ = other.javaPackage_;
			javaOuterClassname_ = other.javaOuterClassname_;
			javaMultipleFiles_ = other.javaMultipleFiles_;
			javaGenerateEqualsAndHash_ = other.javaGenerateEqualsAndHash_;
			javaStringCheckUtf8_ = other.javaStringCheckUtf8_;
			optimizeFor_ = other.optimizeFor_;
			goPackage_ = other.goPackage_;
			ccGenericServices_ = other.ccGenericServices_;
			javaGenericServices_ = other.javaGenericServices_;
			pyGenericServices_ = other.pyGenericServices_;
			deprecated_ = other.deprecated_;
			ccEnableArenas_ = other.ccEnableArenas_;
			objcClassPrefix_ = other.objcClassPrefix_;
			csharpNamespace_ = other.csharpNamespace_;
			swiftPrefix_ = other.swiftPrefix_;
			uninterpretedOption_ = other.uninterpretedOption_.Clone();
		}

		[DebuggerNonUserCode]
		public FileOptions Clone()
		{
			return new FileOptions(this);
		}

		[DebuggerNonUserCode]
		public override bool Equals(object other)
		{
			return Equals(other as FileOptions);
		}

		[DebuggerNonUserCode]
		public bool Equals(FileOptions other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (JavaPackage != other.JavaPackage)
			{
				return false;
			}
			if (JavaOuterClassname != other.JavaOuterClassname)
			{
				return false;
			}
			if (JavaMultipleFiles != other.JavaMultipleFiles)
			{
				return false;
			}
			if (JavaGenerateEqualsAndHash != other.JavaGenerateEqualsAndHash)
			{
				return false;
			}
			if (JavaStringCheckUtf8 != other.JavaStringCheckUtf8)
			{
				return false;
			}
			if (OptimizeFor != other.OptimizeFor)
			{
				return false;
			}
			if (GoPackage != other.GoPackage)
			{
				return false;
			}
			if (CcGenericServices != other.CcGenericServices)
			{
				return false;
			}
			if (JavaGenericServices != other.JavaGenericServices)
			{
				return false;
			}
			if (PyGenericServices != other.PyGenericServices)
			{
				return false;
			}
			if (Deprecated != other.Deprecated)
			{
				return false;
			}
			if (CcEnableArenas != other.CcEnableArenas)
			{
				return false;
			}
			if (ObjcClassPrefix != other.ObjcClassPrefix)
			{
				return false;
			}
			if (CsharpNamespace != other.CsharpNamespace)
			{
				return false;
			}
			if (SwiftPrefix != other.SwiftPrefix)
			{
				return false;
			}
			if (!uninterpretedOption_.Equals(other.uninterpretedOption_))
			{
				return false;
			}
			return true;
		}

		[DebuggerNonUserCode]
		public override int GetHashCode()
		{
			int num = 1;
			if (JavaPackage.Length != 0)
			{
				num ^= JavaPackage.GetHashCode();
			}
			if (JavaOuterClassname.Length != 0)
			{
				num ^= JavaOuterClassname.GetHashCode();
			}
			if (JavaMultipleFiles)
			{
				num ^= JavaMultipleFiles.GetHashCode();
			}
			if (JavaGenerateEqualsAndHash)
			{
				num ^= JavaGenerateEqualsAndHash.GetHashCode();
			}
			if (JavaStringCheckUtf8)
			{
				num ^= JavaStringCheckUtf8.GetHashCode();
			}
			if (OptimizeFor != 0)
			{
				num ^= OptimizeFor.GetHashCode();
			}
			if (GoPackage.Length != 0)
			{
				num ^= GoPackage.GetHashCode();
			}
			if (CcGenericServices)
			{
				num ^= CcGenericServices.GetHashCode();
			}
			if (JavaGenericServices)
			{
				num ^= JavaGenericServices.GetHashCode();
			}
			if (PyGenericServices)
			{
				num ^= PyGenericServices.GetHashCode();
			}
			if (Deprecated)
			{
				num ^= Deprecated.GetHashCode();
			}
			if (CcEnableArenas)
			{
				num ^= CcEnableArenas.GetHashCode();
			}
			if (ObjcClassPrefix.Length != 0)
			{
				num ^= ObjcClassPrefix.GetHashCode();
			}
			if (CsharpNamespace.Length != 0)
			{
				num ^= CsharpNamespace.GetHashCode();
			}
			if (SwiftPrefix.Length != 0)
			{
				num ^= SwiftPrefix.GetHashCode();
			}
			return num ^ uninterpretedOption_.GetHashCode();
		}

		[DebuggerNonUserCode]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		public void WriteTo(CodedOutputStream output)
		{
			if (JavaPackage.Length != 0)
			{
				output.WriteRawTag(10);
				output.WriteString(JavaPackage);
			}
			if (JavaOuterClassname.Length != 0)
			{
				output.WriteRawTag(66);
				output.WriteString(JavaOuterClassname);
			}
			if (OptimizeFor != 0)
			{
				output.WriteRawTag(72);
				output.WriteEnum((int)OptimizeFor);
			}
			if (JavaMultipleFiles)
			{
				output.WriteRawTag(80);
				output.WriteBool(JavaMultipleFiles);
			}
			if (GoPackage.Length != 0)
			{
				output.WriteRawTag(90);
				output.WriteString(GoPackage);
			}
			if (CcGenericServices)
			{
				output.WriteRawTag(128, 1);
				output.WriteBool(CcGenericServices);
			}
			if (JavaGenericServices)
			{
				output.WriteRawTag(136, 1);
				output.WriteBool(JavaGenericServices);
			}
			if (PyGenericServices)
			{
				output.WriteRawTag(144, 1);
				output.WriteBool(PyGenericServices);
			}
			if (JavaGenerateEqualsAndHash)
			{
				output.WriteRawTag(160, 1);
				output.WriteBool(JavaGenerateEqualsAndHash);
			}
			if (Deprecated)
			{
				output.WriteRawTag(184, 1);
				output.WriteBool(Deprecated);
			}
			if (JavaStringCheckUtf8)
			{
				output.WriteRawTag(216, 1);
				output.WriteBool(JavaStringCheckUtf8);
			}
			if (CcEnableArenas)
			{
				output.WriteRawTag(248, 1);
				output.WriteBool(CcEnableArenas);
			}
			if (ObjcClassPrefix.Length != 0)
			{
				output.WriteRawTag(162, 2);
				output.WriteString(ObjcClassPrefix);
			}
			if (CsharpNamespace.Length != 0)
			{
				output.WriteRawTag(170, 2);
				output.WriteString(CsharpNamespace);
			}
			if (SwiftPrefix.Length != 0)
			{
				output.WriteRawTag(186, 2);
				output.WriteString(SwiftPrefix);
			}
			uninterpretedOption_.WriteTo(output, _repeated_uninterpretedOption_codec);
		}

		[DebuggerNonUserCode]
		public int CalculateSize()
		{
			int num = 0;
			if (JavaPackage.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(JavaPackage);
			}
			if (JavaOuterClassname.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(JavaOuterClassname);
			}
			if (JavaMultipleFiles)
			{
				num += 2;
			}
			if (JavaGenerateEqualsAndHash)
			{
				num += 3;
			}
			if (JavaStringCheckUtf8)
			{
				num += 3;
			}
			if (OptimizeFor != 0)
			{
				num += 1 + CodedOutputStream.ComputeEnumSize((int)OptimizeFor);
			}
			if (GoPackage.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(GoPackage);
			}
			if (CcGenericServices)
			{
				num += 3;
			}
			if (JavaGenericServices)
			{
				num += 3;
			}
			if (PyGenericServices)
			{
				num += 3;
			}
			if (Deprecated)
			{
				num += 3;
			}
			if (CcEnableArenas)
			{
				num += 3;
			}
			if (ObjcClassPrefix.Length != 0)
			{
				num += 2 + CodedOutputStream.ComputeStringSize(ObjcClassPrefix);
			}
			if (CsharpNamespace.Length != 0)
			{
				num += 2 + CodedOutputStream.ComputeStringSize(CsharpNamespace);
			}
			if (SwiftPrefix.Length != 0)
			{
				num += 2 + CodedOutputStream.ComputeStringSize(SwiftPrefix);
			}
			return num + uninterpretedOption_.CalculateSize(_repeated_uninterpretedOption_codec);
		}

		[DebuggerNonUserCode]
		public void MergeFrom(FileOptions other)
		{
			if (other != null)
			{
				if (other.JavaPackage.Length != 0)
				{
					JavaPackage = other.JavaPackage;
				}
				if (other.JavaOuterClassname.Length != 0)
				{
					JavaOuterClassname = other.JavaOuterClassname;
				}
				if (other.JavaMultipleFiles)
				{
					JavaMultipleFiles = other.JavaMultipleFiles;
				}
				if (other.JavaGenerateEqualsAndHash)
				{
					JavaGenerateEqualsAndHash = other.JavaGenerateEqualsAndHash;
				}
				if (other.JavaStringCheckUtf8)
				{
					JavaStringCheckUtf8 = other.JavaStringCheckUtf8;
				}
				if (other.OptimizeFor != 0)
				{
					OptimizeFor = other.OptimizeFor;
				}
				if (other.GoPackage.Length != 0)
				{
					GoPackage = other.GoPackage;
				}
				if (other.CcGenericServices)
				{
					CcGenericServices = other.CcGenericServices;
				}
				if (other.JavaGenericServices)
				{
					JavaGenericServices = other.JavaGenericServices;
				}
				if (other.PyGenericServices)
				{
					PyGenericServices = other.PyGenericServices;
				}
				if (other.Deprecated)
				{
					Deprecated = other.Deprecated;
				}
				if (other.CcEnableArenas)
				{
					CcEnableArenas = other.CcEnableArenas;
				}
				if (other.ObjcClassPrefix.Length != 0)
				{
					ObjcClassPrefix = other.ObjcClassPrefix;
				}
				if (other.CsharpNamespace.Length != 0)
				{
					CsharpNamespace = other.CsharpNamespace;
				}
				if (other.SwiftPrefix.Length != 0)
				{
					SwiftPrefix = other.SwiftPrefix;
				}
				uninterpretedOption_.Add(other.uninterpretedOption_);
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
					JavaPackage = input.ReadString();
					break;
				case 66u:
					JavaOuterClassname = input.ReadString();
					break;
				case 72u:
					optimizeFor_ = (Types.OptimizeMode)input.ReadEnum();
					break;
				case 80u:
					JavaMultipleFiles = input.ReadBool();
					break;
				case 90u:
					GoPackage = input.ReadString();
					break;
				case 128u:
					CcGenericServices = input.ReadBool();
					break;
				case 136u:
					JavaGenericServices = input.ReadBool();
					break;
				case 144u:
					PyGenericServices = input.ReadBool();
					break;
				case 160u:
					JavaGenerateEqualsAndHash = input.ReadBool();
					break;
				case 184u:
					Deprecated = input.ReadBool();
					break;
				case 216u:
					JavaStringCheckUtf8 = input.ReadBool();
					break;
				case 248u:
					CcEnableArenas = input.ReadBool();
					break;
				case 290u:
					ObjcClassPrefix = input.ReadString();
					break;
				case 298u:
					CsharpNamespace = input.ReadString();
					break;
				case 314u:
					SwiftPrefix = input.ReadString();
					break;
				case 7994u:
					uninterpretedOption_.AddEntriesFrom(input, _repeated_uninterpretedOption_codec);
					break;
				}
			}
		}
	}
}
