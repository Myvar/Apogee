using System;
using System.Numerics;
using ImGuiNET;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace Apogee.Gui
{
    public static class ImGUIEngine
    {
        public static void Skin()
        {
            var style = ImGui.GetStyle();
            style.WindowPadding = new Vector2(10.0f, 10.0f);
            style.WindowRounding = 5.0f;
            style.ChildWindowRounding = 5.0f;
            style.FramePadding = new Vector2(5.0f, 4.0f);
            style.FrameRounding = 5.0f;
            style.ItemSpacing = new Vector2(5.0f, 5.0f);
            style.ItemInnerSpacing = new Vector2(10.0f, 10.0f);
            style.IndentSpacing = 15.0f;
            style.ScrollbarSize = 16.0f;
            style.ScrollbarRounding = 5.0f;
            style.GrabMinSize = 7.0f;
            style.GrabRounding = 2.0f;

            var color1 = new Vector4(45f / 255f, 55f / 255f, 64f / 255f, 1); // nice bluegray 
            var color2 = new Vector4(85f / 255f, 101f / 255f, 115f / 255f, 1); // light fray
            var color3 = new Vector4(36f / 255f, 36f / 255f, 36f / 255f, 1); // drawk black grayish
            var color4 = new Vector4(242f / 255f, 242f / 255f, 242f / 255f, 1); // dirst white
            var color5 = new Vector4(166f / 255f, 166f / 255f, 166f / 255f, 1); // gray

            style.SetColor(ColorTarget.Text, color4);
            //style.SetColor(ColorTarget.TextDisabled, color5);
            style.SetColor(ColorTarget.WindowBg, color1);
            style.SetColor(ColorTarget.ChildWindowBg, color2);
            style.SetColor(ColorTarget.PopupBg, color3);
            style.SetColor(ColorTarget.Border, color5);
            style.SetColor(ColorTarget.BorderShadow, color3);
            style.SetColor(ColorTarget.FrameBg, color1);
            style.SetColor(ColorTarget.FrameBgHovered, color3);
            style.SetColor(ColorTarget.FrameBgActive, color2);
            style.SetColor(ColorTarget.TitleBg, color1);
            style.SetColor(ColorTarget.TitleBgCollapsed, color2);
            style.SetColor(ColorTarget.TitleBgActive, color3);
            // style.SetColor(ColorTarget.MenuBarBg, color3);
            style.SetColor(ColorTarget.ScrollbarBg, color2);
            style.SetColor(ColorTarget.ScrollbarGrab, color3);
            style.SetColor(ColorTarget.ScrollbarGrabHovered, color4);
            style.SetColor(ColorTarget.ScrollbarGrabActive, color3);
            style.SetColor(ColorTarget.ComboBg, color2);
            style.SetColor(ColorTarget.CheckMark, color4);
            style.SetColor(ColorTarget.SliderGrab, color4);
            style.SetColor(ColorTarget.SliderGrabActive, color4);
            style.SetColor(ColorTarget.Button, color1);
            style.SetColor(ColorTarget.ButtonHovered, color2);
            style.SetColor(ColorTarget.ButtonActive, color3);
            style.SetColor(ColorTarget.Header, color3);
            style.SetColor(ColorTarget.HeaderHovered, color2);
            style.SetColor(ColorTarget.HeaderActive, color3);
            //style.SetColor(ColorTarget.Separator, new Vector4(0.00f, 0.00f, 0.00f, 1.00f));
            //style.SetColor(ColorTarget.SeparatorHovered, new Vector4(0.27f, 0.59f, 0.75f, 1.00f));
            //style.SetColor(ColorTarget.SeparatorActive, new Vector4(0.08f, 0.39f, 0.55f, 1.00f));
            style.SetColor(ColorTarget.ResizeGrip, color1);
            style.SetColor(ColorTarget.ResizeGripHovered, color2);
            style.SetColor(ColorTarget.ResizeGripActive, color3);
            style.SetColor(ColorTarget.CloseButton, color2);
            style.SetColor(ColorTarget.CloseButtonHovered, color1);
            style.SetColor(ColorTarget.CloseButtonActive, color3);
            /* style.SetColor(ColorTarget.PlotLines, new Vector4(1.00f, 1.00f, 1.00f, 1.00f));
             style.SetColor(ColorTarget.PlotLinesHovered, new Vector4(0.90f, 0.70f, 0.00f, 1.00f));
             style.SetColor(ColorTarget.PlotHistogram, new Vector4(0.90f, 0.70f, 0.00f, 1.00f));
             style.SetColor(ColorTarget.PlotHistogramHovered, new Vector4(1.00f, 0.60f, 0.00f, 1.00f));*/
            style.SetColor(ColorTarget.TextSelectedBg, color5);
            style.SetColor(ColorTarget.ModalWindowDarkening, color3);
        }

        private static System.Numerics.Vector4 RGB(int r, int g, int b)
        {
            return new System.Numerics.Vector4(r / 255f, g / 255f, b / 255f, 255 / 255f);
        }

        private static System.Numerics.Vector4 RGBA(int r, int g, int b, int a)
        {
            return new System.Numerics.Vector4(r / 255f, g / 255f, b / 255f, a / 255f);
        }

        public static void SkinEnd()
        {
            // ImGui.PopStyleColor(4);
        }

        private static int s_fontTexture;
        private static float _wheelPosition;
        private static float _scaleFactor;

        public static void Install(GameWindow w)
        {
            int desiredWidth = 600;
            w.KeyDown += new EventHandler<KeyboardKeyEventArgs>(OnKeyDown);
            w.KeyUp += new EventHandler<KeyboardKeyEventArgs>(OnKeyUp);
            w.KeyPress += new EventHandler<KeyPressEventArgs>(OnKeyPress);

            _scaleFactor = w.Width / desiredWidth;

            ImGui.GetIO().FontAtlas.AddDefaultFont();

            SetOpenTKKeyMappings();
            CreateDeviceObjects();

            Skin();
        }


        private static unsafe void SetOpenTKKeyMappings()
        {
            IO io = ImGui.GetIO();
            io.KeyMap[GuiKey.Tab] = (int) Key.Tab;
            io.KeyMap[GuiKey.LeftArrow] = (int) Key.Left;
            io.KeyMap[GuiKey.RightArrow] = (int) Key.Right;
            io.KeyMap[GuiKey.UpArrow] = (int) Key.Up;
            io.KeyMap[GuiKey.DownArrow] = (int) Key.Down;
            io.KeyMap[GuiKey.PageUp] = (int) Key.PageUp;
            io.KeyMap[GuiKey.PageDown] = (int) Key.PageDown;
            io.KeyMap[GuiKey.Home] = (int) Key.Home;
            io.KeyMap[GuiKey.End] = (int) Key.End;
            io.KeyMap[GuiKey.Delete] = (int) Key.Delete;
            io.KeyMap[GuiKey.Backspace] = (int) Key.BackSpace;
            io.KeyMap[GuiKey.Enter] = (int) Key.Enter;
            io.KeyMap[GuiKey.Escape] = (int) Key.Escape;
            io.KeyMap[GuiKey.A] = (int) Key.A;
            io.KeyMap[GuiKey.C] = (int) Key.C;
            io.KeyMap[GuiKey.V] = (int) Key.V;
            io.KeyMap[GuiKey.X] = (int) Key.X;
            io.KeyMap[GuiKey.Y] = (int) Key.Y;
            io.KeyMap[GuiKey.Z] = (int) Key.Z;
        }


        private static void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            ImGui.AddInputCharacter(e.KeyChar);
        }

        private static void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            ImGui.GetIO().KeysDown[(int) e.Key] = true;
            UpdateModifiers(e);
        }


        private static unsafe void UpdateModifiers(KeyboardKeyEventArgs e)
        {
            IO io = ImGui.GetIO();
            io.AltPressed = e.Alt;
            io.CtrlPressed = e.Control;
            io.ShiftPressed = e.Shift;
        }

        private static void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            ImGui.GetIO().KeysDown[(int) e.Key] = false;
            UpdateModifiers(e);
        }

        private static unsafe void CreateDeviceObjects()
        {
            IO io = ImGui.GetIO();

            // Build texture atlas
            FontTextureData texData = io.FontAtlas.GetTexDataAsAlpha8();

            // Create OpenGL texture
            s_fontTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, s_fontTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) All.Linear);
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Alpha,
                texData.Width,
                texData.Height,
                0,
                PixelFormat.Alpha,
                PixelType.UnsignedByte,
                new IntPtr(texData.Pixels));

            // Store the texture identifier in the ImFontAtlas substructure.
            io.FontAtlas.SetTexID(s_fontTexture);

            // Cleanup (don't clear the input data if you want to append new fonts later)
            //io.Fonts->ClearInputData();
            io.FontAtlas.ClearTexData();
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }


        public static unsafe void RenderFrame(Action e, GameEngine ge)
        {
            IO io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(ge.Window.Width, ge.Window.Height);
            io.DisplayFramebufferScale = new System.Numerics.Vector2(_scaleFactor);
            io.DeltaTime = (1f / 60f);

            UpdateImGuiInput(io, ge);

            ImGui.NewFrame();

            e();

            ImGui.Render();

            DrawData* data = ImGui.GetDrawData();
            RenderImDrawData(data, ge);
        }


        private static unsafe void UpdateImGuiInput(IO io, GameEngine ge)
        {
            MouseState cursorState = Mouse.GetCursorState();
            MouseState mouseState = Mouse.GetState();

            //if (ge.Window.mo.Contains(ge.Window.Mouse.X, ge.Window.Mouse.Y))
            {
                var windowPoint = new System.Numerics.Vector2(ge.Window.Mouse.X, ge.Window.Mouse.Y);
                io.MousePosition =
                    windowPoint; //new System.Numerics.Vector2(windowPoint.X / io.DisplayFramebufferScale.X, windowPoint.Y / io.DisplayFramebufferScale.Y);
            }
            //else
            {
                //        io.MousePosition = new System.Numerics.Vector2(-1f, -1f);
            }

            io.MouseDown[0] = mouseState.LeftButton == ButtonState.Pressed;
            io.MouseDown[1] = mouseState.RightButton == ButtonState.Pressed;
            io.MouseDown[2] = mouseState.MiddleButton == ButtonState.Pressed;

            float newWheelPos = mouseState.WheelPrecise;
            float delta = newWheelPos - _wheelPosition;
            _wheelPosition = newWheelPos;
            io.MouseWheel = delta;
        }

        private static unsafe void RenderImDrawData(DrawData* draw_data, GameEngine ge)
        {
            // Rendering
            int display_w, display_h;
            display_w = ge.Window.Width;
            display_h = ge.Window.Height;


            GL.Viewport(0, 0, display_w, display_h);

            int last_texture;
            GL.GetInteger(GetPName.TextureBinding2D, out last_texture);
            GL.PushAttrib(AttribMask.EnableBit | AttribMask.ColorBufferBit | AttribMask.TransformBit);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ScissorTest);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.Enable(EnableCap.Texture2D);

            GL.UseProgram(0);

            // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
            IO io = ImGui.GetIO();
            ImGui.ScaleClipRects(draw_data, io.DisplayFramebufferScale);

            // Setup orthographic projection matrix
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(
                0.0f,
                io.DisplaySize.X / io.DisplayFramebufferScale.X,
                io.DisplaySize.Y / io.DisplayFramebufferScale.Y,
                0.0f,
                -1.0f,
                1.0f);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            // Render command lists

            for (int n = 0; n < draw_data->CmdListsCount; n++)
            {
                NativeDrawList* cmd_list = draw_data->CmdLists[n];
                byte* vtx_buffer = (byte*) cmd_list->VtxBuffer.Data;
                ushort* idx_buffer = (ushort*) cmd_list->IdxBuffer.Data;

                DrawVert vert0 = *((DrawVert*) vtx_buffer);
                DrawVert vert1 = *(((DrawVert*) vtx_buffer) + 1);
                DrawVert vert2 = *(((DrawVert*) vtx_buffer) + 2);

                GL.VertexPointer(2, VertexPointerType.Float, sizeof(DrawVert),
                    new IntPtr(vtx_buffer + DrawVert.PosOffset));
                GL.TexCoordPointer(2, TexCoordPointerType.Float, sizeof(DrawVert),
                    new IntPtr(vtx_buffer + DrawVert.UVOffset));
                GL.ColorPointer(4, ColorPointerType.UnsignedByte, sizeof(DrawVert),
                    new IntPtr(vtx_buffer + DrawVert.ColOffset));

                for (int cmd_i = 0; cmd_i < cmd_list->CmdBuffer.Size; cmd_i++)
                {
                    DrawCmd* pcmd = &(((DrawCmd*) cmd_list->CmdBuffer.Data)[cmd_i]);
                    if (pcmd->UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        GL.BindTexture(TextureTarget.Texture2D, pcmd->TextureId.ToInt32());
                        GL.Scissor(
                            (int) pcmd->ClipRect.X,
                            (int) (io.DisplaySize.Y - pcmd->ClipRect.W),
                            (int) (pcmd->ClipRect.Z - pcmd->ClipRect.X),
                            (int) (pcmd->ClipRect.W - pcmd->ClipRect.Y));
                        ushort[] indices = new ushort[pcmd->ElemCount];
                        for (int i = 0; i < indices.Length; i++)
                        {
                            indices[i] = idx_buffer[i];
                        }
                        GL.DrawElements(PrimitiveType.Triangles, (int) pcmd->ElemCount, DrawElementsType.UnsignedShort,
                            new IntPtr(idx_buffer));
                    }
                    idx_buffer += pcmd->ElemCount;
                }
            }

            // Restore modified state
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.BindTexture(TextureTarget.Texture2D, last_texture);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.PopAttrib();
        }
    }
}