using System;
using ImageSharp;
using ImageSharp.PixelFormats;
using OpenTK.Graphics.OpenGL4;

namespace Apogee.Resources
{
    public class TerainMap
    {
        public int TextureID;
        public int Width, Height;

        public TerainMap(Image<Rgba64> image)
        {
            var data = ColorArrayToByteArray(image);

            Width = image.Width;
            Height = image.Height;

            TextureID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, TextureID);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16 , Width, Height, 0,
                PixelFormat.Rgba, PixelType.Float, data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Linear);
        }

        public void Dispose()
        {
            GL.DeleteTexture(TextureID);
        }


        private byte[] ColorArrayToByteArray(Image<Rgba64> c)
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