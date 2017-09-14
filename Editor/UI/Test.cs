using System.Numerics;
using Apogee.Resources;
using ImGuiNET;

namespace Editor.UI
{
    public unsafe class Test : Window
    {
        public override void Draw(Scene s)
        {
            if (ImGui.BeginWindow("Test"))
            {



                ImGui.EndWindow();
            }
        }
    }
}