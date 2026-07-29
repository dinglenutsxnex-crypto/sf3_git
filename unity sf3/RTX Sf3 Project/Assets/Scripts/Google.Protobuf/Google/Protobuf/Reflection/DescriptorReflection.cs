using System;

namespace Google.Protobuf.Reflection
{
	internal static class DescriptorReflection
	{
		private static FileDescriptor descriptor;

		public static FileDescriptor Descriptor
		{
			get
			{
				return descriptor;
			}
		}

		static DescriptorReflection()
		{
			descriptor = FileDescriptor.FromGeneratedCode(Convert.FromBase64String("CiBnb29nbGUvcHJvdG9idWYvZGVzY3JpcHRvci5wcm90bxIPZ29vZ2xlLnBy" + "b3RvYnVmIkcKEUZpbGVEZXNjcmlwdG9yU2V0EjIKBGZpbGUYASADKAsyJC5n" + "b29nbGUucHJvdG9idWYuRmlsZURlc2NyaXB0b3JQcm90byLbAwoTRmlsZURl" + "c2NyaXB0b3JQcm90bxIMCgRuYW1lGAEgASgJEg8KB3BhY2thZ2UYAiABKAkS" + "EgoKZGVwZW5kZW5jeRgDIAMoCRIZChFwdWJsaWNfZGVwZW5kZW5jeRgKIAMo" + "BRIXCg93ZWFrX2RlcGVuZGVuY3kYCyADKAUSNgoMbWVzc2FnZV90eXBlGAQg" + "AygLMiAuZ29vZ2xlLnByb3RvYnVmLkRlc2NyaXB0b3JQcm90bxI3CgllbnVt" + "X3R5cGUYBSADKAsyJC5nb29nbGUucHJvdG9idWYuRW51bURlc2NyaXB0b3JQ" + "cm90bxI4CgdzZXJ2aWNlGAYgAygLMicuZ29vZ2xlLnByb3RvYnVmLlNlcnZp" + "Y2VEZXNjcmlwdG9yUHJvdG8SOAoJZXh0ZW5zaW9uGAcgAygLMiUuZ29vZ2xl" + "LnByb3RvYnVmLkZpZWxkRGVzY3JpcHRvclByb3RvEi0KB29wdGlvbnMYCCAB" + "KAsyHC5nb29nbGUucHJvdG9idWYuRmlsZU9wdGlvbnMSOQoQc291cmNlX2Nv" + "ZGVfaW5mbxgJIAEoCzIfLmdvb2dsZS5wcm90b2J1Zi5Tb3VyY2VDb2RlSW5m" + "bxIOCgZzeW50YXgYDCABKAki8AQKD0Rlc2NyaXB0b3JQcm90bxIMCgRuYW1l" + "GAEgASgJEjQKBWZpZWxkGAIgAygLMiUuZ29vZ2xlLnByb3RvYnVmLkZpZWxk" + "RGVzY3JpcHRvclByb3RvEjgKCWV4dGVuc2lvbhgGIAMoCzIlLmdvb2dsZS5w" + "cm90b2J1Zi5GaWVsZERlc2NyaXB0b3JQcm90bxI1CgtuZXN0ZWRfdHlwZRgD" + "IAMoCzIgLmdvb2dsZS5wcm90b2J1Zi5EZXNjcmlwdG9yUHJvdG8SNwoJZW51" + "bV90eXBlGAQgAygLMiQuZ29vZ2xlLnByb3RvYnVmLkVudW1EZXNjcmlwdG9y" + "UHJvdG8SSAoPZXh0ZW5zaW9uX3JhbmdlGAUgAygLMi8uZ29vZ2xlLnByb3Rv" + "YnVmLkRlc2NyaXB0b3JQcm90by5FeHRlbnNpb25SYW5nZRI5CgpvbmVvZl9k" + "ZWNsGAggAygLMiUuZ29vZ2xlLnByb3RvYnVmLk9uZW9mRGVzY3JpcHRvclBy" + "b3RvEjAKB29wdGlvbnMYByABKAsyHy5nb29nbGUucHJvdG9idWYuTWVzc2Fn" + "ZU9wdGlvbnMSRgoOcmVzZXJ2ZWRfcmFuZ2UYCSADKAsyLi5nb29nbGUucHJv" + "dG9idWYuRGVzY3JpcHRvclByb3RvLlJlc2VydmVkUmFuZ2USFQoNcmVzZXJ2" + "ZWRfbmFtZRgKIAMoCRosCg5FeHRlbnNpb25SYW5nZRINCgVzdGFydBgBIAEo" + "BRILCgNlbmQYAiABKAUaKwoNUmVzZXJ2ZWRSYW5nZRINCgVzdGFydBgBIAEo" + "BRILCgNlbmQYAiABKAUivAUKFEZpZWxkRGVzY3JpcHRvclByb3RvEgwKBG5h" + "bWUYASABKAkSDgoGbnVtYmVyGAMgASgFEjoKBWxhYmVsGAQgASgOMisuZ29v" + "Z2xlLnByb3RvYnVmLkZpZWxkRGVzY3JpcHRvclByb3RvLkxhYmVsEjgKBHR5" + "cGUYBSABKA4yKi5nb29nbGUucHJvdG9idWYuRmllbGREZXNjcmlwdG9yUHJv" + "dG8uVHlwZRIRCgl0eXBlX25hbWUYBiABKAkSEAoIZXh0ZW5kZWUYAiABKAkS" + "FQoNZGVmYXVsdF92YWx1ZRgHIAEoCRITCgtvbmVvZl9pbmRleBgJIAEoBRIR" + "Cglqc29uX25hbWUYCiABKAkSLgoHb3B0aW9ucxgIIAEoCzIdLmdvb2dsZS5w" + "cm90b2J1Zi5GaWVsZE9wdGlvbnMitgIKBFR5cGUSDwoLVFlQRV9ET1VCTEUQ" + "ARIOCgpUWVBFX0ZMT0FUEAISDgoKVFlQRV9JTlQ2NBADEg8KC1RZUEVfVUlO" + "VDY0EAQSDgoKVFlQRV9JTlQzMhAFEhAKDFRZUEVfRklYRUQ2NBAGEhAKDFRZ" + "UEVfRklYRUQzMhAHEg0KCVRZUEVfQk9PTBAIEg8KC1RZUEVfU1RSSU5HEAkS" + "DgoKVFlQRV9HUk9VUBAKEhAKDFRZUEVfTUVTU0FHRRALEg4KClRZUEVfQllU" + "RVMQDBIPCgtUWVBFX1VJTlQzMhANEg0KCVRZUEVfRU5VTRAOEhEKDVRZUEVf" + "U0ZJWEVEMzIQDxIRCg1UWVBFX1NGSVhFRDY0EBASDwoLVFlQRV9TSU5UMzIQ" + "ERIPCgtUWVBFX1NJTlQ2NBASIkMKBUxhYmVsEhIKDkxBQkVMX09QVElPTkFM" + "EAESEgoOTEFCRUxfUkVRVUlSRUQQAhISCg5MQUJFTF9SRVBFQVRFRBADIlQK" + "FE9uZW9mRGVzY3JpcHRvclByb3RvEgwKBG5hbWUYASABKAkSLgoHb3B0aW9u" + "cxgCIAEoCzIdLmdvb2dsZS5wcm90b2J1Zi5PbmVvZk9wdGlvbnMijAEKE0Vu" + "dW1EZXNjcmlwdG9yUHJvdG8SDAoEbmFtZRgBIAEoCRI4CgV2YWx1ZRgCIAMo" + "CzIpLmdvb2dsZS5wcm90b2J1Zi5FbnVtVmFsdWVEZXNjcmlwdG9yUHJvdG8S" + "LQoHb3B0aW9ucxgDIAEoCzIcLmdvb2dsZS5wcm90b2J1Zi5FbnVtT3B0aW9u" + "cyJsChhFbnVtVmFsdWVEZXNjcmlwdG9yUHJvdG8SDAoEbmFtZRgBIAEoCRIO" + "CgZudW1iZXIYAiABKAUSMgoHb3B0aW9ucxgDIAEoCzIhLmdvb2dsZS5wcm90" + "b2J1Zi5FbnVtVmFsdWVPcHRpb25zIpABChZTZXJ2aWNlRGVzY3JpcHRvclBy" + "b3RvEgwKBG5hbWUYASABKAkSNgoGbWV0aG9kGAIgAygLMiYuZ29vZ2xlLnBy" + "b3RvYnVmLk1ldGhvZERlc2NyaXB0b3JQcm90bxIwCgdvcHRpb25zGAMgASgL" + "Mh8uZ29vZ2xlLnByb3RvYnVmLlNlcnZpY2VPcHRpb25zIsEBChVNZXRob2RE" + "ZXNjcmlwdG9yUHJvdG8SDAoEbmFtZRgBIAEoCRISCgppbnB1dF90eXBlGAIg" + "ASgJEhMKC291dHB1dF90eXBlGAMgASgJEi8KB29wdGlvbnMYBCABKAsyHi5n" + "b29nbGUucHJvdG9idWYuTWV0aG9kT3B0aW9ucxIfChBjbGllbnRfc3RyZWFt" + "aW5nGAUgASgIOgVmYWxzZRIfChBzZXJ2ZXJfc3RyZWFtaW5nGAYgASgIOgVm" + "YWxzZSKaBQoLRmlsZU9wdGlvbnMSFAoMamF2YV9wYWNrYWdlGAEgASgJEhwK" + "FGphdmFfb3V0ZXJfY2xhc3NuYW1lGAggASgJEiIKE2phdmFfbXVsdGlwbGVf" + "ZmlsZXMYCiABKAg6BWZhbHNlEikKHWphdmFfZ2VuZXJhdGVfZXF1YWxzX2Fu" + "ZF9oYXNoGBQgASgIQgIYARIlChZqYXZhX3N0cmluZ19jaGVja191dGY4GBsg" + "ASgIOgVmYWxzZRJGCgxvcHRpbWl6ZV9mb3IYCSABKA4yKS5nb29nbGUucHJv" + "dG9idWYuRmlsZU9wdGlvbnMuT3B0aW1pemVNb2RlOgVTUEVFRBISCgpnb19w" + "YWNrYWdlGAsgASgJEiIKE2NjX2dlbmVyaWNfc2VydmljZXMYECABKAg6BWZh" + "bHNlEiQKFWphdmFfZ2VuZXJpY19zZXJ2aWNlcxgRIAEoCDoFZmFsc2USIgoT" + "cHlfZ2VuZXJpY19zZXJ2aWNlcxgSIAEoCDoFZmFsc2USGQoKZGVwcmVjYXRl" + "ZBgXIAEoCDoFZmFsc2USHwoQY2NfZW5hYmxlX2FyZW5hcxgfIAEoCDoFZmFs" + "c2USGQoRb2JqY19jbGFzc19wcmVmaXgYJCABKAkSGAoQY3NoYXJwX25hbWVz" + "cGFjZRglIAEoCRIUCgxzd2lmdF9wcmVmaXgYJyABKAkSQwoUdW5pbnRlcnBy" + "ZXRlZF9vcHRpb24Y5wcgAygLMiQuZ29vZ2xlLnByb3RvYnVmLlVuaW50ZXJw" + "cmV0ZWRPcHRpb24iOgoMT3B0aW1pemVNb2RlEgkKBVNQRUVEEAESDQoJQ09E" + "RV9TSVpFEAISEAoMTElURV9SVU5USU1FEAMqCQjoBxCAgICAAkoECCYQJyLs" + "AQoOTWVzc2FnZU9wdGlvbnMSJgoXbWVzc2FnZV9zZXRfd2lyZV9mb3JtYXQY" + "ASABKAg6BWZhbHNlEi4KH25vX3N0YW5kYXJkX2Rlc2NyaXB0b3JfYWNjZXNz" + "b3IYAiABKAg6BWZhbHNlEhkKCmRlcHJlY2F0ZWQYAyABKAg6BWZhbHNlEhEK" + "CW1hcF9lbnRyeRgHIAEoCBJDChR1bmludGVycHJldGVkX29wdGlvbhjnByAD" + "KAsyJC5nb29nbGUucHJvdG9idWYuVW5pbnRlcnByZXRlZE9wdGlvbioJCOgH" + "EICAgIACSgQICBAJIp4DCgxGaWVsZE9wdGlvbnMSOgoFY3R5cGUYASABKA4y" + "Iy5nb29nbGUucHJvdG9idWYuRmllbGRPcHRpb25zLkNUeXBlOgZTVFJJTkcS" + "DgoGcGFja2VkGAIgASgIEj8KBmpzdHlwZRgGIAEoDjIkLmdvb2dsZS5wcm90" + "b2J1Zi5GaWVsZE9wdGlvbnMuSlNUeXBlOglKU19OT1JNQUwSEwoEbGF6eRgF" + "IAEoCDoFZmFsc2USGQoKZGVwcmVjYXRlZBgDIAEoCDoFZmFsc2USEwoEd2Vh" + "axgKIAEoCDoFZmFsc2USQwoUdW5pbnRlcnByZXRlZF9vcHRpb24Y5wcgAygL" + "MiQuZ29vZ2xlLnByb3RvYnVmLlVuaW50ZXJwcmV0ZWRPcHRpb24iLwoFQ1R5" + "cGUSCgoGU1RSSU5HEAASCAoEQ09SRBABEhAKDFNUUklOR19QSUVDRRACIjUK" + "BkpTVHlwZRINCglKU19OT1JNQUwQABINCglKU19TVFJJTkcQARINCglKU19O" + "VU1CRVIQAioJCOgHEICAgIACSgQIBBAFIl4KDE9uZW9mT3B0aW9ucxJDChR1" + "bmludGVycHJldGVkX29wdGlvbhjnByADKAsyJC5nb29nbGUucHJvdG9idWYu" + "VW5pbnRlcnByZXRlZE9wdGlvbioJCOgHEICAgIACIo0BCgtFbnVtT3B0aW9u" + "cxITCgthbGxvd19hbGlhcxgCIAEoCBIZCgpkZXByZWNhdGVkGAMgASgIOgVm" + "YWxzZRJDChR1bmludGVycHJldGVkX29wdGlvbhjnByADKAsyJC5nb29nbGUu" + "cHJvdG9idWYuVW5pbnRlcnByZXRlZE9wdGlvbioJCOgHEICAgIACIn0KEEVu" + "dW1WYWx1ZU9wdGlvbnMSGQoKZGVwcmVjYXRlZBgBIAEoCDoFZmFsc2USQwoU" + "dW5pbnRlcnByZXRlZF9vcHRpb24Y5wcgAygLMiQuZ29vZ2xlLnByb3RvYnVm" + "LlVuaW50ZXJwcmV0ZWRPcHRpb24qCQjoBxCAgICAAiJ7Cg5TZXJ2aWNlT3B0" + "aW9ucxIZCgpkZXByZWNhdGVkGCEgASgIOgVmYWxzZRJDChR1bmludGVycHJl" + "dGVkX29wdGlvbhjnByADKAsyJC5nb29nbGUucHJvdG9idWYuVW5pbnRlcnBy" + "ZXRlZE9wdGlvbioJCOgHEICAgIACIq0CCg1NZXRob2RPcHRpb25zEhkKCmRl" + "cHJlY2F0ZWQYISABKAg6BWZhbHNlEl8KEWlkZW1wb3RlbmN5X2xldmVsGCIg" + "ASgOMi8uZ29vZ2xlLnByb3RvYnVmLk1ldGhvZE9wdGlvbnMuSWRlbXBvdGVu" + "Y3lMZXZlbDoTSURFTVBPVEVOQ1lfVU5LTk9XThJDChR1bmludGVycHJldGVk" + "X29wdGlvbhjnByADKAsyJC5nb29nbGUucHJvdG9idWYuVW5pbnRlcnByZXRl" + "ZE9wdGlvbiJQChBJZGVtcG90ZW5jeUxldmVsEhcKE0lERU1QT1RFTkNZX1VO" + "S05PV04QABITCg9OT19TSURFX0VGRkVDVFMQARIOCgpJREVNUE9URU5UEAIq" + "CQjoBxCAgICAAiKeAgoTVW5pbnRlcnByZXRlZE9wdGlvbhI7CgRuYW1lGAIg" + "AygLMi0uZ29vZ2xlLnByb3RvYnVmLlVuaW50ZXJwcmV0ZWRPcHRpb24uTmFt" + "ZVBhcnQSGAoQaWRlbnRpZmllcl92YWx1ZRgDIAEoCRIaChJwb3NpdGl2ZV9p" + "bnRfdmFsdWUYBCABKAQSGgoSbmVnYXRpdmVfaW50X3ZhbHVlGAUgASgDEhQK" + "DGRvdWJsZV92YWx1ZRgGIAEoARIUCgxzdHJpbmdfdmFsdWUYByABKAwSFwoP" + "YWdncmVnYXRlX3ZhbHVlGAggASgJGjMKCE5hbWVQYXJ0EhEKCW5hbWVfcGFy" + "dBgBIAIoCRIUCgxpc19leHRlbnNpb24YAiACKAgi1QEKDlNvdXJjZUNvZGVJ" + "bmZvEjoKCGxvY2F0aW9uGAEgAygLMiguZ29vZ2xlLnByb3RvYnVmLlNvdXJj" + "ZUNvZGVJbmZvLkxvY2F0aW9uGoYBCghMb2NhdGlvbhIQCgRwYXRoGAEgAygF" + "QgIQARIQCgRzcGFuGAIgAygFQgIQARIYChBsZWFkaW5nX2NvbW1lbnRzGAMg" + "ASgJEhkKEXRyYWlsaW5nX2NvbW1lbnRzGAQgASgJEiEKGWxlYWRpbmdfZGV0" + "YWNoZWRfY29tbWVudHMYBiADKAkipwEKEUdlbmVyYXRlZENvZGVJbmZvEkEK" + "CmFubm90YXRpb24YASADKAsyLS5nb29nbGUucHJvdG9idWYuR2VuZXJhdGVk" + "Q29kZUluZm8uQW5ub3RhdGlvbhpPCgpBbm5vdGF0aW9uEhAKBHBhdGgYASAD" + "KAVCAhABEhMKC3NvdXJjZV9maWxlGAIgASgJEg0KBWJlZ2luGAMgASgFEgsK" + "A2VuZBgEIAEoBUKMAQoTY29tLmdvb2dsZS5wcm90b2J1ZkIQRGVzY3JpcHRv" + "clByb3Rvc0gBWj5naXRodWIuY29tL2dvbGFuZy9wcm90b2J1Zi9wcm90b2Mt" + "Z2VuLWdvL2Rlc2NyaXB0b3I7ZGVzY3JpcHRvcqICA0dQQqoCGkdvb2dsZS5Q" + "cm90b2J1Zi5SZWZsZWN0aW9u"), new FileDescriptor[0], new GeneratedClrTypeInfo(null, new GeneratedClrTypeInfo[20]
			{
				new GeneratedClrTypeInfo(typeof(FileDescriptorSet), FileDescriptorSet.Parser, new string[1] { "File" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(FileDescriptorProto), FileDescriptorProto.Parser, new string[12]
				{
					"Name", "Package", "Dependency", "PublicDependency", "WeakDependency", "MessageType", "EnumType", "Service", "Extension", "Options",
					"SourceCodeInfo", "Syntax"
				}, null, null, null),
				new GeneratedClrTypeInfo(typeof(DescriptorProto), DescriptorProto.Parser, new string[10] { "Name", "Field", "Extension", "NestedType", "EnumType", "ExtensionRange", "OneofDecl", "Options", "ReservedRange", "ReservedName" }, null, null, new GeneratedClrTypeInfo[2]
				{
					new GeneratedClrTypeInfo(typeof(DescriptorProto.Types.ExtensionRange), DescriptorProto.Types.ExtensionRange.Parser, new string[2] { "Start", "End" }, null, null, null),
					new GeneratedClrTypeInfo(typeof(DescriptorProto.Types.ReservedRange), DescriptorProto.Types.ReservedRange.Parser, new string[2] { "Start", "End" }, null, null, null)
				}),
				new GeneratedClrTypeInfo(typeof(FieldDescriptorProto), FieldDescriptorProto.Parser, new string[10] { "Name", "Number", "Label", "Type", "TypeName", "Extendee", "DefaultValue", "OneofIndex", "JsonName", "Options" }, null, new Type[2]
				{
					typeof(FieldDescriptorProto.Types.Type),
					typeof(FieldDescriptorProto.Types.Label)
				}, null),
				new GeneratedClrTypeInfo(typeof(OneofDescriptorProto), OneofDescriptorProto.Parser, new string[2] { "Name", "Options" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(EnumDescriptorProto), EnumDescriptorProto.Parser, new string[3] { "Name", "Value", "Options" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(EnumValueDescriptorProto), EnumValueDescriptorProto.Parser, new string[3] { "Name", "Number", "Options" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(ServiceDescriptorProto), ServiceDescriptorProto.Parser, new string[3] { "Name", "Method", "Options" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(MethodDescriptorProto), MethodDescriptorProto.Parser, new string[6] { "Name", "InputType", "OutputType", "Options", "ClientStreaming", "ServerStreaming" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(FileOptions), FileOptions.Parser, new string[16]
				{
					"JavaPackage", "JavaOuterClassname", "JavaMultipleFiles", "JavaGenerateEqualsAndHash", "JavaStringCheckUtf8", "OptimizeFor", "GoPackage", "CcGenericServices", "JavaGenericServices", "PyGenericServices",
					"Deprecated", "CcEnableArenas", "ObjcClassPrefix", "CsharpNamespace", "SwiftPrefix", "UninterpretedOption"
				}, null, new Type[1] { typeof(FileOptions.Types.OptimizeMode) }, null),
				new GeneratedClrTypeInfo(typeof(MessageOptions), MessageOptions.Parser, new string[5] { "MessageSetWireFormat", "NoStandardDescriptorAccessor", "Deprecated", "MapEntry", "UninterpretedOption" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(FieldOptions), FieldOptions.Parser, new string[7] { "Ctype", "Packed", "Jstype", "Lazy", "Deprecated", "Weak", "UninterpretedOption" }, null, new Type[2]
				{
					typeof(FieldOptions.Types.CType),
					typeof(FieldOptions.Types.JSType)
				}, null),
				new GeneratedClrTypeInfo(typeof(OneofOptions), OneofOptions.Parser, new string[1] { "UninterpretedOption" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(EnumOptions), EnumOptions.Parser, new string[3] { "AllowAlias", "Deprecated", "UninterpretedOption" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(EnumValueOptions), EnumValueOptions.Parser, new string[2] { "Deprecated", "UninterpretedOption" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(ServiceOptions), ServiceOptions.Parser, new string[2] { "Deprecated", "UninterpretedOption" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(MethodOptions), MethodOptions.Parser, new string[3] { "Deprecated", "IdempotencyLevel", "UninterpretedOption" }, null, new Type[1] { typeof(MethodOptions.Types.IdempotencyLevel) }, null),
				new GeneratedClrTypeInfo(typeof(UninterpretedOption), UninterpretedOption.Parser, new string[7] { "Name", "IdentifierValue", "PositiveIntValue", "NegativeIntValue", "DoubleValue", "StringValue", "AggregateValue" }, null, null, new GeneratedClrTypeInfo[1]
				{
					new GeneratedClrTypeInfo(typeof(UninterpretedOption.Types.NamePart), UninterpretedOption.Types.NamePart.Parser, new string[2] { "NamePart_", "IsExtension" }, null, null, null)
				}),
				new GeneratedClrTypeInfo(typeof(SourceCodeInfo), SourceCodeInfo.Parser, new string[1] { "Location" }, null, null, new GeneratedClrTypeInfo[1]
				{
					new GeneratedClrTypeInfo(typeof(SourceCodeInfo.Types.Location), SourceCodeInfo.Types.Location.Parser, new string[5] { "Path", "Span", "LeadingComments", "TrailingComments", "LeadingDetachedComments" }, null, null, null)
				}),
				new GeneratedClrTypeInfo(typeof(GeneratedCodeInfo), GeneratedCodeInfo.Parser, new string[1] { "Annotation" }, null, null, new GeneratedClrTypeInfo[1]
				{
					new GeneratedClrTypeInfo(typeof(GeneratedCodeInfo.Types.Annotation), GeneratedCodeInfo.Types.Annotation.Parser, new string[4] { "Path", "SourceFile", "Begin", "End" }, null, null, null)
				})
			}));
		}
	}
}
