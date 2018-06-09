using System.Collections.Generic;
using System.IO;
using System.Threading;
using Apogee.Components;
using SixLabors.ImageSharp;

namespace Apogee.Systems
{
    public class Textures : Ecs.System
    {
        public Dictionary<string, Texture> Hashes { get; set; } = new Dictionary<string, Texture>();


        public void LoadImage(Texture texture)
        {
            if (Hashes.ContainsKey(texture.File))
            {
                texture.Width = Hashes[texture.File].Width;
                texture.Height = Hashes[texture.File].Height;
                texture.TextureId = Hashes[texture.File].TextureId;
                texture.File = texture.File;
            }
            else
            {
                using (var img = Image.Load(Path.Combine(Container.AssetsPath, texture.File)))
                {
                    texture.Load(img);
                    Hashes.Add(texture.File, texture);
                }
            }
        }

        public Textures()
        {
            RunOnce = true;
        }
    }
}