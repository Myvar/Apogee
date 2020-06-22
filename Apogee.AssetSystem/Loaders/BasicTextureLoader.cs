using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Apogee.Core;
using Apogee.Core.Resources;
using OpenToolkit.Graphics.OpenGL;

namespace Apogee.AssetSystem.Loaders
{
    public class BasicTextureLoader : IAssetLoader
    {
        public IAsset LoadFromFile(RawAssetSource source)
        {
            var re = new BasicTexture();
            using (var image = (Bitmap) Image.FromStream(File.OpenRead(source.File)))
            {
                var data = new List<byte>();

                re.Width = image.Width;
                re.Height = image.Height;

                for (var y = 0; y < re.Height; y++)
                {
                    for (var x = 0; x < re.Width; x++)
                    {
                        var px = image.GetPixel(x, y);
                        data.Add(px.R);
                        data.Add(px.G);
                        data.Add(px.B);
                        data.Add(px.A);
                    }
                }


                GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

                re.TextureId = GL.GenTexture();

                GL.BindTexture(TextureTarget.Texture2D, re.TextureId);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, re.Width, re.Height, 0,
                    PixelFormat.Rgba, PixelType.UnsignedByte, data.ToArray());

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                    (int) TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                    (int) TextureMagFilter.Linear);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, -1);
            }

            return re;
        }
    }
}