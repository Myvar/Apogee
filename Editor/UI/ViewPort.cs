using System;
using System.Numerics;
using Apogee;
using Apogee.Resources;
using ImGuiNET;

namespace Editor.UI
{
    public class ViewPort : Window
    {
        public ViewPort()
        {
            Visible = true;
        }

        public override void Draw(Scene s)
        {
            var scene = s as MainScene;
            if (ImGui.BeginWindow("MainViewPort", WindowFlags.AlwaysAutoResize))
            {
                
                scene.Camera.CanMove = ImGui.IsMouseHoveringWindow();
                
                int x = scene.MainView.Texture;
                ImGui.Text("FPS: " + Math.Round(scene.Engine.Window.RenderFrequency, 0));


                ImGui.Image(new IntPtr(x), new Vector2(800, 600), new Vector2(0, 1), new Vector2(1, 0),
                    new Vector4(1, 1, 1, 1),
                    new Vector4(0, 0, 0, 0));

                ImGui.EndWindow();
            }
        }
    }
}