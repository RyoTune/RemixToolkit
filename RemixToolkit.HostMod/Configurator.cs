using RemixToolkit.Core.Configs;
using RemixToolkit.Core.Serializers;

namespace RemixToolkit.HostMod;

internal class Configurator : DynamicConfigurator
{
    public Configurator()
        : base(YamlSerializer.Instance)
    {
    }
}
