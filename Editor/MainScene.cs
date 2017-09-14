using System;
using System.Collections.Generic;
using Apogee;
using Apogee.API;
using Apogee.Core;
using Apogee.Resources;
using Editor.UI;
using ImGuiNET;
using OpenTK;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace Editor
{
    [EntryScene]
    public class MainScene : Scene
    {
        //3D
        public Camera Camera;

        public WorldObject Monkey;
        public Shader BasicShader;
        public RenderTarget MainView;
          
        
        
        public override void Load()
        {
            Camera = new Camera(0.1f, 1000, Engine.Window.Width, Engine.Window.Height, 70, 10);
            BasicShader = new Shader("./Shaders/basic");
            Monkey = new WorldObject(Engine.Assets.Load<Model>("m4a1.ogex"), Engine.Assets.Load<Texture>("Textures/dev-1.jpg"));
           // Monkey.Transform.RotX = 90;
           // Monkey.Transform.RotZ = 180;
            Monkey.Transform.Z = 3;
            MainView = new RenderTarget(800, 600);
            Engine.Window.WindowState = WindowState.Maximized;
            ObjectEditor.CurrentWorldObject = Monkey;
        }

        public override void Update(double deltaTime)
        {
            Camera.Input(Engine.Input, deltaTime);
        }

        public override void UI()
        {
            foreach (var window in Window.Windows)
            {
                if(!window.Visible) continue; 
                
                window.Draw(this);
            }
        }

        public override void Draw()
        {
            MainView.Bind();

            Monkey.Draw(BasicShader, Camera);

            MainView.BindMainWindow();
        }
    }
}