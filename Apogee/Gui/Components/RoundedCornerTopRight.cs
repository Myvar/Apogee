using Apogee.Resources;
using ImageSharp;

namespace Apogee.Gui.Components
{
    public class RoundedCornerTopRight : Component
    {
        public override void Generate()
        {
            var img = new Image<Rgba32>(5, 5);
            
            _texture = new Texture(img);
        }
    }
}