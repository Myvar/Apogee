using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Apogee.Components;
using Apogee.Core;
using Apogee.Systems;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;

namespace Apogee.UI
{
    public class AssetManager : Window
    {
        public Camera Camera;
        public static Dictionary<string, RenderTarget> ThumnailCache { get; set; } = new Dictionary<string, RenderTarget>();
        private Shader _shader = new Shader();

        private Transform _transform = new Transform()
        {
            Translation = new Vec3(0, 0, 4),
            Rotation = new Vec3(90, 45, 180)
        };

        private Texture _texture = new Texture();

        private List<string> Extentions = new List<string>()
        {
            ".ogex",
            ".png",
            ".jpg",
            ".bmp",
            ".gif"
        };

        public static Guid SelectedEntity = Guid.Empty;


        public AssetManager()
        {
            Camera = new Camera(0.1f, 1000, 128, 128, 70, 10);
            _shader.Load(Path.Combine(Container.AssetsPath, "basic"));
            using (var img = Image.Load(Path.Combine(Container.AssetsPath, "dev-orange.jpg")))
            {
                _texture.Load(img);
            }

            //_target = ;
        }

        public override void Draw()
        {
            ImGui.BeginWindow("Assets", ref Visible, WindowFlags.Default);

            int x = 0;
            var w = (int) ImGui.GetContentRegionAvailableWidth();
            int disiredWidth = w / 128;

            if (w % 128 == 0)
            {
                disiredWidth--;
            }

            var files = Directory.GetFiles(Container.AssetsPath);
            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var info = new FileInfo(file);

                if (!Extentions.Contains(info.Extension)) continue;

                if (x < disiredWidth)
                {
                    ImGui.SameLine(0, 2);
                }
                else
                {
                    x = 0;
                }

                ImGui.BeginChildFrame((uint) i, new Vector2(128, 128) + new Vector2(0, 52),
                    WindowFlags.Default);

                ImGui.Text(info.Name);
                switch (info.Extension)
                {
                    case ".ogex":
                        var model = new Model() {File = file};
                        ((Models) GameEngine.Container.Systems.First(
                                system => system.GetType() == typeof(Models)))
                            .LoadModel(model);

                        if (!ThumnailCache.ContainsKey(file))
                        {
                            ThumnailCache.Add(file, new RenderTarget(128, 128));
                        }

                        var target = ThumnailCache[file];

                        target.Bind(OpenTK.Color.Gray);

                        GL.Disable(EnableCap.CullFace);

                        _transform.Rotation.Y += 0.01f;
                        _transform.Rotation.X += 0.01f;
                        var mvp = _transform.GetTranformation();

                        _texture.Apply(0);

                        _shader.Apply();
                        _shader.Update(mvp, Camera.GetProjection() * mvp);
                        _shader.SetUniform("viewPos", Container.Camera.Position);

                        model.Draw();

                        GL.Enable(EnableCap.CullFace);
                        RenderTarget.BindMainWindow();

                        ImGui.Image(new IntPtr(target.Texture), new Vector2(128, 128), new Vector2(0, 1),
                            new Vector2(1, 0),
                            new Vector4(1, 1, 1, 1),
                            new Vector4(0, 0, 0, 0));

                        if (ImGui.Button("Apply"))
                        {
                            if (SelectedEntity != Guid.Empty)
                            {
                                var comp = GameEngine.Container.GetComponent<Model>(SelectedEntity);
                                comp.Vbo = model.Vbo;
                                comp.Ibos = model.Ibos;
                                comp.Size = model.Size;
                                comp.File = model.File;
                            }
                        }

                        break;
                    case ".png":
                    case ".jpg":
                    case ".bitmap":
                    case ".gif":
                        var texture = new Texture() {File = file};
                        ((Textures) GameEngine.Container.Systems.First(
                                system => system.GetType() == typeof(Textures)))
                            .LoadImage(texture);

                        ImGui.Image(new IntPtr(texture.TextureId), new Vector2(128, 128),
                            new Vector2(1, 0),
                            new Vector2(0, 1),
                            new Vector4(1, 1, 1, 1),
                            new Vector4(0, 0, 0, 0));
                        if (ImGui.Button("Apply"))
                        {
                            if (SelectedEntity != Guid.Empty)
                            {
                                var text = GameEngine.Container.GetComponent<Texture>(SelectedEntity);
                                text.TextureId = texture.TextureId;
                                text.Width = texture.Width;
                                text.Height = texture.Height;
                                text.File = texture.File;
                            }
                        }

                        break;
                }

                ImGui.EndChildFrame();
                x++;
            }

            ImGui.NextColumn();


            ImGui.EndWindow();
        }
    }
}