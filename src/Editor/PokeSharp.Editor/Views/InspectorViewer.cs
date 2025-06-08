using System.Numerics;
using FontAwesome;
using ImGuiNET;
using Microsoft.Xna.Framework;
using PokeSharp.Assets;
using PokeSharp.Assets.Services;
using PokeSharp.Assets.VFS;
using PokeSharp.Core.Logging;
using PokeSharp.Editor.Services;

using NVec4 = System.Numerics.Vector4;

namespace PokeSharp.Editor.Views;

public sealed class InspectorViewer : IEditorView
{
    private bool _canInspect;
    private Logger _logger;
    private AssetMetadata? _assetMetadata;
    private readonly IAssetMetadataStore _metadataStore;
    private readonly ISelectionManager _selectionService;

    public InspectorViewer(
        Logger logger,
        ISelectionManager selectionService,
        IAssetMetadataStore metadataStore)
    {
        _logger = logger;
        _metadataStore = metadataStore;

        _selectionService = selectionService;
        _selectionService.SelectionUpdated += OnSelectionChanged;
    }

    private void OnSelectionChanged(object? sender, SelectionUpdatedArgs e)
    {
        _canInspect = e.IsSingleSelect;
        if (!_canInspect) return;

        _assetMetadata = null;
        if (e.SelectedObject is IVirtualFile file)
        {
            if (!_metadataStore.Exists(file.Path))
                return;

            _logger.Trace($"Selection changed, loading asset metadata for '{file.Path}'");
            _assetMetadata = _metadataStore.GetMetadata(file.Path);

            if (_assetMetadata == null)
                _logger.Warn($"No metadata found for asset at path '{file.Path}'");
        }
    }

    public void DrawGui()
    {
        if (ImGui.Begin("Inspector"))
        {
            DrawInspectorContent();
        }

        ImGui.End();
    }

    private void DrawInspectorContent()
    {
        if (!_selectionService.IsSelecting)
        {
            DrawNoSelectionState();
        }
        else if (_selectionService.IsMultiSelect)
        {
            DrawMultiSelectionState();
        }
        else if (_canInspect)
        {
            DrawSingleSelectionState();
        }
    }

    private void DrawNoSelectionState()
    {
        ImGui.PushStyleColor(ImGuiCol.Text, new NVec4(0.6f, 0.6f, 0.6f, 1.0f));
        ImGui.Text("No object currently selected");
        ImGui.Separator();
        ImGui.Text("Select an item to inspect its properties.");
        ImGui.PopStyleColor();
    }

    private void DrawMultiSelectionState()
    {
        ImGui.Text($"Multiple Selection ({_selectionService.SelectionCount} items)");
        ImGui.Separator();

        var files = _selectionService.SelectedObjects.OfType<IVirtualFile>();
        var directories = _selectionService.SelectedObjects.OfType<IVirtualDirectory>();

        if (files.Any())
        {
            ImGui.Text($"Files: {files.Count()}");
            var totalSize = files.Sum(f => f.Size);
            ImGui.Text($"Total Size: {FormatFileSize(totalSize)}");

            var extensions = files.GroupBy(f => f.Path.Extension.ToLower())
                                 .OrderByDescending(g => g.Count());

            if (ImGui.CollapsingHeader("File Types"))
            {
                foreach (var group in extensions)
                {
                    ImGui.BulletText($"{group.Key}: {group.Count()} files");
                }
            }
        }

        if (directories.Any())
        {
            ImGui.Text($"Directories: {directories.Count()}");
        }

        ImGui.Separator();
        if (ImGui.Button("Delete Selected"))
        {
        }
        ImGui.SameLine();
        if (ImGui.Button("Move to..."))
        {
        }
    }

    private void DrawSingleSelectionState()
    {
        object selectedObj = _selectionService.SelectedObject!;
        DrawInspectorHeader(selectedObj);

        // ImGui.Separator();

        // DrawGeneralProperties(selectedObj);

        // ImGui.Separator();

        // DrawSpecificContent(selectedObj);

        // ImGui.Separator();
        // DrawContextualActions(selectedObj);
    }

    private void DrawInspectorHeader(object selectedObj)
    {
        if (selectedObj is IVirtualFile file)
        {
            var icon = GetIconForEntry(file);
            ImGui.Text(icon);
            ImGui.SameLine();
            ImGui.Text($"Import settings for {file.NameWithoutExtension}");

            ImGui.Dummy(new(0, 10));
            ImGui.Separator();
            ImGui.Dummy(new(0, 10));

            string uri = file.Path.Uri;
            ImGui.PushStyleColor(ImGuiCol.Text, new NVec4(0.7f, 0.7f, 0.7f, 1.0f));
            ImGui.Text("Path:");
            ImGui.SameLine(0, 20);
            ImGui.InputText("##Path", ref uri, 256, ImGuiInputTextFlags.ReadOnly);

            if (_assetMetadata != null)
            {
                ImGui.Text("Id");
                ImGui.SameLine(0, 20);
                ImGui.Text(_assetMetadata.Id.ToString());

                ImGui.Dummy(new(0, 10));
                ImGui.Separator();
                ImGui.Dummy(new(0, 10));

                ImGui.Text("State");
                ImGui.SameLine(0, 20);
                ImGui.Text(_assetMetadata.IsValid ? "Valid" : "Invalid");
                if (_assetMetadata.IsValid)
                {
                    if (_assetMetadata.AssetType != null)
                    {
                        string assetTypeName = _assetMetadata.AssetType.Name;
                        ImGui.Text("Asset Type:");
                        ImGui.SameLine(0, 20);
                        ImGui.InputText("##Asset", ref assetTypeName, 256, ImGuiInputTextFlags.ReadOnly);
                    }

                    if (_assetMetadata.Importer != null)
                    {
                        string importerType = _assetMetadata.Importer!.GetType().Name;
                        ImGui.Text("Importer Type:");
                        ImGui.SameLine(0, 20);
                        ImGui.InputText("##Importer", ref importerType, 256, ImGuiInputTextFlags.ReadOnly);
                    }
                }
                else
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, Color.Orange.ToVector4().ToNumerics());
                    ImGui.TextWrapped(_assetMetadata.ErrorMessage);
                    ImGui.PopStyleColor();
                }

                ImGui.Dummy(new(0, 10));
            }

            ImGui.PopStyleColor();
        }
    }

    private void DrawGeneralProperties(object selectedObj)
    {
        ImGui.Dummy(new(0, 10));

        if (ImGui.CollapsingHeader("Properties", ImGuiTreeNodeFlags.DefaultOpen))
        {
            ImGui.Indent();

            if (selectedObj is IVirtualEntry entry)
            {
                ImGui.Text($"Type: {GetFriendlyTypeName(entry)}");
                ImGui.Text($"Size: {FormatFileSize(entry.Size)}");
                // ImGui.Text($"Modified: {entry.LastModified:yyyy-MM-dd HH:mm:ss}");

                if (entry is IVirtualFile file)
                {
                    ImGui.Text($"Extension: {file.Path.Extension}");

                    // // Hash du fichier (calculé de manière asynchrone)
                    // if (_cachedData?.FileHash != null)
                    // {
                    //     ImGui.Text($"MD5: {_cachedData.FileHash}");
                    // }
                }

                if (entry is IVirtualDirectory dir)
                {
                    var filesCount = dir.GetFiles().Count();
                    var directoriesCount = dir.GetDirectories().Count();
                    ImGui.Text($"Items: {filesCount + directoriesCount}");
                    ImGui.Text($"Subdirectories: {directoriesCount}");
                    ImGui.Text($"Files: {filesCount}");
                }
            }

            ImGui.Unindent();
        }
    }

    private void DrawSpecificContent(object selectedObj)
    {
        if (selectedObj is IVirtualEntry entry)
        {
            // Renderer générique
            DrawGenericFileContent(entry);
        }
    }

    private void DrawGenericFileContent(IVirtualEntry entry)
    {
        if (entry is IVirtualFile file)
        {
            if (ImGui.CollapsingHeader("Preview"))
            {
                ImGui.Text("No preview available for this file type.");
                ImGui.Text("Supported formats: .txt, .json, .png, .jpg, .xml, .cs, .md");
            }
        }
    }

    private void DrawContextualActions(object selectedObj)
    {
        if (ImGui.CollapsingHeader("Actions"))
        {
            ImGui.Indent();

            if (selectedObj is IVirtualFile file)
            {
                if (ImGui.Button("Open"))
                {
                }
                ImGui.SameLine();
                if (ImGui.Button("Copy Path"))
                {
                    ImGui.SetClipboardText(file.Path.Uri);
                }

                ImGui.Separator();

                if (ImGui.Button("Rename"))
                {
                }
                ImGui.SameLine();
                if (ImGui.Button("Delete"))
                {
                }

                // if (IsTextFile(file))
                // {
                //     ImGui.Separator();
                //     if (ImGui.Button("Edit in Text Editor"))
                //     {
                //     }
                // }
            }

            if (selectedObj is IVirtualDirectory directory)
            {
                if (ImGui.Button("Open in File Explorer"))
                {
                }

                ImGui.Separator();

                if (ImGui.Button("Create Subfolder"))
                {
                }
                ImGui.SameLine();
                if (ImGui.Button("Create File"))
                {
                }
            }

            ImGui.Unindent();
        }
    }

    private static string GetIconForEntry(IVirtualEntry entry)
    {
        return entry switch
        {
            IVirtualDirectory => "📁",
            IVirtualFile file => file.Path.Extension.ToLower() switch
            {
                ".txt" => $"{FontAwesomeIcons.Text}",
                ".json" => $"{FontAwesomeIcons.Gear}",
                ".png" or ".jpg" or ".jpeg" => $"{FontAwesomeIcons.Image}",
                ".cs" => $"{FontAwesomeIcons.Laptop}",
                _ => $"{FontAwesomeIcons.Text}"
            },
            _ => $"{FontAwesomeIcons.Question}"
        };
    }

    private static string GetFriendlyTypeName(IVirtualEntry entry)
    {
        return entry switch
        {
            IVirtualDirectory => "Directory",
            IVirtualFile file => file.Path.Extension.ToUpper() + " File",
            _ => entry.GetType().Name
        };
    }

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB"];
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}