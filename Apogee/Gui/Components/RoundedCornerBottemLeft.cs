using Apogee.Resources;
using ImageSharp;
using SixLabors.Primitives;
using SixLabors.Shapes;

namespace Apogee.Gui.Components
{
    public class RoundedCornerBottemLeft : Component
    {
        public override void Generate()
        {
            int raduis = 5;
            var img = new Image<Rgba32>(raduis, raduis);
            
            var border = new PathBuilder();
            
            border.AddBezier(new PointF(0, raduis), new PointF(0, 0), new PointF(raduis, 0));
            
            _texture = new Texture(img);
        }
    }
}