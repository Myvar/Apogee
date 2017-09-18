using System.Collections.Generic;
using Apogee.Resources;

namespace Editor.UI
{
    public abstract class Window
    {
        public abstract void Draw(Scene s);

        public bool Visible { get; set; } = false;

        public static List<Window> Windows = new List<Window>()
        {
            new StyleEditor(),
            new Debug(),
            new ViewPort(),
            new TextEditor(),
            new Test(),
            new MenuBar(),
            new ObjectEditor(),
            new ModelIndex(),
            new TextureIndex(),
            new SceneManager()
        };
    }
}