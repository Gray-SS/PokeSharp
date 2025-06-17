using ImGuiNET;
using PokeEngine.Core.Timing;

namespace PokeLab.Presentation.ImGui.Helpers;

public static class GuiHelper
{
    public static bool Button(string text, bool isEnabled)
    {
        ButtonState state = isEnabled ? ButtonState.Enabled : ButtonState.Disabled;
        GuiStyle.PushButtonStyle(state);
        bool isClicked = Gui.Button(text);
        GuiStyle.PopButtonStyle(state);

        return isClicked && isEnabled;
    }

    public static bool Button(string text, NVec2 size, bool isEnabled)
    {
        ButtonState state = isEnabled ? ButtonState.Enabled : ButtonState.Disabled;
        GuiStyle.PushButtonStyle(state);
        bool isClicked = Gui.Button(text, size);
        GuiStyle.PopButtonStyle(state);

        return isClicked && isEnabled;
    }

    public static unsafe bool LoadingSpinner(float radius, int thickness, Color color)
    {
        var drawList = Gui.GetWindowDrawList();
        if (drawList.NativePtr == null)
            return false;

        ImGuiIOPtr io = Gui.GetIO();
        ImGuiStylePtr style = Gui.GetStyle();

        NVec2 pos = Gui.GetCursorScreenPos();
        NVec2 size = new(radius * 2f, (radius + style.FramePadding.Y) * 2f);
        NVec2 center = new(pos.X + radius, pos.Y + radius + style.FramePadding.Y);

        Gui.Dummy(size); // Reserve space
        if (!Gui.IsItemVisible())
            return false;

        int numSegments = 30;
        int start = (int)(Math.Abs(MathF.Sin(io.DeltaTime + GameTime.TotalTime * 1.8f)) * (numSegments - 5));

        float aMin = 2 * MathF.PI * start / numSegments;
        float aMax = 2 * MathF.PI * (numSegments - 3) / numSegments;

        drawList.PathClear();

        for (int i = 0; i < numSegments; i++)
        {
            float a = aMin + (float)i / numSegments * (aMax - aMin);
            float angle = a + GameTime.TotalTime * 8;
            NVec2 point = new(
                center.X + MathF.Cos(angle) * radius,
                center.Y + MathF.Sin(angle) * radius
            );
            drawList.PathLineTo(point);
        }

        uint colorId = Gui.GetColorU32(color.ToVector4().ToNumerics());
        drawList.PathStroke(colorId, ImDrawFlags.None, thickness);
        return true;
    }
}