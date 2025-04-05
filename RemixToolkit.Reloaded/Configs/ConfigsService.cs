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
            foreach (var name in config.GetDynamicMemberNames())
            {
                variables[name] = config.GetSettingValue(name);
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
            if (!string.IsNullOrEmpty(action.If) && GetProcessedValue<bool>(action.If, variables) == false)
            {
                continue;
            }

            var actionUsing = GetProcessedValue<string>(action.Using, variables) ?? throw new Exception($"Failed to process 'using': {action.Using}");
            var controller = GetController(actionUsing) ?? throw new Exception($"Failed to find 'using': {actionUsing}");

            var typeInstance = controller.Target!;
            var actionRun = GetProcessedValue<string>(action.Run, variables) ?? throw new Exception($"Failed to process 'run': {action.Using}");

            var methodKey = $"{actionUsing}.{actionRun}";

            // Retrieve method from cached methods or with reflection.
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

    /// <summary>
    /// Get the final converted value for <paramref name="input"/>. This may be <c>null</c>, a variable value, or a formatted value. 
    /// </summary>
    /// <param name="input">Input text value.</param>
    /// <param name="variables">Available variables.</param>
    /// <param name="targetType">Target type of final value.</param>
    /// <returns>Final value of <paramref name="input"/> converted to <paramref name="targetType"/>.</returns>
    private static object? GetProcessedValue(string? input, Dictionary<string, object?> variables, Type targetType)
    {
        // Input is null value.
        if (input == null || input == "null") return null;

        // Format input text with variables.
        // Doing it early allows for using formatted strings to point to
        // variables. IDK how useful that is, but meh more options.
        input = Smart.Format(input, variables);

        // Input is the raw value of a variable.
        if (variables.TryGetValue(input, out var varValue))
        {
            return Convert.ChangeType(varValue, targetType);
        }

        // Format input text with variables.
        return Convert.ChangeType(input, targetType);
    }

    /// <summary>
    /// Get the final converted value for <paramref name="input"/>. This may be <c>null</c>, a variable value, or a formatted value. 
    /// </summary>
    /// <typeparam name="TValue">Target type of final value.</typeparam>
    /// <param name="input">Input text value.</param>
    /// <param name="variables">Available variables.</param>
    /// <returns>Final value of <paramref name="input"/> converted to <typeparamref name="TValue"/>.</returns>
    private static TValue? GetProcessedValue<TValue>(string? input, Dictionary<string, object?> variables)
        => (TValue?)GetProcessedValue(input, variables, typeof(TValue));

    private static object? ResolveParameter(string argValue, ParameterInfo targetParam, Dictionary<string, object?> variables)
        => GetProcessedValue(argValue, variables, targetParam.ParameterType);

    private static object?[]? ResolveParameters(string[] actionArgs, ParameterInfo[] methodParams, Dictionary<string, object?> variables)
    {
        if (actionArgs.Length == 0) return null;

        var resolved = new List<object?>();
        for (int i = 0; i < actionArgs.Length; i++)
        {
            resolved.Add(ResolveParameter(actionArgs[i], methodParams[i], variables));
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
