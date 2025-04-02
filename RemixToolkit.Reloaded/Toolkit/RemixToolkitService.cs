using Reloaded.Mod.Interfaces;
using Reloaded.Mod.Interfaces.Internal;
using RemixToolkit.Interfaces;
using System.Collections.Concurrent;
using System.Reflection;

namespace RemixToolkit.Reloaded.Toolkit;

internal class RemixToolkitService : IRemixToolkit
{
    private const string LOADER_CONTROLLERS_FIELD = "_controllerModMapping";

    private readonly IModLoader _modLoader;
    private readonly ConcurrentDictionary<Type, ModGenericTuple<object>> _controllerModMapping;

    public RemixToolkitService(IModLoader modLoader)
    {
        _modLoader = modLoader;
        _controllerModMapping = GetControllerMap(modLoader);
    }

    public WeakReference? GetController(string fullName)
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
