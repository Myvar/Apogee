using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Apogee.Components;
using Apogee.Core;
using Apogee.Ecs;
using Apogee.Systems;
using Apogee.UI;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;

namespace Apogee
{
    public class Editor
    {
        public static Editor Instance { get; set; } = new Editor();
      
       
        public void Draw()
        {
            ImGui.BeginMainMenuBar();


            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Open"))
                {
                    Dialog.FileSelector(s => GameEngine.Container.LoadFile(s));
                }

                if (ImGui.MenuItem("Save"))
                {
                    Dialog.TextDialog(s =>
                    {
                        var len = s.Replace("\0", "").Length;
                        var x = new string(s.Trim().Normalize().ToArray(), 0, len);
                        GameEngine.Container.SaveFile(Path.Combine(Container.AssetsPath, x));
                    });
                }

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();

            Dialog.Render();
            
            
            Window.Render();
        }
    }
}