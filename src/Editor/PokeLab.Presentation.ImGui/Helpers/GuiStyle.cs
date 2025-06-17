using ImGuiNET;

namespace PokeLab.Presentation.ImGui.Helpers;

public enum ButtonState
{
    Enabled,
    Disabled,
}

public static class GuiStyle
{
    public static void Initialize()
    {
        ImGuiStylePtr stylePtr = Gui.GetStyle();
        stylePtr.FrameRounding = 4;
        stylePtr.WindowRounding = 4;
        stylePtr.PopupRounding = 4;
        stylePtr.ChildRounding = 4;
        stylePtr.WindowPadding = new NVec2(12, 12);
        stylePtr.WindowMenuButtonPosition = ImGuiDir.None;

        var colors = stylePtr.Colors;
        colors[(int)ImGuiCol.WindowBg] = new NVec4(0.06f, 0.04f, 0.06f, 0.94f);
        colors[(int)ImGuiCol.FrameBg] = new NVec4(0.17f, 0.13f, 0.18f, 0.64f);
        colors[(int)ImGuiCol.TitleBg] = new NVec4(0.00f, 0.00f, 0.00f, 1.00f);
        colors[(int)ImGuiCol.TitleBgActive] = new NVec4(0.08f, 0.08f, 0.08f, 1.00f);
        colors[(int)ImGuiCol.MenuBarBg] = new NVec4(0.07f, 0.07f, 0.07f, 1.00f);
        colors[(int)ImGuiCol.Header] = new NVec4(0.17f, 0.13f, 0.18f, 0.64f);
        colors[(int)ImGuiCol.SeparatorHovered] = new NVec4(0.17f, 0.13f, 0.18f, 0.64f);
        colors[(int)ImGuiCol.ResizeGrip] = new NVec4(0.32f, 0.16f, 0.35f, 0.64f);
        colors[(int)ImGuiCol.ResizeGripHovered] = new NVec4(0.50f, 0.28f, 0.53f, 0.64f);
        colors[(int)ImGuiCol.ResizeGripActive] = new NVec4(0.68f, 0.44f, 0.72f, 0.64f);
        colors[(int)ImGuiCol.Tab] = new NVec4(0.07f, 0.03f, 0.10f, 0.00f);
        colors[(int)ImGuiCol.TabSelected] = new NVec4(0.29f, 0.17f, 0.32f, 0.64f);
        colors[(int)ImGuiCol.TabSelectedOverline] = new NVec4(0.54f, 0.35f, 0.58f, 0.64f);
        colors[(int)ImGuiCol.TabDimmedSelected] = new NVec4(0.29f, 0.16f, 0.32f, 0.64f);
        colors[(int)ImGuiCol.FrameBgHovered] = new NVec4(0.25f, 0.21f, 0.27f, 0.64f);
        colors[(int)ImGuiCol.FrameBgActive] = new NVec4(0.31f, 0.23f, 0.32f, 0.64f);
        colors[(int)ImGuiCol.CheckMark] = new NVec4(0.78f, 0.64f, 0.82f, 0.64f);
        colors[(int)ImGuiCol.SliderGrab] = new NVec4(0.38f, 0.31f, 0.40f, 0.64f);
        colors[(int)ImGuiCol.SliderGrabActive] = new NVec4(0.51f, 0.33f, 0.56f, 0.64f);
        colors[(int)ImGuiCol.TabHovered] = new NVec4(0.54f, 0.35f, 0.58f, 0.64f);
        colors[(int)ImGuiCol.DockingPreview] = new NVec4(0.44f, 0.31f, 0.48f, 0.64f);
        colors[(int)ImGuiCol.Button] = new NVec4(0.32f, 0.23f, 0.34f, 0.64f);
        colors[(int)ImGuiCol.ButtonHovered] = new NVec4(0.43f, 0.29f, 0.47f, 0.64f);
        colors[(int)ImGuiCol.ButtonActive] = new NVec4(0.55f, 0.29f, 0.61f, 0.64f);
        colors[(int)ImGuiCol.HeaderHovered] = new NVec4(0.24f, 0.17f, 0.26f, 0.64f);
        colors[(int)ImGuiCol.HeaderActive] = new NVec4(0.32f, 0.19f, 0.35f, 0.64f);
    }

    public static void PushButtonStyle(ButtonState state)
    {
        switch (state)
        {
            case ButtonState.Disabled:
                NVec4 disabledColor = Gui.GetStyle().Colors[(int)ImGuiCol.Button] with
                {
                    W = 0.35f
                };

                Gui.PushStyleColor(ImGuiCol.Button, disabledColor);
                Gui.PushStyleColor(ImGuiCol.ButtonHovered, disabledColor);
                Gui.PushStyleColor(ImGuiCol.ButtonActive, disabledColor);
                break;
        }
    }

    public static void PopButtonStyle(ButtonState state)
    {
        switch (state)
        {
            case ButtonState.Disabled:
                Gui.PopStyleColor(3);
                break;
        }
    }
}