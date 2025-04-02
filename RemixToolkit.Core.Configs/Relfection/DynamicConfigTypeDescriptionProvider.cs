using RemixToolkit.Core.Configs;
using System.ComponentModel;

namespace RemixToolkit.Core.Configs.Relfection;

public class DynamicConfigTypeDescriptionProvider(DynamicConfig config) : TypeDescriptionProvider(m_Default)
{
    private static readonly TypeDescriptionProvider m_Default = TypeDescriptor.GetProvider(typeof(DynamicConfig));

    public override ICustomTypeDescriptor? GetTypeDescriptor(Type objectType, object? instance)
    {
        if (objectType == typeof(DynamicConfig)) return new DynamicConfigTypeDescriptor(config);
        return base.GetTypeDescriptor(objectType, instance);
    }
}
