using System;
using System.Reflection;
using Apogee.Resources;
using ImGuiNET;

namespace Editor.UI
{
    public class MenuBar : Window
    {
        public MenuBar()
        {
            Visible = true;
        }
        
        public override void Draw(Scene s)
        {
            if (ImGui.BeginMainMenuBar())
            {

                if (ImGui.BeginMenu("File"))
                {
                    ImGui.Separator();

                    if (ImGui.MenuItem("Exit"))
                    {
                        Environment.Exit(0);
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Window"))
                {
                    foreach (var window in Window.Windows)
                    {
                        if (ImGui.MenuItem(window.GetType().GetTypeInfo().Name) && 
                            window.GetType().GetTypeInfo().Name != "MenuBar")
                        {
                            window.Visible = !window.Visible;
                        }
                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
                Visible = true;
            }
        }
    }
}