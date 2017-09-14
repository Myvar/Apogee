using System;
using System.Collections.Generic;
using System.IO;
using ImageSharp;
using ImageSharp.Formats;
using OpenTK.Graphics.OpenGL4;

namespace Apogee.Resources
{
    public class Texture : IResource
    {
        public int TextureID;
        public int Width, Height;

        public Texture()
        {
            
        }

        public static IResource LoadTexture(string file)
        {
            using (var img = Image.Load(file))
            {
                return new Texture(img);
            }
        }
        
        public Texture(Image<Rgba32> image)
        {
            var data = ColorArrayToByteArray(image);

            Width = image.Width;
            Height = image.Height;


            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            TextureID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, TextureID);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, data);

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
            GL.DeleteTexture(TextureID);
        }


        private byte[] ColorArrayToByteArray(Image<Rgba32> c)
        {
              return c.Pixels.AsBytes().ToArray();
        }

        public void Apply(int samplerSlot)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + samplerSlot);
            GL.BindTexture(TextureTarget.Texture2D, TextureID);
        }
    }
}