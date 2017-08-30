using ImageSharp;
using SixLabors.Fonts;
using SixLabors.Primitives;

namespace Apogee.Gui.UI.Controls
{
    public class TextControl : Control
    {
        public string Text { get; set; } = "Text Control";
        public Font Font { get; set; }

        public TextControl() : base(1, 1)
        {
            Font = Fonts.DefualtFont;
            
        }
        
        public TextControl(string txt) : this()
        {
            Text = txt;
            this.Paint();
        }

        public override void Paint()
        {
            //clear the image
            Buffer.Fill(Rgba32.Transparent);

            //resize everything to the text size
            var size = TextMeasurer.Measure(Text, Font, 72f, 80f);
            Resize((int) size.Width, (int) size.Height);
            Width = (int) size.Width;
            Height = (int) size.Height;

            Buffer.DrawText(Text, Font, Rgba32.Black, new PointF(0, 0));
            
            Rasterise();
        }
    }
}