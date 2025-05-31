using FontAwesome;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NativeFileDialogSharp;
using PokeSharp.Assets;
using PokeSharp.Assets.VFS;
using PokeSharp.Assets.VFS.Events;
using PokeSharp.Core.Logging;
using PokeSharp.Editor.Helpers;
using PokeSharp.Editor.Services;

using NVec2 = System.Numerics.Vector2;

namespace PokeSharp.Editor.Views;

// TODO: Implement undo and redo actions
public sealed class ContentViewer : IGuiHook
{
    private bool _isDragging;
    private IVirtualDirectory _currentDirectory = null!;

    private bool _focusNewEntry;
    private string _newEntryName = string.Empty;

    #region Creating entry fields

    private bool _isCreatingEntry;
    private bool _isCreatingDirectory;

    #endregion // Creating entry fields

    #region Renaming entry fields

    private bool _isRenamingEntry;
    private VirtualPath? _renamedEntryPath;

    #endregion // Renaming entry fields

    private bool _isDirty;
    private readonly ILogger _logger;
    private readonly AssetPipeline _pipeline;
    private readonly IVirtualFileSystem _vfs;
    private readonly IContentNavigator _navigator;
    private readonly ISelectionManager _selectionManager;
    private readonly IEditorProjectManager _projectManager;

    private VolumeInfo _libVolume = null!;

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
        if (e.VolumeType != "library")
        {
            _navigator.NavigateTo(VirtualPath.Parse($"{e.Scheme}://"));
            _currentDirectory = _navigator.GetCurrentDirectory();
        }
        else
        {
            _libVolume = e;
        }

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
                HandleShortcuts();

                ImGui.EndChild();
            }

            HandleContentShortcuts();

            ImGui.PopStyleVar();
            ImGui.End();
        }
    }

    private void HandleContentShortcuts()
    {
        if (ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows))
        {
            if (ImGui.IsWindowFocused() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                _selectionManager.ClearSelection();
            }

            if (_isCreatingEntry || _isRenamingEntry)
            {
                if (ImGui.IsKeyPressed(ImGuiKey.Escape))
                {
                    _isCreatingEntry = false;
                    _isRenamingEntry = false;
                }
            }
            else if (_selectionManager.IsSelecting)
            {
                if (ImGui.IsKeyPressed(ImGuiKey.I) && ImGui.IsKeyDown(ImGuiKey.ModCtrl))
                    ImportFromDisk();

                if (ImGui.IsKeyPressed(ImGuiKey.Escape))
                {
                    _selectionManager.ClearSelection();
                }

                if (ImGui.IsKeyPressed(ImGuiKey.F2) && _selectionManager.IsSingleSelect && _selectionManager.SelectedObject is IVirtualEntry entry)
                {
                    BeginRenamingEntry(entry);
                }

                if (ImGui.IsKeyPressed(ImGuiKey.D) && ImGui.IsKeyDown(ImGuiKey.ModCtrl))
                {
                    foreach (IVirtualEntry item in _selectionManager.SelectedObjects.Cast<IVirtualEntry>())
                        item.Duplicate();

                    _selectionManager.ClearSelection();
                    Invalidate();
                }

                if (ImGui.IsKeyPressed(ImGuiKey.Delete) || ImGui.IsKeyPressed(ImGuiKey.Backspace))
                {
                    foreach (IVirtualEntry item in _selectionManager.SelectedObjects.Cast<IVirtualEntry>())
                        item.Delete();

                    _selectionManager.ClearSelection();
                    Invalidate();
                }
            }
        }
    }

    private void HandleShortcuts()
    {
        if (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
        {
            _selectionManager.ClearSelection();
        }
    }

    private void DrawSidebar()
    {
        float availX = ImGui.GetContentRegionAvail().X;
        float childWidth = availX * 0.2f;

        ImGui.SetNextWindowSize(new NVec2(), ImGuiCond.Appearing);
        ImGui.PushStyleColor(ImGuiCol.Button, Color.Transparent.ToVector4().ToNumerics());
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Color(30, 30, 30).ToVector4().ToNumerics());
        ImGui.PushStyleColor(ImGuiCol.Header, new Color(40, 40, 40).ToVector4().ToNumerics());
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

            HandleShortcuts();

            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.EndChild();
        }
    }

    private void DrawAssets()
    {
        if (ImGui.TreeNodeEx($"{FontAwesomeIcons.Cube}  Assets", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanAvailWidth))
        {
            var volumes = _vfs.GetVolumes().Where(x => x.VolumeType != "library");

            if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && volumes.Any())
                _navigator.NavigateTo(volumes.First().RootPath);

            foreach (VolumeInfo volume in volumes)
            {
                string icon = volume.VolumeType switch
                {
                    "local" => FontAwesomeIcons.House,
                    "ROM" => FontAwesomeIcons.Lock,
                    _ => FontAwesomeIcons.Question
                };

                var flags = volume == volumes.First() ? ImGuiTreeNodeFlags.DefaultOpen : ImGuiTreeNodeFlags.None;
                bool opened = ImGui.TreeNodeEx($"{icon}  {volume.DisplayName}", flags | ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.OpenOnArrow);

                if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                    _navigator.NavigateTo(volume.RootPath);

                if (opened)
                {
                    IVirtualDirectory dir = _vfs.GetDirectory(volume.RootPath);
                    DrawAssetDirectory(dir);

                    // _navigator.NavigateTo(dir.Path);
                    ImGui.TreePop();
                }
            }

            ImGui.TreePop();
        }

        if (_libVolume != null)
        {
            bool LibVolumeOpened = ImGui.TreeNodeEx($"{FontAwesomeIcons.Gear}  {_libVolume.DisplayName}", ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.OpenOnArrow);
            if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                _navigator.NavigateTo(_libVolume.RootPath);

            if (LibVolumeOpened)
            {
                IVirtualDirectory dir = _vfs.GetDirectory(_libVolume.RootPath);
                DrawAssetDirectory(dir);

                // _navigator.NavigateTo(dir.Path);
                ImGui.TreePop();
            }
        }
    }

    private void DrawAssetDirectory(IVirtualDirectory directory)
    {
        ImGui.PushID(directory.Path.Uri);

        bool isSelected = _selectionManager.SelectedObjects.Contains(directory);
        bool opened = ImGui.TreeNodeEx($"{FontAwesomeIcons.Folder}  {directory.Name}", ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.OpenOnArrow | (isSelected ? ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.None) | (directory.Path.IsRoot ? ImGuiTreeNodeFlags.DefaultOpen : ImGuiTreeNodeFlags.None));
        if (ImGui.BeginDragDropSource())
        {
            ImGuiHelper.SetDragDropPayload("MOVE_ENTRY", directory);
            ImGui.EndDragDropSource();
        }

        if (ImGui.BeginDragDropTarget())
        {
            if (ImGuiHelper.AcceptDragDropPayload("MOVE_ENTRY", out IVirtualEntry? toMoveEntry))
                toMoveEntry.Move(directory.Path);

            ImGui.EndDragDropTarget();
        }

        DrawItemContextMenu(directory);

        if (ImGui.IsItemHovered())
        {
            if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
            {
                _navigator.NavigateTo(directory.Path);
            }
            else if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                _selectionManager.SelectObject(directory);
            }
        }

        if (opened)
        {
            foreach (IVirtualDirectory childDir in directory.GetDirectories())
                DrawAssetDirectory(childDir);

            foreach (IVirtualFile childFile in directory.GetFiles())
            {
                ImGui.PushID(childFile.Path.Uri);

                ImGui.Dummy(NVec2.Zero);
                ImGui.SameLine();

                bool selected = _selectionManager.SelectedObjects.Contains(childFile);
                if (ImGui.Selectable($"{childFile.Name}", selected))
                {
                    _selectionManager.SelectObject(childFile);
                }

                if (ImGui.BeginDragDropSource())
                {
                    ImGuiHelper.SetDragDropPayload("MOVE_ENTRY", childFile);
                    ImGui.EndDragDropSource();
                }

                DrawItemContextMenu(childFile);

                if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                    _navigator.NavigateTo(directory.Path);

                ImGui.PopID();
            }

            ImGui.TreePop();
        }

        ImGui.PopID();
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

            if (ImGui.BeginItemTooltip())
            {
                ImGui.TextColored(Color.Orange.ToVector4().ToNumerics(), $"{FontAwesomeIcons.TriangleExclamation}  Currently not implemented");
                ImGui.EndTooltip();
            }

            // Import Button
            ImGui.SameLine();
            if (ImGui.Button($"{FontAwesomeIcons.FileImport}  Import") && _projectManager.ActiveProject != null)
                ImportFromDisk();

            if (ImGui.BeginItemTooltip())
            {
                ImGui.Text("Import an asset from the disk (Ctrl+O)");
                ImGui.EndTooltip();
            }

            ImGui.SameLine();
            if (ImGui.Button($"{FontAwesomeIcons.FloppyDisk}  Save"))
            {
                _logger.Warn("Saving is not implemented yet");
            }

            if (ImGui.BeginItemTooltip())
            {
                ImGui.Text("Save all and export all the assets (CTRL+S)");
                ImGui.EndTooltip();
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

            if (ImGui.BeginDragDropTarget())
            {
                if (ImGuiHelper.AcceptDragDropPayload("MOVE_ENTRY", out IVirtualEntry? toMoveEntry))
                    toMoveEntry.Move(current);

                ImGui.EndDragDropTarget();
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
        float padding = 25f;
        float thumbnailSize = 80f;
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

                if (ImGui.BeginMenu("Asset"))
                {

                    ImGui.EndMenu();
                }

                ImGui.EndMenu();
            }

            ImGui.Separator();

            if (ImGui.MenuItem("Refresh"))
            {
                Invalidate();
                ImGui.End();
            }

            if (ImGui.MenuItem("Export to disk"))
            {

                ImGui.End();
            }

            if (ImGui.MenuItem("Import from disk"))
            {
                ImportFromDisk();
                ImGui.End();
            }

            ImGui.Separator();
            if (ImGui.MenuItem("Reimport all"))
            {
                _pipeline.ReimportAll();
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

            if (ImGui.MenuItem("Rename", "F2", false, !isReadOnly))
            {
                BeginRenamingEntry(item);
            }

            if (ImGui.MenuItem("Delete", "Delete", false, !isReadOnly))
            {
                item.Delete();
                Invalidate();
            }

            if (ImGui.MenuItem("Duplicate", "Ctrl+D", false, !isReadOnly))
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

            ImGui.Separator();

            if (ImGui.MenuItem("Reimport all"))
            {
                _pipeline.ReimportAll();
            }

            ImGui.EndPopup();
        }
    }

    private void DrawVirtualContent(float padding, float thumbnailSize)
    {
        bool selecting = false;
        var selectionRect = (NVec2.Zero, NVec2.Zero);

        if (!_isDragging)
        {
            selecting = ImGuiHelper.BeginSelectionRect("EntriesSelection", out selectionRect);
            if (selecting && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                _selectionManager.ClearSelection();
        }
        else _isDragging = false;

        if (_isCreatingEntry && _isCreatingDirectory)
        {
            DrawCreatingEntry(thumbnailSize, padding);
            ImGui.NextColumn();
        }

        foreach (IVirtualDirectory directory in _directories)
        {
            DrawVirtualEntry(directory, thumbnailSize, padding, selecting, selectionRect);
        }

        if (_isCreatingEntry && !_isCreatingDirectory)
        {
            DrawCreatingEntry(thumbnailSize, padding);
            ImGui.NextColumn();
        }

        foreach (IVirtualFile file in _files)
        {
            DrawVirtualEntry(file, thumbnailSize, padding, selecting, selectionRect);
        }
    }

    private void DrawVirtualEntry(IVirtualEntry entry, float thumbnailSize, float padding, bool selecting, (NVec2 Min, NVec2 Max) selectionRect)
    {
        ImGui.PushID(entry.Path.Uri);
        NVec2 itemPos = ImGui.GetCursorScreenPos();

        if (_isRenamingEntry && _renamedEntryPath == entry.Path)
        {
            DrawRenamingEntry(thumbnailSize, padding);
        }
        else
        {
            DrawEntry(entry, thumbnailSize, padding);
        }

        if (selecting)
        {
            NVec2 itemSize = new NVec2(thumbnailSize + padding, thumbnailSize + padding);

            if (ImGuiHelper.IsRectInSelection(selectionRect, itemPos, itemSize))
            {
                _selectionManager.SelectObject(entry, true);
            }
            else _selectionManager.UnselectObject(entry);
        }

        ImGui.PopID();
        ImGui.NextColumn();
    }

    private void DrawEntry(IVirtualEntry entry, float thumbnailSize, float padding)
    {
        // ImGui.SetCursorPos(ImGui.GetCursorPos() + new NVec2(padding * 0.5f));

        bool isDirectory = entry.IsDirectory;

        DrawEntryIcon(entry, isDirectory ? "folder" : "file", thumbnailSize, padding);
        DrawItemContextMenu(entry);

        if (ImGui.IsItemHovered())
        {
            if (isDirectory && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
            {
                _navigator.NavigateTo(entry.Path);
            }
            else if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                bool isAdditive = ImGui.IsKeyDown(ImGuiKey.ModCtrl);
                if (isAdditive && _selectionManager.SelectedObjects.Contains(entry))
                {
                    _selectionManager.UnselectObject(entry);
                }
                else _selectionManager.SelectObject(entry, isAdditive);
            }
        }

        float imageWidth = ImGui.GetItemRectSize().X;
        string entryName = entry.Name;
        if (entryName.Length >= 10)
        {
            entryName = entryName[0..10] + "...";
        }

        float textWidth = ImGui.CalcTextSize(entryName).X;

        float cursorX = ImGui.GetCursorPosX();

        float textOffsetX = MathF.Max(0, (imageWidth - textWidth) / 2.0f);

        ImGui.Dummy(new(0, 3));
        ImGui.SetCursorPosX(cursorX + textOffsetX);

        ImGui.TextWrapped(entryName);
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
            // TODO: Add some feedback if the directory or the file already exists
            if (_isCreatingDirectory && !_currentDirectory.DirectoryExists(_newEntryName))
            {
                _currentDirectory.CreateDirectory(_newEntryName);
                Invalidate();
            }
            else if (!_isCreatingDirectory && !_currentDirectory.DirectoryExists(_newEntryName))
            {
                _currentDirectory.CreateFile(_newEntryName);
                Invalidate();
            }

            _isCreatingEntry = false;
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
        ImGui.Image(_icons[_renamedEntryPath!.IsDirectory ? "folder" : "file"], new NVec2(thumbnailSize));
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
            IVirtualEntry entry = _renamedEntryPath.IsDirectory ?
                _currentDirectory.GetDirectory(_renamedEntryPath!.Name) :
                _currentDirectory.GetFile(_renamedEntryPath!.Name);

            if (entry.Exists)
            {
                try
                {
                    entry.Rename(_newEntryName);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error occured while renaming {entry.Path} to {_newEntryName}: {ex.Message}");
                }
            }
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

    private void DrawEntryIcon(IVirtualEntry entry, string icon, float thumbnailSize, float padding)
    {
        bool isSelected = _selectionManager.SelectedObjects.Contains(entry);
        Color bgColor = isSelected ? new Color(50, 50, 50) : new Color(30, 30, 30);
        Color hoveredColor = isSelected ? new Color(60, 60, 60) : new Color(40, 40, 40);
        Color activeColor = isSelected ? new Color(70, 70, 70) : new Color(50, 50, 50);

        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new NVec2(padding, padding) * 0.5f);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 20f);
        ImGui.PushStyleColor(ImGuiCol.Button, bgColor.ToVector4().ToNumerics());
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, hoveredColor.ToVector4().ToNumerics());
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, activeColor.ToVector4().ToNumerics());

        ImGui.ImageButton($"##{entry.Name}", _icons[icon], new NVec2(thumbnailSize - padding * 0.5f));
        if (ImGui.BeginDragDropSource())
        {
            _isDragging = true;
            ImGuiHelper.SetDragDropPayload("MOVE_ENTRY", entry);
            ImGui.ImageButton($"##{entry.Name}_dragndrop", _icons[icon], new NVec2(thumbnailSize - padding * 0.5f) * 0.3f);
            ImGui.EndDragDropSource();
        }

        if (entry.IsDirectory && ImGui.BeginDragDropTarget())
        {
            if (ImGuiHelper.AcceptDragDropPayload("MOVE_ENTRY", out IVirtualEntry? toMoveEntry))
                toMoveEntry.Move(entry.Path);

            ImGui.EndDragDropTarget();
        }

        if (!_isDragging && ImGui.BeginItemTooltip())
        {
            ImGui.Text(entry.Name);
            ImGui.EndTooltip();
        }

        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        ImGui.PopStyleVar();
        ImGui.PopStyleVar();
    }

    // TODO: Add a target path to know where we need to create the entry
    private void BeginCreatingEntry(string defaultName, bool isDirectory)
    {
        _isCreatingEntry = true;
        _isCreatingDirectory = isDirectory;
        _focusNewEntry = true;
        _newEntryName = defaultName;
    }

    private void BeginRenamingEntry(IVirtualEntry entry)
    {
        _isRenamingEntry = true;
        _focusNewEntry = true;
        _newEntryName = entry.Name;
        _renamedEntryPath = entry.Path;
        _selectionManager.SelectObject(entry);

        _navigator.NavigateTo(entry.Path.GetParent());
    }

    private void Invalidate()
    {
        _isDirty = true;
    }

    private void ImportFromDisk()
    {
        if (!_projectManager.HasActiveProject)
            return;

        var result = Dialog.FileOpen();
        if (result.IsOk)
        {
            string projectRoot = _projectManager.ActiveProject!.ContentRoot;
            string fileName = Path.GetFileName(result.Path);
            string destination = Path.Combine(projectRoot, fileName);

            File.Copy(result.Path, destination, overwrite: true);

            var relative = Path.GetRelativePath(projectRoot, destination);
            var vpath = VirtualPath.Parse($"local://{relative}");
            _pipeline.TryImport(vpath);

            _navigator.NavigateTo(vpath.GetParent());
        }
    }

    private void UpdateFilesAndDirectories()
    {
        _logger.Debug("Updating file directories");
        _files.Clear();
        _directories.Clear();

        _files.AddRange(_currentDirectory.GetFiles());
        _directories.AddRange(_currentDirectory.GetDirectories());

        foreach (var item in _selectionManager.SelectedObjects)
        {
            if (item is IVirtualEntry entry)
                _selectionManager.UnselectObject(entry);
        }

        if (!_currentDirectory.Exists)
            _navigator.NavigateTo(VirtualPath.Root(_currentDirectory.Path.Scheme));
    }
}