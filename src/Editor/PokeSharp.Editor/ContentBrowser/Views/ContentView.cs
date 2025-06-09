using FontAwesome;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NativeFileDialogSharp;
using PokeSharp.Assets;
using PokeSharp.Assets.Services;
using PokeSharp.Assets.VFS;
using PokeSharp.Assets.VFS.Services;
using PokeSharp.Assets.VFS.Volumes;
using PokeSharp.Core.Logging;
using PokeSharp.Editor.ContentBrowser.Services;
using PokeSharp.Editor.Helpers;
using PokeSharp.Editor.Services;
using PokeSharp.Engine.Core.Threadings;
using NVec2 = System.Numerics.Vector2;

namespace PokeSharp.Editor.ContentBrowser.Views;

// TODO: Implement undo and redo actions
public sealed class ContentView : IEditorView
{
    private bool _isDragging;

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

    private readonly Logger _logger;
    private readonly AssetPipeline _assetPipeline;
    private readonly IAssetMetadataStore _metadataStore;
    private readonly IVirtualFileSystem _vfs;
    private readonly ISelectionManager _selectionManager;
    private readonly IProjectManager _projectManager;
    private readonly IContentCacheService _cacheService;
    private readonly IContentNavigator _navigator;

    private static readonly ImGuiTreeNodeFlags _defaultTreeNodeFlags = ImGuiTreeNodeFlags.OpenOnArrow |
                                                                       ImGuiTreeNodeFlags.SpanAvailWidth;

    private readonly Dictionary<string, nint> _icons;

    public ContentView(
        AssetPipeline pipeline,
        GraphicsDevice graphicsDevice,
        IVirtualFileSystem vfs,
        IGuiResourceManager textureManager,
        IProjectManager projectManager,
        ISelectionManager selectionManager,
        IContentNavigator navigator,
        IContentCacheService cacheService,
        IAssetMetadataStore metadataStore,
        Logger logger)
    {
        _vfs = vfs;
        _logger = logger;
        _metadataStore = metadataStore;
        _projectManager = projectManager;
        _selectionManager = selectionManager;
        _cacheService = cacheService;
        _assetPipeline = pipeline;
        _navigator = navigator;

        _icons = new Dictionary<string, nint>()
        {
            ["folder"] = LoadAndRegisterTexture(textureManager, graphicsDevice, AppContext.BaseDirectory, "Resources", "folder.png"),
            ["file"] = LoadAndRegisterTexture(textureManager, graphicsDevice, AppContext.BaseDirectory, "Resources", "file.png"),
            ["invalid-asset"] = LoadAndRegisterTexture(textureManager, graphicsDevice, AppContext.BaseDirectory, "Resources", "question-mark.png")
        };
    }

    private static nint LoadAndRegisterTexture(IGuiResourceManager textureManager, GraphicsDevice device, params string[] paths)
    {
        string path = Path.Combine(paths);
        Texture2D texture = Texture2D.FromFile(device, path);

        return textureManager.RegisterTexture(texture);
    }

    public void DrawGui()
    {
        if (ImGui.Begin("Content Browser"))
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new NVec2(10f));

            DrawNavigationBar();
            DrawProjectHierarchy();

            ImGui.SameLine();

            if (ImGui.BeginChild("##browser", new NVec2(0, 0), ImGuiChildFlags.Borders))
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
        }

        ImGui.End();
    }

    private void HandleContentShortcuts()
    {
        if (ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows))
        {
            if (ImGui.IsWindowFocused() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                _selectionManager.ClearSelection();
            }

            if (ImGui.IsKeyPressed(ImGuiKey.I) && ImGui.IsKeyDown(ImGuiKey.ModCtrl))
                ImportFromDisk();

            if (ImGui.IsKeyPressed(ImGuiKey.R) && ImGui.IsKeyDown(ImGuiKey.ModCtrl))
                _cacheService.Invalidate(ContentScope.CurrentDirectory);

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
                if (ImGui.IsKeyPressed(ImGuiKey.Escape))
                {
                    _selectionManager.ClearSelection();
                }

                if (ImGui.IsKeyPressed(ImGuiKey.F2) && _selectionManager.IsSingleSelect && _selectionManager.SelectedObject is IVirtualEntry entry)
                {
                    BeginRenamingEntry(entry);
                }

                if (_selectionManager.IsSingleSelect && _selectionManager.SelectedObject is IVirtualDirectory dir)
                {
                    if (ImGui.IsKeyPressed(ImGuiKey.Enter))
                    {
                        _navigator.NavigateTo(dir.Path);
                    }
                }

                if (ImGui.IsKeyPressed(ImGuiKey.D) && ImGui.IsKeyDown(ImGuiKey.ModCtrl))
                {
                    foreach (IVirtualEntry item in _selectionManager.SelectedObjects.Cast<IVirtualEntry>())
                    {
                        bool isReadOnly = item.Volume.IsReadOnly;
                        if (isReadOnly) continue;

                        _assetPipeline.TryDuplicate(item.Path);
                    }

                    _selectionManager.ClearSelection();
                    _cacheService.Invalidate(ContentScope.CurrentDirectory);
                }

                if (ImGui.IsKeyPressed(ImGuiKey.Delete) || ImGui.IsKeyPressed(ImGuiKey.Backspace))
                {
                    foreach (IVirtualEntry item in _selectionManager.SelectedObjects.Cast<IVirtualEntry>())
                    {
                        bool isReadOnly = item.Volume.IsReadOnly;
                        if (isReadOnly) continue;

                        _assetPipeline.TryDelete(item.Path);
                    }

                    _selectionManager.ClearSelection();
                    _cacheService.Invalidate(ContentScope.CurrentDirectory);
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

    private void DrawProjectHierarchy()
    {
        float availX = ImGui.GetContentRegionAvail().X;
        float childWidth = availX * 0.2f;

        ImGui.SetNextWindowSize(NVec2.Zero, ImGuiCond.Appearing);
        if (ImGui.BeginChild("##project_hierarchy", new NVec2(childWidth, 0), ImGuiChildFlags.Borders | ImGuiChildFlags.ResizeX))
        {
            DrawProjectHierarchyTreeNodes();

            ImGui.Dummy(new(0, 5));
            ImGui.Separator();
            ImGui.Dummy(new(0, 5));

            if (ImGui.TreeNodeEx($"{FontAwesomeIcons.Star}  Recent/Favorites"))
            {
                ImGui.TextColored(new Color(180, 180, 180).ToVector4().ToNumerics(), $"Not implemented yet");
                ImGui.TreePop();
            }


            ImGui.EndChild();
        }
    }

    private void DrawProjectHierarchyTreeNodes()
    {
        if (!Project.IsActive) return;

        DrawAssetsTreeNode();
        DrawLibraryTreeNode();
    }

    private void DrawLibraryTreeNode()
    {
        IVirtualVolume libsVolume = Project.Active.LibsVolume;
        if (libsVolume != null)
        {
            bool libVolumeOpened = ImGui.TreeNodeEx($"{FontAwesomeIcons.Gear}  {libsVolume.DisplayName}", ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.OpenOnArrow);
            if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                _navigator.NavigateTo(libsVolume.RootPath);

            if (libVolumeOpened)
            {
                DrawAssetDirectory(libsVolume.RootDirectory);
                ImGui.TreePop();
            }
        }
    }

    private void DrawAssetsTreeNode()
    {
        IVirtualVolume libsVolume = Project.Active.LibsVolume;
        if (ImGui.TreeNodeEx($"{FontAwesomeIcons.Cube}  Assets", ImGuiTreeNodeFlags.DefaultOpen | _defaultTreeNodeFlags))
        {
            var volumes = _cacheService.Volumes.Where(x => x.Id != libsVolume.Id);

            if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && volumes.Any())
                _navigator.NavigateTo(volumes.First().RootPath);

            foreach (IVirtualVolume volume in volumes)
            {
                string icon = volume.Id switch
                {
                    "local" => FontAwesomeIcons.House,
                    "ROM" => FontAwesomeIcons.Lock,
                    _ => FontAwesomeIcons.PuzzlePiece
                };

                var flags = volume == volumes.First() ? ImGuiTreeNodeFlags.DefaultOpen : ImGuiTreeNodeFlags.None;
                bool opened = ImGui.TreeNodeEx($"{icon}  {volume.DisplayName}", flags | _defaultTreeNodeFlags);

                if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                    _navigator.NavigateTo(volume.RootPath);

                if (opened)
                {
                    DrawAssetDirectory(volume.RootDirectory);
                    ImGui.TreePop();
                }
            }

            ImGui.TreePop();
        }
    }

    private void DrawAssetDirectory(IVirtualDirectory directory)
    {
        ImGui.PushID(directory.Path.Uri);

        bool opened = false;
        if (!directory.IsRoot)
        {
            bool isSelected = _selectionManager.SelectedObjects.Contains(directory);
            bool isLeaf = !directory.GetDirectories().Any();
            opened = ImGui.TreeNodeEx($"{FontAwesomeIcons.Folder}  {directory.Name}", ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.OpenOnArrow | (isSelected ? ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.None) | (directory.Path.IsRoot ? ImGuiTreeNodeFlags.DefaultOpen : ImGuiTreeNodeFlags.None) | (isLeaf ? ImGuiTreeNodeFlags.Leaf : ImGuiTreeNodeFlags.None));

            if (ImGui.BeginDragDropSource())
            {
                ImGuiHelper.SetDragDropPayload("MOVE_ENTRY", directory);
                ImGui.EndDragDropSource();
            }
        }

        if (ImGui.BeginDragDropTarget())
        {
            if (ImGuiHelper.AcceptDragDropPayload("MOVE_ENTRY", out IVirtualEntry? toMoveEntry))
                _assetPipeline.TryMove(toMoveEntry.Path, directory.Path);

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
                bool additive = ImGui.IsKeyDown(ImGuiKey.ModCtrl);
                _selectionManager.SelectObject(directory, additive);
            }
        }

        if (directory.IsRoot || opened)
        {
            foreach (IVirtualDirectory childDir in directory.GetDirectories())
                DrawAssetDirectory(childDir);

            if (!directory.IsRoot)
                ImGui.TreePop();
        }

        ImGui.PopID();
    }


    private void DrawNavigationBar()
    {
        bool isEnabled = _projectManager.HasActiveProject;

        if (!isEnabled)
            return;

        bool isReadOnly = _navigator.CurrentDirectory.Volume.IsReadOnly;
        if (ImGui.BeginChild("##navbar", new NVec2(0, 40), ImGuiChildFlags.Borders))
        {
            Color textColor = !isReadOnly ? Color.White : new Color(180, 180, 180);
            Color buttonColor = !isReadOnly ? new Color(40, 40, 40) : new Color(30, 30, 30);
            Color buttonHoveredColor = !isReadOnly ? new Color(50, 50, 50) : buttonColor;
            Color buttonActiveColor = !isReadOnly ? new Color(60, 60, 60) : buttonColor;

            ImGui.PushStyleColor(ImGuiCol.Text, textColor.ToVector4().ToNumerics());
            ImGui.PushStyleColor(ImGuiCol.Button, buttonColor.ToVector4().ToNumerics());
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, buttonHoveredColor.ToVector4().ToNumerics());
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, buttonActiveColor.ToVector4().ToNumerics());
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 5);

            if (ImGui.Button($"{FontAwesomeIcons.Plus} New") && !isReadOnly)
            {
                ImGui.OpenPopup("AddPopup");
            }

            if (ImGui.BeginItemTooltip())
            {
                if (isReadOnly)
                {
                    ImGui.TextColored(Color.Orange.ToVector4().ToNumerics(), $"{FontAwesomeIcons.TriangleExclamation}  Your current volume is read-only. Please navigate to a not read-only volume to create a new asset");
                }
                else ImGui.TextColored(Color.Orange.ToVector4().ToNumerics(), $"{FontAwesomeIcons.TriangleExclamation}  Currently not implemented");

                ImGui.EndTooltip();
            }

            // Import Button
            ImGui.SameLine();
            if (ImGui.Button($"{FontAwesomeIcons.FileImport}  Import") && _projectManager.ActiveProject != null)
                ImportFromDisk();

            if (ImGui.BeginItemTooltip())
            {
                if (isReadOnly)
                {
                    ImGui.TextColored(Color.Orange.ToVector4().ToNumerics(), $"{FontAwesomeIcons.TriangleExclamation}  Your current volume is read-only. Please navigate to a not read-only volume to import an asset");
                }
                else ImGui.Text("Import an asset from the disk (Ctrl+I)");
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

            ImGui.PopStyleColor(4);
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
        if (_navigator.CurrentDirectory == null)
            return;

        List<string> segments = [string.Empty];
        segments.AddRange(_navigator.CurrentPath.LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries));

        VirtualPath current = null!;
        for (int i = 0; i < segments.Count; i++)
        {
            string segment = segments[i];
            if (segment == string.Empty) current = VirtualPath.BuildRoot(_navigator.CurrentPath.Scheme);
            else current = current.Combine(segment + "/");

            string displayName = current.IsRoot ? "Root" : segment;

            Color color = i == segments.Count - 1 ? new Color(40, 40, 40) : Color.Transparent;

            ImGui.PushStyleColor(ImGuiCol.Button, color.ToVector4().ToNumerics());
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Color(50, 50, 50).ToVector4().ToNumerics());
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 5f);
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new NVec2(3f));

            if (ImGui.Button(displayName))
            {
                _navigator.NavigateTo(current);
            }

            if (ImGui.BeginDragDropTarget())
            {
                if (ImGuiHelper.AcceptDragDropPayload("MOVE_ENTRY", out IVirtualEntry? toMoveEntry))
                    _assetPipeline.TryMove(toMoveEntry.Path, current);

                ImGui.EndDragDropTarget();
            }

            if (i < segments.Count - 1)
            {
                ImGui.SameLine();
                ImGui.Text(">");
                ImGui.SameLine();
            }


            ImGui.PopStyleVar(2);
            ImGui.PopStyleColor(2);
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
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new NVec2(3f));
    }

    private static void PopNavButtonStyle()
    {
        ImGui.PopStyleColor(4);
        ImGui.PopStyleVar(2);
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
        bool isReadOnly = _navigator.CurrentDirectory.Volume.IsReadOnly;
        if (ImGui.BeginPopupContextWindow("##window_context_menu", ImGuiPopupFlags.MouseButtonRight | ImGuiPopupFlags.NoOpenOverItems))
        {
            if (ImGui.BeginMenu("New", !isReadOnly))
            {
                if (ImGui.MenuItem("Directory", !isReadOnly))
                {
                    BeginCreatingEntry("New Directory", isDirectory: true);
                }

                if (ImGui.MenuItem("File", !isReadOnly))
                {
                    BeginCreatingEntry("New File", isDirectory: false);
                }

                if (ImGui.BeginMenu("Asset", !isReadOnly))
                {

                    ImGui.EndMenu();
                }

                ImGui.EndMenu();
            }

            ImGui.Separator();

            if (ImGui.MenuItem("Refresh", "Ctrl+R", false, true))
            {
                _cacheService.Refresh(ContentScope.CurrentDirectory);
            }

            if (ImGui.MenuItem("Export to disk"))
            {

            }

            if (ImGui.MenuItem("Import from disk", "Ctrl+I", false, !isReadOnly))
            {
                ImportFromDisk();
            }

            ImGui.Separator();
            if (ImGui.MenuItem("Reimport all"))
            {
                _assetPipeline.ReimportAll();
            }

            ImGui.EndPopup();
        }
    }

    private void DrawItemContextMenu(IVirtualEntry item)
    {
        bool isReadOnly = item.Volume.IsReadOnly;
        bool isDirectory = item.IsDirectory;

        if (ImGui.BeginPopupContextItem("##item_context_menu", ImGuiPopupFlags.MouseButtonRight))
        {
            if (isDirectory)
            {
                if (ImGui.MenuItem("Open", "Enter"))
                {
                    _navigator.NavigateTo(item.Path);
                }

                ImGui.Separator();
            }

            if (ImGui.MenuItem("Rename", "F2", false, !isReadOnly))
            {
                BeginRenamingEntry(item);
            }

            if (ImGui.MenuItem("Delete", "Delete", false, !isReadOnly))
            {
                _assetPipeline.TryDelete(item.Path);
            }

            if (ImGui.MenuItem("Duplicate", "Ctrl+D", false, !isReadOnly))
            {
                _assetPipeline.TryDuplicate(item.Path);
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
                _assetPipeline.ReimportAll();
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

        foreach (IVirtualDirectory directory in _cacheService.Directories)
        {
            DrawVirtualEntry(directory, thumbnailSize, padding, selecting, selectionRect);
        }

        if (_isCreatingEntry && !_isCreatingDirectory)
        {
            DrawCreatingEntry(thumbnailSize, padding);
            ImGui.NextColumn();
        }

        foreach (IVirtualFile file in _cacheService.Files)
        {
            DrawVirtualEntry(file, thumbnailSize, padding, selecting, selectionRect);
        }
    }

    private void DrawVirtualEntry(IVirtualEntry entry, float thumbnailSize, float padding, bool selecting, (NVec2 Min, NVec2 Max) selectionRect)
    {
        ImGui.PushID(entry.Path.Uri);
        NVec2 itemPos = ImGui.GetCursorScreenPos();

        bool isReadOnly = entry.Volume.IsReadOnly;
        if (!isReadOnly && _isRenamingEntry && _renamedEntryPath == entry.Path)
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
        bool isDirectory = entry.IsDirectory;

        DrawEntryIcon(entry, thumbnailSize, padding);
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
            var currentDirectory = _navigator.CurrentDirectory;
            if (_isCreatingDirectory && !currentDirectory.DirectoryExists(_newEntryName))
            {
                currentDirectory.CreateDirectory(_newEntryName);
                _assetPipeline.ReimportAll();
            }
            else if (!_isCreatingDirectory && !currentDirectory.DirectoryExists(_newEntryName))
            {
                currentDirectory.CreateFile(_newEntryName);
                _assetPipeline.ReimportAll();
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
        ImGui.Image(_icons[_renamedEntryPath!.IsDirectory ? "folder" : "file"], new NVec2(thumbnailSize - padding * 0.5f));
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
            IVirtualEntry entry = _vfs.GetEntry(_renamedEntryPath);
            if (entry.Exists)
            {
                try
                {
                    _assetPipeline.TryRename(entry.Path, _newEntryName);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error occured while renaming {entry.Path} to {_newEntryName}: {ex.Message}");
                }
            }
            else _logger.Warn($"The renamed entry at path '{_renamedEntryPath}' was not found.");

            _isRenamingEntry = false;
        }
        else if (canceled)
        {
            _isRenamingEntry = false;
        }

        ImGui.PopID();
    }

    private void DrawEntryIcon(IVirtualEntry entry, float thumbnailSize, float padding)
    {
        bool isReadOnly = entry.Volume.IsReadOnly;
        bool isSelected = _selectionManager.SelectedObjects.Contains(entry);
        Color bgColor = isSelected ? new Color(50, 50, 50) : new Color(30, 30, 30);
        Color hoveredColor = isSelected ? new Color(60, 60, 60) : new Color(40, 40, 40);
        Color activeColor = isSelected ? new Color(70, 70, 70) : new Color(50, 50, 50);

        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new NVec2(padding * 0.5f));
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 20f);
        ImGui.PushStyleColor(ImGuiCol.Button, bgColor.ToVector4().ToNumerics());
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, hoveredColor.ToVector4().ToNumerics());
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, activeColor.ToVector4().ToNumerics());

        string icon = "folder";
        AssetMetadata? metadata = _metadataStore.GetMetadata(entry.Path);
        if (metadata != null && metadata.IsValid) icon = "file";
        else if (!entry.IsDirectory) icon = "invalid-asset";

        ImGui.ImageButton($"##{entry.Name}", _icons[icon], new NVec2(thumbnailSize - padding * 0.5f));
        if (!isReadOnly && ImGui.BeginDragDropSource())
        {
            _isDragging = true;
            ImGuiHelper.SetDragDropPayload("MOVE_ENTRY", entry);
            ImGui.ImageButton($"##{entry.Name}_dragndrop", _icons[icon], new NVec2(thumbnailSize - padding * 0.5f) * 0.3f);
            ImGui.EndDragDropSource();
        }

        if (!isReadOnly && entry.IsDirectory && ImGui.BeginDragDropTarget())
        {
            if (ImGuiHelper.AcceptDragDropPayload("MOVE_ENTRY", out IVirtualEntry? toMoveEntry))
                _assetPipeline.TryMove(toMoveEntry.Path, entry.Path);

            ImGui.EndDragDropTarget();
        }

        if (!_isDragging && ImGui.BeginItemTooltip())
        {
            if (!entry.IsDirectory)
            {
                if (metadata == null)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, Color.Orange.ToVector4().ToNumerics());
                    ImGui.Text($"{FontAwesomeIcons.TriangleExclamation}  No metadata found for this asset.");
                    ImGui.PopStyleColor();
                }
                else if (!metadata.IsValid)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, Color.Orange.ToVector4().ToNumerics());
                    ImGui.Text($"{FontAwesomeIcons.TriangleExclamation}  {metadata.ErrorMessage}");
                    ImGui.PopStyleColor();
                    ImGui.PushStyleColor(ImGuiCol.Text, Color.DarkGray.ToVector4().ToNumerics());
                    ImGui.Text(metadata.Id.ToString());
                    ImGui.PopStyleColor();
                }
                else
                {
                    ImGui.Text($"{entry.Name} - {metadata.AssetType.Name}");
                    ImGui.PushStyleColor(ImGuiCol.Text, Color.DarkGray.ToVector4().ToNumerics());
                    ImGui.Text(metadata.Id.ToString());
                    ImGui.PopStyleColor();
                }
            }
            else ImGui.Text(entry.Name);

            ImGui.EndTooltip();
        }

        ImGui.PopStyleColor(3);
        ImGui.PopStyleVar(2);
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
        if (entry.Volume.IsReadOnly)
            return;

        _isRenamingEntry = true;
        _focusNewEntry = true;
        _newEntryName = entry.Name;
        _renamedEntryPath = entry.Path;
        _selectionManager.SelectObject(entry);

        _navigator.NavigateTo(entry.Path.GetParent());
    }

    private void ImportFromDisk()
    {
        var currentDirectory = _navigator.CurrentDirectory;
        if (!_projectManager.HasActiveProject || currentDirectory == null || currentDirectory.Volume.IsReadOnly)
            return;

        Task.Run(() =>
        {
            var result = Dialog.FileOpen();
            if (result.IsOk)
            {
                string projectRoot = _projectManager.ActiveProject!.AssetsRoot;
                string fileName = Path.GetFileName(result.Path);
                string destination = Path.Combine(projectRoot, currentDirectory.Path.LocalPath, fileName);

                File.Copy(result.Path, destination, overwrite: true);

                var relative = Path.GetRelativePath(projectRoot, destination);
                var vpath = VirtualPath.Parse($"local://{relative}");

                ThreadHelper.RunOnMainThread(() => _assetPipeline.Import(vpath));
            }
        });
    }
}