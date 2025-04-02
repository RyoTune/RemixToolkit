namespace RemixToolkit.Core.Configs.Models;

public class DynamicConfigSchema
{
    public const string SchemaFileName = "schema.yaml";

    public Dictionary<string, string> Constants { get; set; } = [];

    public ConfigSetting[] Settings { get; set; } = [];

    public ConfigAction[] Actions { get; set; } = [];
}
