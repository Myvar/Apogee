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
        private RenderTarget _renderTarget = new RenderTarget(128, 128);
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
            if(Models.Count > 0) return;
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

        private Dictionary<string, int> ThumnailCache { get; set; } = new Dictionary<string, int>();

        private int RenderThumnail(string file)
        {
            if (ThumnailCache.ContainsKey(file)) return ThumnailCache[file];
            
            var m = LoadCached(file);

            _renderTarget.Clear();
            
            _renderTarget.Bind();

            var obj = new WorldObject(m, _texture);
            obj.Transform.Z = 3;
            
            obj.Draw(BasicShader, Camera);
            
            
            _renderTarget.BindMainWindow();
            
            ThumnailCache.Add(file, _renderTarget.Texture);
            return _renderTarget.Texture;
        }
        
        public override void Draw(Scene s)
        {
            IndexData(s);
            
            ImGui.BeginWindow("ModelIndex");
            
            //list

            foreach (var model in Models)
            {
                var x = RenderThumnail(model);
                var fl = new FileInfo(model);
                //ImGui.Button(fl.Name);
                
                ImGui.Image(new IntPtr(x), new Vector2(128, 128), new Vector2(0, 1), new Vector2(1, 0),
                    new Vector4(1, 1, 1, 1),
                    new Vector4(0, 0, 0, 0));
            }
            ImGui.NextColumn();
            
            //display
            
            
            ImGui.EndWindow();

        }
    }
}