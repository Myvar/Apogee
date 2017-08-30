using SixLabors.Fonts;

namespace Apogee.Gui.UI
{
    public static class Fonts
    {
        private static FontCollection _FontCollection = new FontCollection();

        public static Font DefualtFont { get; set; }

        public static void Init()
        {
            FontFamily font = _FontCollection.Install("./Fonts/Roboto-Regular.ttf");
            DefualtFont = new Font(font, 20);
        }

    }
}