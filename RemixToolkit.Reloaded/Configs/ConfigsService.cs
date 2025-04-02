using Reloaded.Mod.Interfaces;
using Reloaded.Mod.Interfaces.Internal;
using RemixToolkit.Core.Configs;
using RemixToolkit.Core.Serializers;
using SmartFormat;
using System.Collections.Concurrent;
using System.Reflection;

namespace RemixToolkit.Reloaded.Configs;

internal class ConfigsService
{
    private const string LOADER_CONTROLLERS_FIELD = "_controllerModMapping";
    private readonly ConcurrentDictionary<Type, ModGenericTuple<object>> _controllerModMapping;

    private readonly IModLoader _modLoader;
    private readonly Dictionary<string, MethodInfo> cachedMethods = [];

    public ConfigsService(IModLoader modLoader)
    {
        _modLoader = modLoader;
        _controllerModMapping = GetControllerMap(modLoader);
    }

    public void OnModLoading(IModV1 mod, IModConfigV1 modConfig)
    {
        if (DynamicConfig.TryCreateForMod(YamlSerializer.Instance, _modLoader, modConfig, out var config))
        {
            /* Build config variables dictionary. */
            var variables = new Dictionary<string, object?>()
            {
                ["ModId"] = modConfig.ModId,
                ["ModFolder"] = _modLoader.GetDirectoryForModId(modConfig.ModId),
                ["ModDir"] = _modLoader.GetDirectoryForModId(modConfig.ModId),
            };

            // Add defined constants.
            foreach (var item in config.Constants)
            {
                variables[item.Key] = item.Value;
            }

            // Add setting values.
            foreach (var item in config.PropertyDescriptors)
            {
                variables[item.Name] = item.GetValue(variables);
            }

            // Add lists.
            foreach (var list in config.Lists)
            {
                variables[list.Key] = list.Value;
            }

            ApplyDynamicConfig(config, variables);
        }
    }

    private void ApplyDynamicConfig(DynamicConfig config, Dictionary<string, object?> variables)
    {
        // Execute actions.
        foreach (var action in config.Actions)
        {
            if (!string.IsNullOrEmpty(action.If) && (bool)config.GetSettingValue(action.If)! == false)
            {
                continue;
            }

            var actionUsing = Smart.Format(action.Using, variables);
            var controller = GetController(actionUsing) ?? throw new Exception($"Failed to find 'using': {actionUsing}");

            var typeInstance = controller.Target!;
            var actionRun = Smart.Format(action.Run, variables);

            var methodKey = $"{actionUsing}.{actionRun}";

            // Retrieve method from cached methods or with relfection.
            if (!cachedMethods.TryGetValue(methodKey, out var method))
            {
                var type = typeInstance.GetType();
                method = type.GetMethod(actionRun) ?? throw new Exception($"Failed to find 'run': {actionRun}");
                cachedMethods[methodKey] = method;
            }

            var methodRet = method.Invoke(typeInstance, ResolveParameters(action.With, method.GetParameters(), variables));
            if (!string.IsNullOrEmpty(action.Output))
            {
                variables[action.Output] = methodRet;
            }
        }
    }

    private WeakReference? GetController(string fullName)
    {
        foreach (var key in _controllerModMapping.Keys)
        {
            if (key.FullName == fullName)
            {
                return new WeakReference(_controllerModMapping[key].Generic);
            }
        }

        Log.Error($"{nameof(GetController)} for \"{fullName}\" returned null.");
        return null;
    }

    private static object?[]? ResolveParameters(string[] actionArgs, ParameterInfo[] methodParams, Dictionary<string, object?> configVars)
    {
        if (actionArgs.Length == 0) return null;

        var resolved = new List<object?>();
        for (int i = 0; i < actionArgs.Length; i++)
        {
            var argText = actionArgs[i];

            // Null arg value.
            if (argText == null || argText == "null") return null;

            var targetParam = methodParams[i];

            // Arg is the raw value of a setting/constant.
            if (configVars.TryGetValue(argText, out var configArg))
            {
                // Convert variable to param type.
                resolved.Add(Convert.ChangeType(configArg, targetParam.ParameterType));
                continue;
            }

            // Parameter is string, format and use arg text.
            if (targetParam.ParameterType == typeof(string))
            {
                resolved.Add(Smart.Format(argText, configVars));
                continue;
            }

            // Convert arg text to parameter type.
            resolved.Add(Convert.ChangeType(argText, targetParam.ParameterType));
        }

        return resolved.ToArray();
    }

    private static ConcurrentDictionary<Type, ModGenericTuple<object>> GetControllerMap(IModLoader modLoader)
    {
        var loaderType = modLoader.GetType();
        var controllerMapField = loaderType.GetField(LOADER_CONTROLLERS_FIELD, BindingFlags.NonPublic | BindingFlags.Instance);
        if (controllerMapField == null)
        {
            Log.Error($"Failed to get controllers field from loader: {LOADER_CONTROLLERS_FIELD}");
            return [];
        }

        var controllerMap = (ConcurrentDictionary<Type, ModGenericTuple<object>>?)controllerMapField.GetValue(modLoader);
        if (controllerMap == null)
        {
            Log.Error($"Failed to get controllers value from field.");
            return [];
        }

        return controllerMap;
    }
}
