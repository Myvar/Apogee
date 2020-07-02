using System;
using System.Drawing;
using OpenToolkit.Graphics.OpenGL;

namespace Apogee.Engine
{
    public class RenderTarget : IDisposable
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int FrameBuffer { get; set; }
        public int DepthBuffer { get; set; }
        public int Texture { get; set; }
        public int DepthTexture { get; set; }


        public RenderTarget(int width, int height)
        {
            Width = width;
            Height = height;

            int fb;
            GL.GenFramebuffers(1, out fb);
            FrameBuffer = fb;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FrameBuffer);

            {
                int tx;
                GL.GenTextures(1, out tx);
                Texture = tx;
            }
            {
                int tx;
                GL.GenTextures(1, out tx);
                DepthTexture = tx;
            }

            GL.BindTexture(TextureTarget.Texture2D, Texture);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb,
                Width, height, 0, PixelFormat.Rgb,
                PixelType.Float, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Linear);

            GL.BindTexture(TextureTarget.Texture2D, DepthTexture);

            GL.TexImage2D(TextureTarget.Texture2D, 0,
                PixelInternalFormat.DepthComponent32f,
                Width, height, 0, PixelFormat.DepthComponent,
                PixelType.Float, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Linear);

            int db;
            GL.GenRenderbuffers(1, out db);
            DepthBuffer = db;

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthBuffer);

            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, Width, Height);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer, DepthBuffer);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0, Texture, 0);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer
                , FramebufferAttachment.DepthAttachment, DepthTexture, 0);

            GL.DrawBuffers(1, new[] {DrawBuffersEnum.ColorAttachment0,});

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                //Terminal.Debug("Failed to create render target");
                Environment.Exit(1);
            }


            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, ApogeeEngine.Window.Size.X, ApogeeEngine.Window.Size.Y);
        }

        public void Dispose()
        {
            GL.DeleteTexture(Texture);
            GL.DeleteTexture(DepthTexture);
            GL.DeleteBuffer(DepthBuffer);
            GL.DeleteBuffer(FrameBuffer);
        }

        public void Bind()
        {
            GL.ActiveTexture(0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FrameBuffer);
            GL.Viewport(0, 0, Width, Height);
        }


        public void Apply(int samplerSlot)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + samplerSlot);
            GL.BindTexture(TextureTarget.Texture2D, Texture);
        }


        public void ApplyDepth(int samplerSlot)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + samplerSlot);
            GL.BindTexture(TextureTarget.Texture2D, DepthTexture);
            GL.Enable(EnableCap.Texture2D);
        }

        public static void BindMainWindow()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, ApogeeEngine.Window.Size.X, ApogeeEngine.Window.Size.Y);
        }

        public void Clear(Color c)
        {
            GL.ClearColor(c);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void Draw()
        {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthBuffer);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0, Texture, 0);

            BindMainWindow();

            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, FrameBuffer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);

            //  GL.Viewport(0, 0, GameEngine.Window.Width, GameEngine.Window.Height);
            GL.BlitFramebuffer(0, 0, Width, Height,
                0, 0, Width, Height,
                ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);

            // GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, ApogeeEngine.Window.Size.X, ApogeeEngine.Window.Size.Y);
        } 
    }
}