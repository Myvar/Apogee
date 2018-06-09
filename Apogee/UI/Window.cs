using System;
using System.Collections.Generic;
using System.Reflection;

namespace Apogee.UI
{
    public class Window
    {
        public string Text { get; set; }
        public bool Visible = true;

        public virtual void Draw()
        {
        }

        public static List<Window> Instances { get; set; } = new List<Window>();

        static Window()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.BaseType == typeof(Window))
                {
                    Instances.Add((Window) Activator.CreateInstance(type, new object[] { }));
                }
            }
        }

        public static void Render()
        {
            foreach (var window in Instances)
            {
                window.Draw();
            }
        }
    }
}