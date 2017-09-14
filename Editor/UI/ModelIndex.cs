using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Apogee.API;
using Apogee.Core;
using Apogee.Resources;
using ImGuiNET;

namespace Editor.UI
{
    public class ModelIndex : Window
    {
        private Texture _texture;
        public Camera Camera;
        public Shader BasicShader;

        public ModelIndex()
        {
            Visible = true;

            Camera = new Camera(0.1f, 1000, 128, 128, 70, 10);
            BasicShader = new Shader("./Shaders/basic");
        }

        private List<string> Models { get; set; } = new List<string>();

        public void IndexData(Scene s)
        {
            if (Models.Count > 0) return;
            _texture = s.Engine.Assets.Load<Texture>("Textures/dev-1.jpg");

            Models = s.Engine.Assets.IndexModels();
        }

        private Dictionary<string, Model> ModelCache { get; set; } = new Dictionary<string, Model>();

        private Model LoadCached(string file)
        {
            if (ModelCache.ContainsKey(file))
            {
                return ModelCache[file];
            }

            var model = Model.LoadModel(file) as Model;
            ModelCache.Add(file, model);

            return model;
        }

        private Dictionary<string, RenderTarget> ThumnailCache { get; set; } = new Dictionary<string, RenderTarget>();
        private Dictionary<string, WorldObject> ThumnailWorldObjCache { get; set; } = new Dictionary<string, WorldObject>();

        private int RenderThumnail(string file)
        {


            if (!ThumnailCache.ContainsKey(file))
            {
                ThumnailCache.Add(file, new RenderTarget(128, 128));
            }

            var m = LoadCached(file);

            var rt = ThumnailCache[file];
            rt.Bind(OpenTK.Color.Gray);


            if (!ThumnailWorldObjCache.ContainsKey(file))
            {
                ThumnailWorldObjCache.Add(file, new WorldObject(m, _texture));
            }
            
            var obj = ThumnailWorldObjCache[file];
            obj.Transform.Z = 5;
            
            obj.Transform.RotY += 0.1f;
            
            obj.Draw(BasicShader, Camera);


            rt.BindMainWindow();

            return rt.Texture;
        }

        public override void Draw(Scene s)
        {
            IndexData(s);

            ImGui.BeginWindow("ModelIndex");

            //list
            uint c = 0;
            int l = 0;
            foreach (var model in Models)
            {
                
                var x = RenderThumnail(model);
                var fl = new FileInfo(model);
                //ImGui.Button(fl.Name);

                if (l < 6)
                {
                    ImGui.SameLine();
                }
                else
                {
                    l = 0;
                }

                ImGui.BeginChildFrame(c++, new Vector2(128, 128) + new Vector2(0, 50), WindowFlags.Default);
                
                ImGui.Text(fl.Name);
                
                ImGui.Image(new IntPtr(x), new Vector2(128, 128), new Vector2(0, 1), new Vector2(1, 0),
                    new Vector4(1, 1, 1, 1),
                    new Vector4(0, 0, 0, 0));


                if (ImGui.Button("Apply"))
                {
                    ObjectEditor.CurrentWorldObject.Model = LoadCached(model);
                }
                
                ImGui.EndChildFrame();
                l++;
            }
            ImGui.NextColumn();

            //display


            ImGui.EndWindow();
        }
    }
}