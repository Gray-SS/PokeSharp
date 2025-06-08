using ImGuiNET;
using PokeSharp.Assets;
using PokeSharp.Assets.Services;

namespace PokeSharp.Editor.Views;

public sealed class AssetManagerView : IEditorView
{
    private readonly IAssetMetadataStore _assetMetadataStore;

    public AssetManagerView(IAssetMetadataStore metadataStore)
    {
        _assetMetadataStore = metadataStore;
    }

    public void DrawGui()
    {
        if (ImGui.Begin("Asset Manager"))
        {
            ImGui.Dummy(new(0, 10));

            foreach (AssetMetadata metadata in _assetMetadataStore.GetCachedMetadatas())
            {
                ImGui.Text(metadata.Id.ToString());

                if (metadata.AssetType != null)
                    ImGui.Text(metadata.AssetType.Name);

                if (metadata.ResourcePath != null)
                    ImGui.Text(metadata.ResourcePath.Uri);

                ImGui.Dummy(new(0, 10));
                ImGui.Separator();
                ImGui.Dummy(new(0, 10));
            }
        }

        ImGui.End();
    }
}