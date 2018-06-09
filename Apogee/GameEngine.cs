using Apogee.Core;
using Apogee.Gui;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Apogee
{
    public static class GameEngine
    {
        public static GameWindow Window { get; set; }

        
        public static Container Container { get; set; }

        public static void Load(string containerPath)
        {
            Window = new GameWindow(2000, 1600);
            Container = new Container(containerPath);
        }

        public static void Start()
        {
            InitOpenGL();


            Container.Update(true);

            _input = new Input(Window);

            //events
            Window.Load += (sender, args) => { };
            Window.Unload += (sender, args) => { };

            Window.Resize += (sender, args) => { GL.Viewport(0, 0, Window.Width, Window.Height); };

            Window.Closing += (sender, args) => { };
            Window.Closed += (sender, args) => { };

            Window.RenderFrame += WindowOnRenderFrame;
            Window.UpdateFrame += WindowOnUpdateFrame;


            //start
            Window.TargetRenderFrequency = 200;
            Window.TargetUpdateFrequency = 128;

            //Window.WindowState = WindowState.Fullscreen;
            ImGuiEngine.Install();
            Window.Run();
        }

        private static Input _input;

        private static void WindowOnUpdateFrame(object sender, FrameEventArgs e)
        {
            Window.Title = Window.RenderFrequency.ToString("N");
            Container.Camera.Input(_input, e.Time);
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

        private static void WindowOnRenderFrame(object sender, FrameEventArgs e)
        {
            GL.ClearColor(0.2f, 0.2f, 0.2f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            RenderTarget.BindMainWindow();
            Container.Update();

            ImGuiEngine.RenderFrame(Editor.Instance.Draw);

            Window.SwapBuffers();
        }
    }
}