using PokeSharp.Assets;
using PokeSharp.Assets.Exceptions;
using YamlDotNet.Core;

namespace PokeSharp.Scenes;

public sealed class SceneImporter : AssetImporter<SceneData>
{
    public override Type ProcessorType => typeof(SceneProcessor);

    public override bool CanImport(string ext)
    {
        return ext == ".pkscene";
    }

    public override SceneData Import(string path)
    {
        try
        {
            var yaml = File.ReadAllText(path);
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().Build();

            var sceneData = deserializer.Deserialize<SceneData>(yaml);
            return sceneData;
        }
        catch (YamlException ex)
        {
            throw new AssetImporterException($"The scene is not in a valid format: {ex.Message}");
        }
    }
}
