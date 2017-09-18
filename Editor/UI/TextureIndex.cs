using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Apogee.Resources;
using ImGuiNET;

namespace Editor.UI
{
    public class TextureIndex : Window
    {
        public TextureIndex()
        {
            Visible = true;
        }

        public Dictionary<string, Texture> Textures { get; set; } = new Dictionary<string, Texture>();

        public void Load(Scene s)
        {
            if(Textures.Count != 0) return;

            var x = s.Engine.Assets.IndexTextures();

            foreach (var text in x)
            {
                Textures.Add(text, Texture.LoadTexture(text) as Texture);
            }
        }
        
        public override void Draw(Scene s)
        {
            Load(s);
            
            ImGui.BeginWindow("TextureIndex");
            
            uint c = 0;
            int l = 0;
            foreach (var texture in Textures)
            {
                
                if (l < 4)
                {
                    ImGui.SameLine();
                }
                else
                {
                    l = 0;
                }

                ImGui.BeginChildFrame(c++, new Vector2(128, 128) + new Vector2(0, 50), WindowFlags.Default);
                
                ImGui.Text(new FileInfo(texture.Key).Name);
                
                ImGui.Image(new IntPtr(texture.Value.TextureID), new Vector2(128, 128), new Vector2(0, 1), new Vector2(1, 0),
                    new Vector4(1, 1, 1, 1),
                    new Vector4(0, 0, 0, 0));


                if (ImGui.Button("Apply"))
                {
                    ObjectEditor.CurrentWorldObject.Texture = texture.Value;
                }
                
                ImGui.EndChildFrame();
                l++;
            }
            
            ImGui.EndWindow();
        }
    }
}