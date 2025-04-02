using Reloaded.Mod.Interfaces;
using RemixToolkit.Interfaces.Serializers;
using System.ComponentModel;
using RemixToolkit.Core.Configs.Models;
using RemixToolkit.Core.Configs.Relfection;

namespace RemixToolkit.Core.Configs;

public class DynamicConfigurator(IYamlSerializer yaml) : IConfiguratorV2
{
    private string? _modDir;
    private string? _configDir;

    public IConfigurable[] GetConfigurations()
    {
        if (DynamicConfig.TryCreateForMod(yaml, _modDir!, _configDir!, out var config))
        {
            TypeDescriptor.AddProvider(new DynamicConfigTypeDescriptionProvider(config), typeof(DynamicConfig));
            return [config];
        }

        return [];
    }

    public void SetModDirectory(string modDirectory) => _modDir = modDirectory;

    public void SetConfigDirectory(string configDirectory) => _configDir = configDirectory;

    public bool TryRunCustomConfiguration() => false;

    public void Migrate(string oldDirectory, string newDirectory) { }
}
