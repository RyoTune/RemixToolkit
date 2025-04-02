namespace RemixToolkit.Interfaces;

public interface IRemixToolkit
{
    /// <summary>
    /// Gets a controller from the mod loader's stored list of controllers by name.
    /// </summary>
    /// <param name="fullName">Full type name of controller.</param>
    /// <returns>The specified controller if found, <c>null</c> otherwise.</returns>
    WeakReference? GetController(string fullName);
}
