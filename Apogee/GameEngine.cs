using System;
using System.Diagnostics;
using System.IO;
using Apogee.Core;
using Apogee.Engine.Core;
using Apogee.Resources;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Apogee
{
    public class GameEngine
    {
        public string GameDirectory { get; set; }
        public Assets Assets { get; set; }
        public GameWindow Window { get; set; }
        public Input Input { get; set; }
        
        
        /// <summary>
        /// Init the GameEngine
        /// </summary>
        /// <param name="gd">The Directory containing the game data</param>
        public GameEngine(string gd)
        {
            GameDirectory = Path.GetFullPath(gd);

            //init
            Terminal.Debug($"GameDirectory: {GameDirectory}");
            Terminal.Debug($"WorkingDirectory: {Directory.GetCurrentDirectory()}");

            Assets = new Assets(this);
            
            Window = new GameWindow();
        }
        
        private static void InitOpenGL()
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

            //culling
            GL.FrontFace(FrontFaceDirection.Cw);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);

            //Depth
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthClamp);

            GL.Enable(EnableCap.Texture2D);
        }

        
        Camera Camera;

        private Model m;
        Transform Transform;
        Shader BasicShader;
        
        /// <summary>
        /// Use this Method to start the game
        /// </summary>
        /// <returns></returns>
        public GameEngine Start()
        {
            Terminal.Log("Starting Game");
            Camera = new Camera(0.1f, 1000, Window.Width, Window.Height, 70, 10);
            
            //testing
            //var s = Assets.Load<Scene>("mainscene.cs");
            //s.Assets = Assets;
            //s.Update();
            BasicShader = new Shader("./Shaders/basic");
            Transform = new Transform();
            
            
            Window.Load += (sender, args) =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                m = Assets.Load<Model>("monkey.ogex");
                sw.Stop();
                Terminal.Log(sw.Elapsed.ToString());
            };
            Window.RenderFrame += new EventHandler<FrameEventArgs>(OnRender);
            Window.UpdateFrame += new EventHandler<FrameEventArgs>(OnUpdate);
           
            Input = new Input(Window);
            
            
            InitOpenGL();

            Window.TargetRenderFrequency = 400;
            Window.VSync = VSyncMode.Off;
            Window.Title = "Apogee Engine: --- FPS";
            Window.Run(60);

            return this;
        }
        
        public void Clear(Color c)
        {
            GL.ClearColor(c);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void OnRender(object sender, FrameEventArgs fea)
        {
            Clear(Color.White);

            //Render
            //Game.Render();
            
            BasicShader.Apply();
            BasicShader.Update(Transform.GetTranformation(), Camera.GetProjection() * Transform.GetTranformation());
            BasicShader.SetUniform("viewPos", Camera.Position);
            
            m.Draw();

            //Gui
            //Game.Gui();

            Window.SwapBuffers();
        }
        
        
        public void OnUpdate(object sender, FrameEventArgs fea)
        {
            Window.Title = "Apogee Engine: " + Math.Round(Window.RenderFrequency) + " FPS";

            Camera.Input(Input, fea.Time);
        }
    }
}