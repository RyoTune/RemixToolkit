namespace RemixToolkit.Core.Configs.Models;

public class DynamicConfigSchema
{
    public Dictionary<string, string> Constants { get; set; } = [];

    public ConfigSetting[] Settings { get; set; } = [];

    public ConfigAction[] Actions { get; set; } = [];
}
