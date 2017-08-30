using SixLabors.Fonts;

namespace Apogee.Gui.UI
{
    public class Control : Container
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public Control(int width, int height) : base(width, height)
        {
            Width = width;
            Height = height;
        }

        public virtual void Paint()
        {
            
        }

        public virtual void Draw()
        {
            Apply();
            GuiEngine.DrawImage(X, Y, Width, Height);
        }
    }
}