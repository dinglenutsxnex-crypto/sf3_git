using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public sealed class CustomSerializationObjectGraphVisitor : ChainedObjectGraphVisitor
	{
		private readonly IEmitter emitter;

		private readonly IEnumerable<IYamlTypeConverter> typeConverters;

		public CustomSerializationObjectGraphVisitor(IEmitter emitter, IObjectGraphVisitor nextVisitor, IEnumerable<IYamlTypeConverter> typeConverters)
			: base(nextVisitor)
		{
			this.emitter = emitter;
			this.typeConverters = ((typeConverters == null) ? Enumerable.Empty<IYamlTypeConverter>() : typeConverters.ToList());
		}

		public override bool Enter(IObjectDescriptor value)
		{
			IYamlTypeConverter yamlTypeConverter = typeConverters.FirstOrDefault((IYamlTypeConverter t) => t.Accepts(value.Type));
			if (yamlTypeConverter != null)
			{
				yamlTypeConverter.WriteYaml(emitter, value.Value, value.Type);
				return false;
			}
			IYamlSerializable yamlSerializable = value as IYamlSerializable;
			if (yamlSerializable != null)
			{
				yamlSerializable.WriteYaml(emitter);
				return false;
			}
			return base.Enter(value);
		}
	}
}
