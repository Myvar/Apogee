using System;
using System.IO;
using Apogee.Core;
using Apogee.Resources;
using ImageSharp;
using ImageSharp.Drawing.Pens;
using SixLabors.Fonts;
using SixLabors.Primitives;

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
            _shader = new Shader("./Shaders/GUI");
            _projection = GuiHelper.GetOrth(window);
            _tranform = new Transform();
            
            WindowSize = new Vector2f(window.Width, window.Height);


            using (Image<Rgba32> image = new Image<Rgba32>(100, 100))
            {
                // FontCollection fonts = new FontCollection();
                // FontFamily font = fonts.Install("./Fonts/Roboto-Regular.ttf");
                /* image
                     .Fill(Rgba32.Green)
                     .DrawLines(new Pen<Rgba32>(Rgba32.Red, 5), new PointF[] {new PointF(0, 0), new PointF(100, 100)})
                     .DrawLines(new Pen<Rgba32>(Rgba32.Blue, 5), new PointF[] {new PointF(100, 0), new PointF(0, 100)})
                    // .DrawText("Test", new Font(font, 50), Rgba32.Black, new PointF(1, 1))
                     .GaussianBlur();*/
                _texture = new Texture(Image.Load("./uv.png"));
            }
        }

        public void Draw()
        {
            GuiHelper.Start2D();
            _shader.Apply();

                DrawImage(100, 100, 100, 100, _texture);
                
            

            GuiHelper.End2D();
        }

        public void DrawImage(int x, int y, int w, int h, Texture t)
        {
            t.Apply(0);
            _tranform.Translation = new Vector3f(x, WindowSize.Y - y - h, 0);
            _tranform.Scale = new Vector3f(w, h, 0);
            

            _shader.SetUniform("Model", _tranform.GetTranformation());
            _shader.SetUniform("Proj", _projection);
            _quad = GuiHelper.NewQuad();
            _quad.Draw();
        }
    }
}