using System.Numerics;
using FontAwesome;
using ImGuiNET;
using PokeSharp.Assets;
using PokeSharp.Assets.VFS;
using PokeSharp.Editor.Services;

namespace PokeSharp.Editor.Views;

public sealed class InspectorViewer : IGuiHook
{
    private bool _canInspect;
    private readonly AssetPipeline _assetPipeline;
    private readonly ISelectionManager _selectionService;


    public InspectorViewer(ISelectionManager selectionService, AssetPipeline assetPipeline)
    {
        _assetPipeline = assetPipeline;

        _selectionService = selectionService;
        _selectionService.SelectionUpdated += OnSelectionChanged;
    }

    private void OnSelectionChanged(object? sender, SelectionUpdatedArgs e)
    {
        _canInspect = e.IsSingleSelect;
        if (!_canInspect) return;

    }

    public void DrawGui()
    {
        if (ImGui.Begin("Inspector"))
        {
            DrawInspectorContent();
            ImGui.End();
        }
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
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.6f, 0.6f, 0.6f, 1.0f));
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

        ImGui.Separator();

        DrawGeneralProperties(selectedObj);

        ImGui.Separator();

        DrawSpecificContent(selectedObj);

        ImGui.Separator();
        DrawContextualActions(selectedObj);
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
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.7f, 0.7f, 0.7f, 1.0f));
            ImGui.InputText("Path", ref uri, 256, ImGuiInputTextFlags.ReadOnly);

            if (_assetPipeline.HasMetadata(file.Path))
            {
                AssetMetadata metadata = _assetPipeline.GetMetadata(file.Path);
                if (metadata.HasResource)
                {
                    string? importerType = metadata.Importer!.GetType().Name;
                    ImGui.InputText("Importer", ref importerType, 256, ImGuiInputTextFlags.ReadOnly);

                    string? processorType = metadata.Processor!.GetType().Name;
                    ImGui.InputText("Processor", ref processorType, 256, ImGuiInputTextFlags.ReadOnly);
                }
            }

            ImGui.PopStyleColor();
        }
    }

    private void DrawGeneralProperties(object selectedObj)
    {
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