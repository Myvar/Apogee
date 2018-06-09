using System;
using System.Collections.Generic;
using System.Linq;
using Apogee.Ecs;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Apogee.Components
{
    public class Texture : Component
    {
        [JsonIgnore]
        public int TextureId;
        [JsonIgnore]
        public int Width, Height;
        
        public string File { get; set; }

        public void Load(Image<Rgba32> image)
        {
            var data = new List<byte>();

            Width = image.Width;
            Height = image.Height;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var px = image[x, y];
                    data.AddRange(BitConverter.GetBytes(px.Rgba));
                }
            }


            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            TextureId = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, TextureId);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, data.ToArray());

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Linear);

/*
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)0x2703);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, -1);*/
        }

        public void Dispose()
        {
            GL.DeleteTexture(TextureId);
        }


        public void Apply(int samplerSlot)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + samplerSlot);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
        }
    }
}