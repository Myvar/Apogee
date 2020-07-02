using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Apogee.AssetSystem;
using Apogee.AssetSystem.Loaders;
using Apogee.Core;
using Apogee.Core.Math;
using Apogee.Core.Resources;
using OpenToolkit.Graphics.OpenGL;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Desktop;

namespace Apogee.Engine
{
    public static class ApogeeEngine
    {
        public static GameWindow Window { get; set; }

        private static BasicShader _rectShader;
        private static Matrix4F _orthMat;
        private static Matrix4F _prespectiveMat;
        private static BasicMesh _quad;
        private static BasicMesh _mesh;
        private static MultisampleRenderTarget _target;

        static ApogeeEngine()
        {
            var gws = new GameWindowSettings();

            gws.RenderFrequency = 120;
            gws.UpdateFrequency = 120;

            var nws = new NativeWindowSettings();

            Window = new GameWindow(gws, nws);
            Window.Title = "Apogee";

            Window.VSync = VSyncMode.Off;

            Window.RenderFrame += WindowOnRenderFrame;
            Window.Load += OnLoad;
            Window.Resize += OnResize;
        }

        private static BasicTexture _texture;

        public static void OnLoad()
        {
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);


            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);
            GL.Enable(EnableCap.DepthClamp);
            GL.Enable(EnableCap.Multisample);

            _texture = (BasicTexture) new BasicTextureLoader().LoadFromFile(
                new RawAssetSource()
                {
                    File = "/home/myvar/GitHub/Apogee/Apogee/Assets/DevTexture/uv_1.png"
                });

            _mesh = (BasicMesh) new BasicMeshLoaderGLTF().LoadFromFile(
                new RawAssetSource()
                {
                    File = "/home/myvar/GitHub/Apogee/Apogee/Assets/simple_man.glb"
                });
            _mesh.Animator?.DoAnimation(_mesh.Animation);
            _rectShader = new BasicShader(File.ReadAllText("./Shaders/rect.glsl"));
            _quad = new BasicMesh();
            _quad.Load(
                new List<Vertex>
                {
                    new Vertex(new Vector3F(0, 0, 0), new Vector2F(0, 0)),
                    new Vertex(new Vector3F(1, 0, 0), new Vector2F(1, 0)),
                    new Vertex(new Vector3F(1, 1, 0), new Vector2F(1, 1)),
                    new Vertex(new Vector3F(0, 1, 0), new Vector2F(0, 1)),
                }, new List<List<uint>>()
                {
                    new List<uint>()
                    {
                        0, 1, 2,
                        2, 3, 0
                    },
                });

            var orth = new Matrix4F().InitIdentity().InitOrthographic(0, Window.Size.X, Window.Size.Y, 0, -1, 1);

            _orthMat = orth;

            _prespectiveMat = new Matrix4F().InitIdentity()
                .InitProjection(70f, Window.Size.X, Window.Size.Y, 0.01f, 1000f);

            _target = new MultisampleRenderTarget(Window.Size.X, Window.Size.Y);


            using (var img = new Bitmap(512, 512))
            using (var g = Graphics.FromImage(img))
            {
                g.Clear(Color.White);

                var ColourValues =
                    new string[]
                    {
                        "FF0000", "00FF00", "0000FF", "FFFF00", "FF00FF", "00FFFF", "000000",
                        "800000", "008000", "000080", "808000", "800080", "008080", "808080",
                        "C00000", "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0",
                        "400000", "004000", "000040", "404000", "400040", "004040", "404040",
                        "200000", "002000", "000020", "202000", "200020", "002020", "202020",
                        "600000", "006000", "000060", "606000", "600060", "006060", "606060",
                        "A00000", "00A000", "0000A0", "A0A000", "A000A0", "00A0A0", "A0A0A0",
                        "E00000", "00E000", "0000E0", "E0E000", "E000E0", "00E0E0", "E0E0E0",
                    };


                int y = 5;
                int x = 5;
                for (var i = 0; i < _mesh.JointsNames.Count; i++)
                {
                    var name = _mesh.JointsNames[i];
                    var cc = Color.FromArgb(
                        int.Parse(
                            ColourValues[i + 1],
                            System.Globalization.NumberStyles.HexNumber));

                    var c = Color.FromArgb(255, cc);

                    using (var b = new SolidBrush(c))
                    {
                        g.FillRectangle(b, x, y, 15, 15);
                        g.DrawString("[" + i + "]" + name, SystemFonts.DefaultFont, Brushes.Black, x + 20, y);
                    }

                    y += 20;

                    if (i > 20 && x == 5)
                    {
                        y = 5;
                        x = img.Width / 2;
                    }
                }

                img.Save("test.png");
            }
            
             //_mesh.Animator.Update((float) 0.01f);
        }


        public static void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);

            var orth = new Matrix4F().InitIdentity().InitOrthographic(0, Window.Size.X, Window.Size.Y, 0, -1, 1);

            _orthMat = orth;

            _prespectiveMat = new Matrix4F().InitIdentity()
                .InitProjection(70f, Window.Size.X, Window.Size.Y, 0.01f, 1000f);
            _target.Dispose();
            _target = new MultisampleRenderTarget(Window.Size.X, Window.Size.Y);
        }

        private static float _timmer = 0;

        private static void WindowOnRenderFrame(FrameEventArgs e)
        {
            _target.Bind();
            GL.ClearColor(0, 0.8f, 0.8f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            /*_texture.Apply(0);

            _rectShader.Apply();

            var trans = new Matrix4F().InitIdentity().InitTranslation(10, 10, 0);
            var scale = new Matrix4F().InitIdentity().InitScale(512, 512, 0);

            _rectShader.SetUniform("mvp", _orthMat * trans * scale);
            _quad.Draw();*/

            _texture.Apply(0);
            _rectShader.Apply();
            var trans = new Transform();
            trans.Translation.Z = 2;
            trans.Translation.Y = -1;

            // trans.Scale = new Vector3F(0.5f);

           // trans.Rotation.Y = 180;
            trans.Rotation.Y = _timmer;
            //  trans.Rotation.Z = _timmer;
            //  trans.Rotation.X = _timmer;


            if (_mesh.JointCount > 0)
            {
                _mesh.Animator.Update((float) e.Time);

                var t = _mesh.GetJointTransforms();

                for (int i = 0; i < t.Length; i++)
                {
                    _rectShader.SetUniform($"jointTransforms[{i}]", t[i]);
                }
            }

            _timmer += 0.5f;

            _rectShader.SetUniform("mvp", _prespectiveMat * trans.GetTranformation());

            _mesh.Draw();

            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);

            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            _target.Blit(Window.Size.X, Window.Size.Y);
            Window.SwapBuffers();
        }

        public static void Run()
        {
            Window.Run();
        }
    }
}