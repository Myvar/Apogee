using System;
using Apogee.Resources;
using ImGuiNET;

namespace Editor.UI
{
    public class Debug : Window
    {
        public override void Draw(Scene s)
        {
            if (ImGui.BeginWindow("Debug"))
            {
                if (ImGui.CollapsingHeader("Rendering", TreeNodeFlags.DefaultOpen))
                {
                    ImGui.Checkbox("Wire frame", ref s.Engine.WireFrame);
                }


                ImGui.EndWindow();
            }
        }
    }
}