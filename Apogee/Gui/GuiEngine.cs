using System;
using System.IO;
using Apogee.Core;
using Apogee.Gui.UI;
using Apogee.Gui.UI.Controls;
using Apogee.Resources;
using ImageSharp;
using ImageSharp.Drawing;
using ImageSharp.Drawing.Brushes;
using ImageSharp.Drawing.Pens;
using ImageSharp.Processing;
using SixLabors.Fonts;
using SixLabors.Primitives;
using SixLabors.Shapes;

namespace Apogee.Gui
{
    public class GuiEngine
    {
        private static Shader _shader { get; set; }
        private static Matrix4f _projection { get; set; }
        private static Transform _tranform { get; set; }
        private static Model _quad { get; set; }
        private static Vector2f WindowSize { get; set; }

        private static Texture _texture { get; set; }

        public static void Init(OpenTK.GameWindow window)
        {
            Fonts.Init();
            
            _shader = new Shader("./Shaders/GUI");
            _projection = GuiHelper.GetOrth(window);
            _tranform = new Transform();

            WindowSize = new Vector2f(window.Width, window.Height);


            using (Image<Rgba32> image = new Image<Rgba32>(500, 500))
            {
                FontCollection fonts = new FontCollection();
                FontFamily font = fonts.Install("./Fonts/Roboto-Regular.ttf");

                var t = new TextGraphicsOptions();
                t.Antialias = true;
                
                image
                    .Fill(Rgba32.Transparent)
                    .DrawText("Test", new Font(font, 5), Rgba32.Black, new PointF(1, 1), t)
                    .DrawText("Test", new Font(font, 10), Rgba32.Black, new PointF(1, 10), t)
                    .DrawText("Test", new Font(font, 20), Rgba32.Black, new PointF(1, 20), t)
                    .DrawText("Test", new Font(font, 30), Rgba32.Black, new PointF(1, 40), t)
                    .DrawText("Test", new Font(font, 40), Rgba32.Black, new PointF(1, 70), t)
                    .DrawText("Test", new Font(font, 50), Rgba32.Black, new PointF(1, 110), t)
                    .DrawText("Test", new Font(font, 60), Rgba32.Black, new PointF(1, 170), t)
                    .DrawText("Test", new Font(font, 100), Rgba32.Black, new PointF(1, 250), t)
                    
                    .RotateFlip(RotateType.Rotate180, FlipType.Horizontal);
                _texture = new Texture(image);
            }
        }

  
        
        public void Draw(Action toDraw)
        {
            GuiHelper.Start2D();
            _shader.Apply();

            toDraw();

            GuiHelper.End2D();
        }

        public static void DrawImage(int x, int y, int w, int h, Texture t)
        {
            t.Apply(0);
            _tranform.Translation = new Vector3f(x, WindowSize.Y - y - h, 0);
            _tranform.Scale = new Vector3f(w, h, 0);


            _shader.SetUniform("Model", _tranform.GetTranformation());
            _shader.SetUniform("Proj", _projection);
            _quad = GuiHelper.NewQuad();
            _quad.Draw();
        }
        
        public static void DrawImage(int x, int y, int w, int h)
        {
            _tranform.Translation = new Vector3f(x, WindowSize.Y - y - h, 0);
            _tranform.Scale = new Vector3f(w, h, 0);


            _shader.SetUniform("Model", _tranform.GetTranformation());
            _shader.SetUniform("Proj", _projection);
            _quad = GuiHelper.NewQuad();
            _quad.Draw();
        }
    }
}