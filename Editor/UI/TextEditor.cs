using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Apogee.Resources;
using ImGuiNET;

namespace Editor.UI
{
    public unsafe class TextEditor : Window
    {
        public IntPtr Buffer;

        public TextEditor()
        {
            Buffer = Marshal.AllocHGlobal(1024 * 8);

            var ptr = (byte*) Buffer.ToPointer();
            for (int i = 0; i < 1024 * 8; i++)
            {
                ptr[i] = 0;
            }
        }

        public override void Draw(Scene s)
        {
            if (ImGui.BeginWindow("Text Editor"))
            {

                ImGui.InputTextMultiline("##Source", Buffer, 1024 * 8, new Vector2(800, 600),
                    InputTextFlags.CallbackAlways, Callback);


                ImGui.EndWindow();
            }
        }

        private int Callback(TextEditCallbackData* data)
        {

            return 0;
        }
    }
}