using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Apogee.Core
{
    public class RenderTarget
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int FrameBuffer { get; set; }
        public int DepthBuffer { get; set; }
        public int Texture { get; set; }

        public RenderTarget(int width, int height)
        {
            Width = width;
            Height = height;

            int fb;
            GL.GenFramebuffers(1, out fb);
            FrameBuffer = fb;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FrameBuffer);

            int tx;
            GL.GenTextures(1, out tx);
            Texture = tx;

            GL.BindTexture(TextureTarget.Texture2D, Texture);


            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Width, height, 0, PixelFormat.Rgb,
                PixelType.Byte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Nearest);

            int db;
            GL.GenRenderbuffers(1, out db);
            DepthBuffer = db;

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, Width, Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer, DepthBuffer);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, Texture, 0);

            GL.DrawBuffers(1, new[] {DrawBuffersEnum.ColorAttachment0,});

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                Terminal.Debug("Failed to create render target");
                Environment.Exit(1);
            }
        }

        public void Dispose()
        {
            GL.DeleteTexture(Texture);
            GL.DeleteBuffer(DepthBuffer);
            GL.DeleteBuffer(FrameBuffer);
        }

        public void Bind(OpenTK.Color c)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FrameBuffer);
            GL.Viewport(0, 0, Width, Height);
            GL.ClearColor(c);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void Bind()
        {
            Bind(OpenTK.Color.Black);
        }

        public void BindMainWindow()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, Width, Height);
        }

        public void Clear()
        {
            GL.ClearColor(OpenTK.Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
    }
}