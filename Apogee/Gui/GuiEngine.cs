using System;
using System.IO;
using Apogee.Core;
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


        public static void Init(OpenTK.GameWindow window)
        {
            
            _shader = new Shader("./Shaders/GUI");
            _projection = GuiHelper.GetOrth(window);
            _tranform = new Transform();

            WindowSize = new Vector2f(window.Width, window.Height);


          
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