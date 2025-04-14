using NCalc;
using Reloaded.Mod.Interfaces;

namespace RemixToolkit.Reloaded.Configs;

internal record ConfigExpressionContext : ExpressionContext
{
    private readonly IModLoader _modLoader;

    public ConfigExpressionContext(IModLoader modLoader, Dictionary<string, object?> variables)
        : base(ExpressionOptions.AllowNullParameter)
    {
        _modLoader = modLoader;

        this.StaticParameters = variables;
        this.Functions = new Dictionary<string, ExpressionFunction>()
        {
            ["IsModEnabled"] = args => IsModEnabled(args),
        };
    }

    private bool IsModEnabled(ExpressionFunctionData args)
    {
        var modId = (string)args[0].Evaluate()!;
        return _modLoader.GetAppConfig().EnabledMods.Contains(modId);
    }
}
