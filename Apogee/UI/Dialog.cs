using System;
using System.IO;
using System.Text;
using ImGuiNET;

namespace Apogee.UI
{
    public static class Dialog
    {
        private static bool filePopUp = false;
        private static bool textPopUp = false;
        private static Action<string> fileSelectorcallback;

        private static byte[] s = new byte[1024];


        public static void FileSelector(Action<string> x)
        {
            filePopUp = true;
            fileSelectorcallback = x;
        }

        public static void TextDialog(Action<string> x)
        {
            textPopUp = true;
            fileSelectorcallback = x;
        }


        public unsafe static void Render()
        {
            if (filePopUp)
            {
                ImGui.BeginWindow("File Selector");

                foreach (var file in Directory.GetFiles(Container.AssetsPath))
                {
                    if (ImGui.Button(file.Remove(0, Container.AssetsPath.Length)))
                    {
                        filePopUp = false;
                        fileSelectorcallback(file);
                    }
                }

                ImGui.EndWindow();
            }

            if (textPopUp)
            {
                ImGui.BeginWindow("Input Dialog");


                ImGui.InputText("Input: ", s, (uint) s.Length, InputTextFlags.Default,
                    data => 0);

                if (ImGui.Button("OK"))
                {
                    textPopUp = false;
                    fileSelectorcallback(Encoding.UTF8.GetString(s).Replace("\0", "").Trim().Normalize());
                    s = new byte[1024];
                }

                ImGui.EndWindow();
            }
        }
    }
}