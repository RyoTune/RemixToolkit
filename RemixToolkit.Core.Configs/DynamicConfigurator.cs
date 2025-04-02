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
        var schemaFile = GetModSchemaFile(_modDir!);
        if (File.Exists(schemaFile))
        {
            var config = new DynamicConfig(yaml, GetModSchemaFile(_modDir!), Path.Join(_configDir, "config.yaml"));
            TypeDescriptor.AddProvider(new DynamicConfigTypeDescriptionProvider(config), typeof(DynamicConfig));
            return [config];
        }

        return [];
    }

    public void SetModDirectory(string modDirectory) => _modDir = modDirectory;

    public void SetConfigDirectory(string configDirectory) => _configDir = configDirectory;

    public bool TryRunCustomConfiguration() => false;

    public void Migrate(string oldDirectory, string newDirectory) { }

    private static string GetModSchemaFile(string modDir) => Path.Join(modDir, "ReMIX", "Config", DynamicConfigSchema.SchemaFileName);
}
