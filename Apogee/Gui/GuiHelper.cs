using System.Collections.Generic;
using Apogee.Core;
using Apogee.Resources;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Apogee.Gui
{
    public static class GuiHelper
    {
        public static void Start2D()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Disable(EnableCap.DepthTest);
            GL.Color4(1.0f, 1.0f, 1.0f, 1);
            GL.Disable(EnableCap.CullFace);
        }

        public static void End2D()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.ColorMaterial);
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.CullFace);
        }

        public static Model NewQuad()
        {
            var quad = new Model();
            quad.Load(
                new Vertex[]
                {
                    new Vertex(new Vector3f(0,0,0), new Vector2f(0, 0)),
                    new Vertex(new Vector3f(1,0,0), new Vector2f(1, 0)),
                    new Vertex(new Vector3f(1,1,0), new Vector2f(1, 1)),
                    new Vertex(new Vector3f(0,1,0), new Vector2f(0, 1)),
                    
                }, new List<List<uint>>()
                {
                    new List<uint>()
                    {
                       3,2,0,
                       2,1,0
                    }
                });
            
            return quad;
        }

        public static Matrix4f GetOrth(GameWindow Window)
        {
            return new Matrix4f().InitOrthographic(0, Window.Width, 0, Window.Height, -1, 1);
        }
        
        public static void Tranform(int x, int y, int w, int h, Shader s , GameWindow window)
        {
           
        }
    }
}