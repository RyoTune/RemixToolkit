using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using RemixToolkit.Reloaded.Configuration;
using RemixToolkit.Reloaded.Template;
using RemixToolkit.Interfaces;
using RemixToolkit.Reloaded.FileSystem;
using RemixToolkit.Reloaded.Configs;
using RemixToolkit.Interfaces.Serializers;
using RemixToolkit.Reloaded.Serializers;
using Reloaded.Mod.Interfaces.Internal;

#if DEBUG
using System.Diagnostics;
#endif

namespace RemixToolkit.Reloaded;

public class Mod : ModBase, IExports
{
    private readonly IModLoader _modLoader;
    private readonly IReloadedHooks? _hooks;
    private readonly ILogger _log;
    private readonly IMod _owner;

    private Config _config;
    private readonly IModConfig _modConfig;

    private readonly ConfigsService _configService;

    public Mod(ModContext context)
    {
        _modLoader = context.ModLoader;
        _hooks = context.Hooks;
        _log = context.Logger;
        _owner = context.Owner;
        _config = context.Configuration;
        _modConfig = context.ModConfig;
#if DEBUG
        Debugger.Launch();
#endif
        Project.Initialize(_modConfig, _modLoader, _log, true);
        Log.LogLevel = _config.LogLevel;

        _configService = new ConfigsService(_modLoader);

        _modLoader.AddOrReplaceController<IYamlSerializer>(_owner, YamlSerializer.Instance);
        _modLoader.AddOrReplaceController<IFileSystem>(_owner, FileSystemService.Instance);
        _modLoader.ModLoading += OnModLoading;
     }

    private void OnModLoading(IModV1 mod, IModConfigV1 modConfig)
    {
        if (!modConfig.ModDependencies.Contains(_modConfig.ModId))
        {
            return;
        }

        _configService.OnModLoading(mod, modConfig);
    }

    #region Standard Overrides
    public override void ConfigurationUpdated(Config configuration)
    {
        // Apply settings from configuration.
        // ... your code here.
        _config = configuration;
        _log.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
        Log.LogLevel = _config.LogLevel;
    }

    public Type[] GetTypes() => [typeof(IYamlSerializer), typeof(IFileSystem)];
    #endregion

    #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Mod() { }
#pragma warning restore CS8618
    #endregion
}