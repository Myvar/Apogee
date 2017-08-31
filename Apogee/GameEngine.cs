﻿using System;
using System.IO;
using System.Threading;
using Apogee.API;
using Apogee.Engine.Core;
using Apogee.Gui;
using Apogee.Resources;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Apogee
{
    public class GameEngine
    {
        public string GameDirectory { get; set; }
        public Assets Assets { get; set; }
        public GameWindow Window { get; set; }
        public Input Input { get; set; }
        public GuiEngine GuiEngine { get; set; }


        /// <summary>
        /// Init the GameEngine
        /// </summary>
        /// <param name="gd">The Directory containing the game data</param>
        public GameEngine(string gd)
        {
            GameDirectory = Path.GetFullPath(gd);

            if (!Directory.Exists(gd))
            {
                Terminal.Debug("Directory does not exist: " + GameDirectory);
                return;
            }
            
            //init
            Terminal.Debug($"GameDirectory: {GameDirectory}");
            Terminal.Debug($"WorkingDirectory: {Directory.GetCurrentDirectory()}");

            Terminal.Debug($"Loading Assets");
            Assets = new Assets(this);

            Terminal.Debug($"Loading Window");
            Window = new GameWindow(800, 600);
           
            Terminal.Debug(GL.GetString(StringName.Version));
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

        private Scene MainScene;

        /// <summary>
        /// Use this Method to start the game
        /// </summary>
        /// <returns></returns>
        public GameEngine Start()
        {
            Terminal.Log("Starting Game");

            //testing
            MainScene = Assets.Load<Scene>("mainscene.cs");
            var en = this;
            MainScene.LoadGameEngine(ref en);


            Window.Load += OnWindowOnLoad;
            Window.RenderFrame += OnRender;
            Window.UpdateFrame += OnUpdate;
            
            
            Input = new Input(Window);
            

            InitOpenGL();
            ImGUIEngine.Install(Window);
            GuiEngine.Init(Window);
            ApiLoader.Load(this);

            GuiEngine = new GuiEngine();

            Window.TargetRenderFrequency = 400;
            Window.VSync = VSyncMode.Off;
            Window.Title = "Apogee Engine: --- FPS";
            Window.Run(60);

            return this;
        }

        private void OnWindowOnLoad(object sender, EventArgs args)
        {
            MainScene.Load();
        }

        public void Clear(Color c)
        {
            GL.ClearColor(c);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void OnRender(object sender, FrameEventArgs fea)
        {
            Clear(Color.DarkCyan);

            //Render
            MainScene.Draw();

            //Gui
            //GuiEngine.Draw(MainScene.UI);
            if (WireFrame) GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            ImGUIEngine.RenderFrame(MainScene.UI, this);
            if (WireFrame) GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            Window.SwapBuffers();
        }

        private bool WireFrame = false;
        
        public void OnUpdate(object sender, FrameEventArgs fea)
        {
            if (Input.IsKeyDown(Key.Z))
            {
                if (WireFrame)
                {
                    GL.Enable(EnableCap.CullFace);
                    GL.Enable(EnableCap.DepthTest);
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                }
                else
                {
                    GL.Disable(EnableCap.CullFace);
                    GL.Disable(EnableCap.DepthTest);
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                }

                WireFrame = !WireFrame;

                Thread.Sleep(50);
            }
            
            Window.Title = "Apogee Engine: " + Math.Round(Window.RenderFrequency) + " FPS";

            MainScene.Update(fea.Time);

            GL.Viewport(0, 0, Window.Size.Width, Window.Size.Height);
        }
    }
}