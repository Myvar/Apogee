using System;
using Apogee.Engine;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Apogee
{
    public class GameEngine : IDisposable
    {
        public static Sdl2Window Window;
        public static GraphicsDevice GraphicsDevice;

        private AGame _game;


        public GameEngine(AGame game)
        {
            _game = game;
            var windowCI = new WindowCreateInfo()
            {
                X = 100,
                Y = 100,
                WindowWidth = 960,
                WindowHeight = 540,
                WindowTitle = "Apogee"
            };
            Window = VeldridStartup.CreateWindow(ref windowCI);
            Window.Resized += () =>
                GraphicsDevice.MainSwapchain.Resize((uint) Window.Width, (uint) Window.Height);

            Window.KeyDown += e =>
            {
                if (e.Key == Key.Escape)
                {
                    Stop();
                }
            };

            var options = new GraphicsDeviceOptions(
                debug: false,
                swapchainDepthFormat: PixelFormat.R16_UNorm,
                syncToVerticalBlank: true,
                resourceBindingModel: ResourceBindingModel.Improved,
                preferDepthRangeZeroToOne: true,
                preferStandardClipSpaceYDirection: true);
            GraphicsDevice = VeldridStartup.CreateGraphicsDevice(Window, options, GraphicsBackend.OpenGL);
        }

        public void Stop()
        {
            _game.CleanUp();
            GraphicsDevice.Dispose();

            Environment.Exit(0);
        }

        public void Start()
        {
            _game.Load();

            while (Window.Exists)
            {
                Window.PumpEvents();

                if (Window.Exists)
                {
                    Draw();
                }
            }

            Stop();
        }

        private void Draw()
        {
            _game.Update();
            _game.Render();
            GraphicsDevice.SwapBuffers();
        }

        public void Dispose()
        {
            _game.CleanUp();
            GraphicsDevice.Dispose();
        }
    }
}