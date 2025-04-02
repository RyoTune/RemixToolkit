namespace RemixToolkit.Interfaces.Serializers;

public interface ISerializer
{
    T Deserialize<T>(string content);

    T DeserializeFile<T>(string filePath);

    void SerializeFile<T>(string filePath, T obj);
}
