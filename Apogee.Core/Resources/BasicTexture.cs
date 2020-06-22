using OpenToolkit.Graphics.OpenGL;

namespace Apogee.Core.Resources
{
    public class BasicTexture : IAsset
    {
        public int TextureId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

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