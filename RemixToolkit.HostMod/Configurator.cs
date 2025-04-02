using RemixToolkit.Interfaces.Serializers;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using RemixToolkit.Core.Configs;

namespace RemixToolkit.HostMod;

internal class Configurator : DynamicConfigurator
{
    public Configurator()
        : base(new YamlSerializer())
    {
    }
}

public class YamlSerializer : IYamlSerializer
{
    public static readonly YamlSerializer Instance = new();

    private static readonly YamlDotNet.Serialization.ISerializer Serializer = new SerializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .DisableAliases()
        .Build();

    private static readonly IDeserializer Deserializer = new DeserializerBuilder()
        .IgnoreUnmatchedProperties()
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .Build();

    public T DeserializeFile<T>(string filePath) => Deserializer.Deserialize<T>(File.ReadAllText(filePath))
        ?? throw new Exception($"Failed to deserialize file.\nFile: {filePath}");

    public void SerializeFile<T>(string filePath, T obj)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllText(filePath, Serializer.Serialize(obj));
    }

    public T Deserialize<T>(string content) => Deserializer.Deserialize<T>(content)
        ?? throw new Exception($"Failed to deserialize content.\nContent: {content}");
}
