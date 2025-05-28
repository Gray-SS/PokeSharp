using FontAwesome;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NativeFileDialogSharp;
using PokeSharp.Assets;
using PokeSharp.Assets.VFS;
using PokeSharp.Assets.VFS.Events;
using PokeSharp.Core.Logging;
using PokeSharp.Editor.Services;

using NVec2 = System.Numerics.Vector2;

namespace PokeSharp.Editor.Views;

// TODO: Implement undo and redo actions
// TODO: Add a selection service to manage selected asset
// TODO: Add drag and drop
public sealed class ContentViewer : IGuiHook
{
    private IVirtualDirectory _currentDirectory = null!;

    private bool _focusNewEntry;
    private string _newEntryName = string.Empty;

    #region Creating entry fields

    private bool _isCreatingEntry;
    private bool _isCreatingDirectory;

    #endregion // Creating entry fields

    #region Renaming entry fields

    private bool _isRenamingEntry;
    private bool _isRenamingDirectory;
    private VirtualPath? _renamedEntryPath;

    #endregion // Renaming entry fields

    private bool _isDirty;
    private readonly ILogger _logger;
    private readonly AssetPipeline _pipeline;
    private readonly IVirtualFileSystem _vfs;
    private readonly IContentNavigator _navigator;
    private readonly ISelectionManager _selectionManager;
    private readonly IEditorProjectManager _projectManager;

    private readonly Dictionary<string, nint> _icons;
    private readonly List<IVirtualFile> _files;
    private readonly List<IVirtualDirectory> _directories;

    public ContentViewer(
        AssetPipeline pipeline,
        GraphicsDevice graphicsDevice,
        IVirtualFileSystem vfs,
        IContentNavigator navigator,
        IGuiResourceManager textureManager,
        IEditorProjectManager projectManager,
        ISelectionManager selectionManager,
        ILogger logger)
    {
        _logger = logger;
        _projectManager = projectManager;
        _selectionManager = selectionManager;
        _pipeline = pipeline;
        _files = new List<IVirtualFile>();
        _directories = new List<IVirtualDirectory>();

        _navigator = navigator;
        _navigator.CurrentPathChanged += OnCurrentPathChanged;

        _vfs = vfs;
        _vfs.OnFileChanged += OnFileChanged;
        _vfs.OnVolumeMounted += OnVolumeMounted;

        // TODO: When the pipeline will be linked to the virtual file system
        //       we will be able to load the texture from the asset pipeline instead of using Texture2D.FromStream
        _icons = new Dictionary<string, nint>()
        {
            ["folder"] = LoadAndRegisterTexture(textureManager, graphicsDevice, AppContext.BaseDirectory, "Resources", "folder.png"),
            ["file"] = LoadAndRegisterTexture(textureManager, graphicsDevice, AppContext.BaseDirectory, "Resources", "file.png"),
            ["hard-drive"] = LoadAndRegisterTexture(textureManager, graphicsDevice, AppContext.BaseDirectory, "Resources", "hard-drive.png"),
        };
    }

    private void OnVolumeMounted(object? sender, VolumeInfo e)
    {
        _logger.Debug($"Volume mounted. Getting current directory");
        _navigator.NavigateTo(VirtualPath.Parse($"{e.Scheme}://"));
        _currentDirectory = _navigator.GetCurrentDirectory();
        Invalidate();
    }

    private void OnCurrentPathChanged(object? sender, VirtualPath e)
    {
        _logger.Debug($"Current path changed '{e.LocalPath}'");
        _currentDirectory = _navigator.GetCurrentDirectory();
        Invalidate();
    }

    private void OnFileChanged(object? sender, FileSystemChangedArgs e)
    {
        _logger.Debug($"File change detected '{e.Type}' - {e.FullVirtualPath}");
        Invalidate();
    }

    private static nint LoadAndRegisterTexture(IGuiResourceManager textureManager, GraphicsDevice device, params string[] paths)
    {
        string path = Path.Combine(paths);
        Texture2D texture = Texture2D.FromFile(device, path);

        return textureManager.RegisterTexture(texture);
    }

    public void DrawGui()
    {
        if (_isDirty)
        {
            UpdateFilesAndDirectories();
            _isDirty = false;
        }

        if (ImGui.Begin("Content Browser"))
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, 26f);
            DrawNavigationBar();

            DrawSidebar();

            ImGui.SameLine();

            if (ImGui.BeginChild("##browser", new NVec2(0, 0), ImGuiChildFlags.Border))
            {
                if (!_projectManager.HasActiveProject)
                {
                    ImGui.Text("No project is currently opened, try to load a project first before using the content browser");
                }
                else DrawContent();

                ImGui.End();
            }

            ImGui.PopStyleVar();

            ImGui.End();
        }
    }

    private void DrawSidebar()
    {
        float availX = ImGui.GetContentRegionAvail().X;
        float childWidth = availX * 0.2f;

        ImGui.SetNextWindowSize(new NVec2(), ImGuiCond.Appearing);
        ImGui.PushStyleColor(ImGuiCol.Button, Color.Transparent.ToVector4().ToNumerics());
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Color(30, 30, 30).ToVector4().ToNumerics());
        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Color(40, 40, 40).ToVector4().ToNumerics());
        ImGui.PushStyleColor(ImGuiCol.HeaderActive, new Color(60, 60, 60).ToVector4().ToNumerics());
        if (ImGui.BeginChild("##file_system_tree", new NVec2(childWidth, 0), ImGuiChildFlags.Border | ImGuiChildFlags.ResizeX))
        {
            DrawAssets();

            ImGui.Dummy(new(0, 5));
            ImGui.Separator();
            ImGui.Dummy(new(0, 5));

            if (ImGui.TreeNodeEx($"{FontAwesomeIcons.Star}  Recent/Favorites"))
            {
                ImGui.TextColored(new Color(180, 180, 180).ToVector4().ToNumerics(), $"Not implemented yet");
                ImGui.TreePop();
            }

            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.EndChild();
        }
    }

    private void DrawAssets()
    {
        if (ImGui.TreeNodeEx($"{FontAwesomeIcons.Cube}  Assets", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.SpanAvailWidth))
        {
            var volumes = _vfs.GetVolumes();
            foreach (VolumeInfo volume in volumes)
            {
                string icon = volume.Scheme == "fs" ? FontAwesomeIcons.FloppyDisk : FontAwesomeIcons.Adn;
                var flags = volume == volumes.First() ? ImGuiTreeNodeFlags.DefaultOpen : ImGuiTreeNodeFlags.None;
                if (ImGui.TreeNodeEx($"{icon}  {volume.DisplayName}", flags | ImGuiTreeNodeFlags.SpanAvailWidth))
                {
                    IVirtualDirectory dir = _vfs.GetDirectory(volume.RootPath);
                    DrawAssetDirectory(dir);

                    ImGui.TreePop();
                }
            }

            ImGui.TreePop();
        }
    }

    private void DrawAssetDirectory(IVirtualDirectory directory)
    {
        if (ImGui.TreeNode($"{FontAwesomeIcons.Folder}  {directory.Name}"))
        {
            foreach (IVirtualDirectory childDir in directory.GetDirectories())
                DrawAssetDirectory(childDir);

            foreach (IVirtualFile childFile in directory.GetFiles())
            {
                ImGui.Selectable($"{FontAwesomeIcons.File}  {childFile.Name}");
            }

            ImGui.TreePop();
        }
    }

    private void DrawNavigationBar()
    {
        bool isEnabled = _projectManager.HasActiveProject;

        if (!isEnabled)
            return;

        if (ImGui.BeginChild("##navbar", new NVec2(0, 40), ImGuiChildFlags.Border))
        {
            ImGui.PushStyleColor(ImGuiCol.Button, new Color(40, 40, 40).ToVector4().ToNumerics());
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Color(50, 50, 50).ToVector4().ToNumerics());
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Color(60, 60, 60).ToVector4().ToNumerics());
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 5);
            if (ImGui.Button($"{FontAwesomeIcons.Plus} New"))
            {
                ImGui.OpenPopup("AddPopup");
            }

            // Import Button
            ImGui.SameLine();
            if (ImGui.Button($"{FontAwesomeIcons.FileImport}  Import"))
            {
                var result = Dialog.FileOpen();
                if (result.IsOk)
                {
                    // TODO: Import the file
                    _logger.Info($"Importing file from path: {result.Path}");
                }
            }

            ImGui.SameLine();
            if (ImGui.Button($"{FontAwesomeIcons.FloppyDisk}  Save"))
            {
                _logger.Warn("Saving is not implemented yet");
            }
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.PopStyleVar();

            ImGui.SameLine(0, 20f);
            DrawNavBackButton();

            ImGui.SameLine();
            DrawNavForwardButton();

            ImGui.SameLine(0, 20f);
            float size = ImGui.GetContentRegionAvail().Y;
            ImGui.Image(_icons["folder"], new(size, size));
            ImGui.SameLine();

            DrawFoldersBreadcrumb();

            ImGui.EndChild();
        }
    }

    private void DrawFoldersBreadcrumb()
    {
        if (_currentDirectory == null)
            return;

        List<string> segments = [string.Empty];
        segments.AddRange(_navigator.CurrentPath.LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries));

        VirtualPath current = null!;
        for (int i = 0; i < segments.Count; i++)
        {
            string segment = segments[i];
            if (segment == string.Empty) current = VirtualPath.Root(_navigator.CurrentPath.Scheme);
            else current = current.Combine(segment + "/");

            string displayName = current.IsRoot ? "Root" : segment;

            Color color = i == segments.Count - 1 ? new Color(40, 40, 40) : Color.Transparent;

            ImGui.PushStyleColor(ImGuiCol.Button, color.ToVector4().ToNumerics());
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Color(50, 50, 50).ToVector4().ToNumerics());
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 5f);
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, 5f);

            if (ImGui.Button(displayName))
            {
                _navigator.NavigateTo(current);
            }

            if (i < segments.Count - 1)
            {
                ImGui.SameLine();
                ImGui.Text(">");
                ImGui.SameLine();
            }


            ImGui.PopStyleVar();
            ImGui.PopStyleVar();
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
        }
    }

    private void DrawNavForwardButton()
    {
        bool isActive = _navigator.CanNavigateForward();
        PushNavButtonStyle(isActive);
        if (ImGui.ArrowButton("##forward", ImGuiDir.Right) && isActive)
        {
            _navigator.NavigateForward();
        }
        PopNavButtonStyle();
    }

    private void DrawNavBackButton()
    {
        bool isActive = _navigator.CanNavigateBack();
        PushNavButtonStyle(isActive);
        if (ImGui.ArrowButton("##back", ImGuiDir.Left) && isActive)
        {
            _navigator.NavigateBack();
        }
        PopNavButtonStyle();
    }

    private static void PushNavButtonStyle(bool isActive)
    {
        Color textColor = isActive ? Color.White : new Color(120, 120, 120);
        Color bgColor = new Color(40, 40, 40);
        Color hoveredColor = isActive ? new Color(50, 50, 50) : bgColor;
        Color activeColor = isActive ? new Color(60, 60, 60) : bgColor;

        ImGui.PushStyleColor(ImGuiCol.Text, textColor.ToVector4().ToNumerics());
        ImGui.PushStyleColor(ImGuiCol.Button, bgColor.ToVector4().ToNumerics());
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, activeColor.ToVector4().ToNumerics());
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, hoveredColor.ToVector4().ToNumerics());
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 5f);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, 5f);
    }

    private static void PopNavButtonStyle()
    {
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        ImGui.PopStyleVar();
        ImGui.PopStyleVar();
    }

    private void DrawContent()
    {
        float padding = 20f;
        float thumbnailSize = 100f;
        float cellSize = thumbnailSize + padding;
        float panelWidth = ImGui.GetContentRegionAvail().X;

        int columnsCount = Math.Max(1, (int)(panelWidth / cellSize));
        ImGui.Columns(columnsCount, string.Empty, false);
        DrawVirtualContent(padding, thumbnailSize);
        DrawWindowContextMenu();
        ImGui.Columns(1);
    }

    private void DrawWindowContextMenu()
    {
        // TODO: Need a way to handle read only access
        bool isReadOnly = false;
        if (ImGui.BeginPopupContextWindow("##window_context_menu", ImGuiPopupFlags.MouseButtonRight | ImGuiPopupFlags.NoOpenOverItems))
        {
            if (ImGui.BeginMenu("New"))
            {
                if (ImGui.MenuItem("Directory", !isReadOnly))
                {
                    BeginCreatingEntry("New Directory", isDirectory: true);
                    Invalidate();
                }

                if (ImGui.MenuItem("File", !isReadOnly))
                {
                    BeginCreatingEntry("New File", isDirectory: false);
                    Invalidate();
                }

                ImGui.EndMenu();
            }

            ImGui.EndPopup();
        }
    }

    private void DrawItemContextMenu(IVirtualEntry item)
    {
        // TODO: Need a way to handle read only access
        bool isReadOnly = false;
        bool isDirectory = item.IsDirectory;

        if (ImGui.BeginPopupContextItem("##item_context_menu", ImGuiPopupFlags.MouseButtonRight))
        {
            if (ImGui.MenuItem("Open", isDirectory))
            {
                _navigator.NavigateTo(item.Path);
            }

            if (ImGui.MenuItem("Rename", !isReadOnly))
            {
                BeginRenamingEntry(item.Name, item.Path, isDirectory);
            }

            if (ImGui.MenuItem("Delete", !isReadOnly))
            {
                item.Delete();
                Invalidate();
            }

            if (ImGui.MenuItem("Duplicate", !isReadOnly))
            {
                item.Duplicate();
                Invalidate();
            }

            if (ImGui.MenuItem("Copy Path"))
            {
                // TODO: Add a clipboard service
                // TODO: Add a feedback to validate the path has been copied
                ImGui.SetClipboardText(item.Path.ToString());
            }

            ImGui.EndPopup();
        }
    }

    private void DrawVirtualContent(float padding, float thumbnailSize)
    {
        if (_isCreatingEntry && _isCreatingDirectory)
        {
            DrawCreatingEntry(thumbnailSize, padding);
            ImGui.NextColumn();
        }

        foreach (IVirtualDirectory directory in _directories)
        {
            ImGui.PushID(directory.Path.ToString());

            if (_isRenamingEntry && _renamedEntryPath == directory.Path)
            {
                DrawRenamingEntry(thumbnailSize, padding);
            }
            else
            {
                DrawEntry(directory, thumbnailSize, padding);
            }

            ImGui.PopID();
            ImGui.NextColumn();
        }

        if (_isCreatingEntry && !_isCreatingDirectory)
        {
            DrawCreatingEntry(thumbnailSize, padding);
            ImGui.NextColumn();
        }

        foreach (IVirtualFile file in _files)
        {
            ImGui.PushID(file.Path.ToString());

            if (_isRenamingEntry && _renamedEntryPath == file.Path)
            {
                DrawRenamingEntry(thumbnailSize, padding);
            }
            else
            {
                DrawEntry(file, thumbnailSize, padding);
            }

            ImGui.PopID();
            ImGui.NextColumn();
        }
    }

    private void DrawEntry(IVirtualEntry entry, float thumbnailSize, float padding)
    {
        // ImGui.SetCursorPos(ImGui.GetCursorPos() + new NVec2(padding * 0.5f));

        bool isDirectory = entry.IsDirectory;
        DrawEntryIcon(entry.Name, isDirectory ? "folder" : "file", thumbnailSize, padding);

        DrawItemContextMenu(entry);
        if (ImGui.IsItemHovered())
        {
            if (isDirectory && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
            {
                _navigator.NavigateTo(entry.Path);

            }
            else if (!isDirectory && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                _selectionManager.SelectObject(entry);
            }
        }

        float imageWidth = ImGui.GetItemRectSize().X;
        float textWidth = ImGui.CalcTextSize(entry.Name).X;

        float cursorX = ImGui.GetCursorPosX();

        float textOffsetX = textWidth >= imageWidth ? padding : (imageWidth - textWidth) / 2.0f;

        ImGui.Dummy(new(0, 3));
        ImGui.SetCursorPosX(cursorX + textOffsetX);

        ImGui.TextWrapped(entry.Name);
    }

    private void DrawCreatingEntry(float thumbnailSize, float padding)
    {
        ImGui.SetCursorPos(ImGui.GetCursorPos() + new NVec2(padding * 0.5f));
        ImGui.Image(_icons[_isCreatingDirectory ? "folder" : "file"], new NVec2(thumbnailSize));
        ImGui.SetCursorPos(ImGui.GetCursorPos() + new NVec2(padding * 0.5f));

        ImGui.PushID("new_entry_input");

        ImGui.SetNextItemWidth(thumbnailSize + padding);
        if (_focusNewEntry)
        {
            ImGui.SetKeyboardFocusHere();
            _focusNewEntry = false;
        }

        bool validated = ImGui.InputText("##new_entry", ref _newEntryName, 256, ImGuiInputTextFlags.EnterReturnsTrue);
        bool canceled = !ImGui.IsItemActive() && !ImGui.IsItemHovered() && (ImGui.IsMouseClicked(ImGuiMouseButton.Left) || ImGui.IsMouseClicked(ImGuiMouseButton.Right));

        if (validated)
        {
            if (_isCreatingDirectory)
                _currentDirectory.CreateDirectory(_newEntryName);
            else
                _currentDirectory.CreateFile(_newEntryName);

            _isCreatingEntry = false;
            Invalidate();
        }
        else if (canceled)
        {
            _isCreatingEntry = false;
        }

        ImGui.PopID();
    }

    private void DrawRenamingEntry(float thumbnailSize, float padding)
    {
        ImGui.SetCursorPos(ImGui.GetCursorPos() + new NVec2(padding * 0.5f));
        ImGui.Image(_icons[_isRenamingDirectory ? "folder" : "file"], new NVec2(thumbnailSize));
        ImGui.SetCursorPos(ImGui.GetCursorPos() + new NVec2(padding * 0.5f));

        ImGui.PushID("rename_entry_input");

        ImGui.SetNextItemWidth(thumbnailSize + padding);
        if (_focusNewEntry)
        {
            ImGui.SetKeyboardFocusHere();
            _focusNewEntry = false;
        }

        bool validated = ImGui.InputText("##input_rename_entry", ref _newEntryName, 256, ImGuiInputTextFlags.EnterReturnsTrue);
        bool canceled = !ImGui.IsItemActive() && !ImGui.IsItemHovered() && (ImGui.IsMouseClicked(ImGuiMouseButton.Left) || ImGui.IsMouseClicked(ImGuiMouseButton.Right));

        if (validated)
        {
            IVirtualEntry? entry = _isRenamingDirectory ?
                _directories.FirstOrDefault(x => x.Path == _renamedEntryPath) :
                _files.FirstOrDefault(x => x.Path == _renamedEntryPath);

            if (entry != null) entry.Rename(_newEntryName);
            else _logger.Warn($"The renamed entry at path '{_renamedEntryPath}' was not found.");

            _isRenamingEntry = false;
            Invalidate();
        }
        else if (canceled)
        {
            _isRenamingEntry = false;
        }

        ImGui.PopID();
    }

    private void DrawEntryIcon(string entryName, string icon, float thumbnailSize, float padding)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new NVec2(padding, padding) * 0.5f);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 20f);
        ImGui.PushStyleColor(ImGuiCol.Button, new Color(30, 30, 30).ToVector4().ToNumerics());
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Color(40, 40, 40).ToVector4().ToNumerics());
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Color(50, 50, 50).ToVector4().ToNumerics());
        ImGui.ImageButton($"##{entryName}", _icons[icon], new NVec2(thumbnailSize));
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        ImGui.PopStyleVar();
        ImGui.PopStyleVar();
    }

    private void BeginCreatingEntry(string defaultName, bool isDirectory)
    {
        _isCreatingEntry = true;
        _isCreatingDirectory = isDirectory;
        _focusNewEntry = true;
        _newEntryName = defaultName;
    }

    private void BeginRenamingEntry(string entryName, VirtualPath entryPath, bool isDirectory)
    {
        _isRenamingEntry = true;
        _focusNewEntry = true;
        _newEntryName = entryName;
        _renamedEntryPath = entryPath;
        _isRenamingDirectory = isDirectory;
    }

    private void Invalidate()
    {
        _isDirty = true;
    }

    private void UpdateFilesAndDirectories()
    {
        _logger.Debug("Updating file directories");
        _files.Clear();
        _directories.Clear();

        _files.AddRange(_currentDirectory.GetFiles());
        _directories.AddRange(_currentDirectory.GetDirectories());

        _logger.Debug($"Current Directory: {_currentDirectory.Path}");
        foreach (IVirtualDirectory dir in _directories)
        {
            _logger.Trace($"- {dir.Path}");
        }
        foreach (IVirtualFile file in _files)
        {
            _logger.Trace($"- {file.Path}");
        }
    }
}