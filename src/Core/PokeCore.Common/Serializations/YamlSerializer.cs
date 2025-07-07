using YamlDotNet.Serialization;

namespace PokeCore.Common.Serializations;

public sealed class YamlSerializer : IYamlSerializer
{
    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;

    public YamlSerializer()
    {
        _serializer = new SerializerBuilder()
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults)
            .Build();

        _deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .Build();
    }

    public string Serialize<T>(T graph)
    {
        return _serializer.Serialize(graph);
    }

    public T Deserialize<T>(string yaml)
    {
        return _deserializer.Deserialize<T>(yaml);
    }
}