using System.Dynamic;
using System.Numerics;
using ImageSharp;
using ImageSharp.Drawing.Brushes;
using ImageSharp.Drawing.Pens;
using SixLabors.Fonts;
using SixLabors.Primitives;
using SixLabors.Shapes;

namespace Apogee.Gui.UI.Controls
{
    public class ButtonControl : Control
    {
        public string Text { get; set; } = "btn";
        public Font Font { get; set; }

        public ButtonControl() : base(32, 26)
        {
            Font = Fonts.DefualtFont;
        }

        public ButtonControl(string txt) : this()
        {
            Text = txt;
            this.Paint();
        }

        public override void Paint()
        {
            //clear the image
            Buffer.Fill(Rgba32.Transparent);


            //resize everything to the text size
            var size = TextMeasurer.Measure(Text, Font, 72f, 92f);
            size.Width += 14 + 2;
            size.Height += 8 + 2;
            Resize((int) size.Width, (int) size.Height);
            Width = (int) size.Width;
            Height = (int) size.Height;

            var bg = Rgba32.FromHex("#AFB3B6");
            var borderColor = Rgba32.FromHex("#272C34");
            var textColor = Rgba32.FromHex("#20252C");
            //draw border

            var border = new PathBuilder();

            

            int raduis = 5;
            int borderwidth = 1;


            int offsetW = 2;
            int offsetH = 2;
            
            //top left border
            border.AddBezier(new PointF(0, raduis), new PointF(0, 0), new PointF(raduis, 0));

            //top right
            border.AddBezier(new PointF(Width - offsetW - raduis, 0), new PointF(Width - offsetW, 0),
                new PointF(Width - offsetW, raduis));

            //bottem right
            border.AddBezier(new PointF(Width - offsetW, Height - offsetH - raduis),
                new PointF(Width - offsetW, Height - offsetH),
                new PointF(Width - offsetW - raduis, Height - offsetH));

            //bottem left
            border.AddBezier(new PointF(raduis, Height - offsetH), new PointF(0, Height - offsetH),
                new PointF(0, Height - offsetH - raduis));


            //draw border
            Buffer.Fill(bg, border.Build()
                .Transform(Matrix3x2.CreateTranslation(1, 1)));

            Buffer.Draw(Pens.Solid(borderColor, borderwidth), border.CloseFigure().Build()
                .Transform(Matrix3x2.CreateTranslation(1, 1)));


            Buffer.DrawText(Text, Font, textColor, new PointF(8, 5));

            Rasterise();
        }
    }
}