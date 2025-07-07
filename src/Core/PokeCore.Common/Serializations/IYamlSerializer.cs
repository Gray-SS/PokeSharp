namespace PokeCore.Common.Serializations;

public interface IYamlSerializer
{
    string Serialize<T>(T graph);
    T Deserialize<T>(string yaml);
}