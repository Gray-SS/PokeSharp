using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using ImGuiNET;

namespace PokeSharp.Editor.Helpers;

public static class ImGuiHelper
{
    private static readonly Dictionary<string, object> _dragDropPayloads = new();
    private static readonly Dictionary<string, SelectionState> _selectionStates = new();

    public static void SetDragDropPayload(string payloadName, object payload)
    {
        ImGui.SetDragDropPayload(payloadName, IntPtr.Zero, 0);
        _dragDropPayloads[payloadName] = payload;
    }

    public static unsafe bool AcceptDragDropPayload<T>(string payloadName, [NotNullWhen(true)] out T? payload) where T : class
    {
        ImGuiPayloadPtr ptr = ImGui.AcceptDragDropPayload(payloadName);

        payload = null;
        if (ptr.NativePtr != null)
        {
            if (!_dragDropPayloads.Remove(payloadName, out object? foundPayload))
                throw new InvalidOperationException($"No payload was found with name '{payloadName}'. Please make sure you've used the '{nameof(SetDragDropPayload)}' method from '{nameof(ImGuiHelper)}'");

            payload = (T)foundPayload;
            return true;
        }

        return false;
    }

    private struct SelectionState
    {
        public double StartTime;
        public Vector2 StartPosition;
        public Vector2 StartPositionWindow;
        public bool IsWaiting;
        public bool IsActive;
        public bool WasCompleted;
    }

    public static bool BeginSelectionRect(string id, out (Vector2 Min, Vector2 Max) rect)
    {
        rect = default;
        ImGuiIOPtr io = ImGui.GetIO();

        if (!_selectionStates.TryGetValue(id, out SelectionState state))
        {
            state = new SelectionState();
        }

        // Commencer la sélection SEULEMENT si on a pas bougé pendant un petit délai
        if (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left) && !state.WasCompleted)
        {
            state.StartPosition = io.MousePos;
            state.StartPositionWindow = ImGui.GetMousePos();
            state.StartTime = ImGui.GetTime(); // Nouveau : temps de début
            state.IsWaiting = true; // Nouveau : état d'attente
            state.IsActive = false;
            _selectionStates[id] = state;
        }

        // Attendre un peu avant d'activer (ou attendre un mouvement minimum)
        if (state.IsWaiting && ImGui.IsMouseDown(ImGuiMouseButton.Left))
        {
            double currentTime = ImGui.GetTime();
            Vector2 currentPos = io.MousePos;
            float distance = Vector2.Distance(state.StartPosition, currentPos);

            // Activer si : délai écoulé (200ms) OU mouvement > 5 pixels
            if (currentTime - state.StartTime > 0.2f || distance > 5.0f)
            {
                state.IsActive = true;
                state.IsWaiting = false;
                _selectionStates[id] = state;
            }
        }

        // Annuler si on relâche pendant l'attente
        if (state.IsWaiting && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
        {
            state.IsWaiting = false;
            state.WasCompleted = true;
            _selectionStates[id] = state;
            return false;
        }


        if (state.IsActive && ImGui.IsMouseDown(ImGuiMouseButton.Left))
        {
            Vector2 currentPos = io.MousePos;
            Vector2 currentPosWindow = ImGui.GetMousePos();

            Vector2 minScreen = new Vector2(
                Math.Min(state.StartPosition.X, currentPos.X),
                Math.Min(state.StartPosition.Y, currentPos.Y)
            );
            Vector2 maxScreen = new Vector2(
                Math.Max(state.StartPosition.X, currentPos.X),
                Math.Max(state.StartPosition.Y, currentPos.Y)
            );

            rect = (
                new Vector2(
                    Math.Min(state.StartPositionWindow.X, currentPosWindow.X),
                    Math.Min(state.StartPositionWindow.Y, currentPosWindow.Y)
                ),
                new Vector2(
                    Math.Max(state.StartPositionWindow.X, currentPosWindow.X),
                    Math.Max(state.StartPositionWindow.Y, currentPosWindow.Y)
                )
            );


            float minSize = 5.0f;
            if (Math.Abs(maxScreen.X - minScreen.X) > minSize || Math.Abs(maxScreen.Y - minScreen.Y) > minSize)
            {
                var drawList = ImGui.GetForegroundDrawList();

                uint fillColor = ImGui.GetColorU32(ImGuiCol.ButtonHovered);
                fillColor = (fillColor & 0x00FFFFFF) | 0x40000000;
                drawList.AddRectFilled(minScreen, maxScreen, fillColor, 2f);

                uint borderColor = ImGui.GetColorU32(ImGuiCol.ButtonHovered);
                drawList.AddRect(minScreen, maxScreen, borderColor, 2f, ImDrawFlags.None, 1.5f);
            }

            return true;
        }


        if (state.IsActive && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
        {
            state.IsActive = false;
            state.WasCompleted = true;
            _selectionStates[id] = state;
            return false;
        }

        if (state.WasCompleted && !ImGui.IsMouseDown(ImGuiMouseButton.Left))
        {
            state.WasCompleted = false;
            _selectionStates[id] = state;
        }

        return false;
    }

    public static void CancelSelectionRect(string id)
    {
        _selectionStates.Remove(id);
    }

    public static void ClearAllSelections()
    {
        _selectionStates.Clear();
    }

    public static bool IsRectInSelection((Vector2 Min, Vector2 Max) selectionRect, Vector2 itemPos, Vector2 itemSize)
    {
        Vector2 itemMin = itemPos;
        Vector2 itemMax = itemPos + itemSize;

        return !(itemMax.X < selectionRect.Min.X ||
                 itemMin.X > selectionRect.Max.X ||
                 itemMax.Y < selectionRect.Min.Y ||
                 itemMin.Y > selectionRect.Max.Y);
    }

    public static bool IsPointInSelection((Vector2 Min, Vector2 Max) selectionRect, Vector2 point)
    {
        return point.X >= selectionRect.Min.X &&
               point.X <= selectionRect.Max.X &&
               point.Y >= selectionRect.Min.Y &&
               point.Y <= selectionRect.Max.Y;
    }
}