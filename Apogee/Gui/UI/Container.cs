using Apogee.Resources;
using ImageSharp;
using ImageSharp.Processing;

namespace Apogee.Gui.UI
{
    public class Container
    {
        private object _locker = new object();

        private Image<Rgba32> _img;

        public Image<Rgba32> Buffer
        {
            get
            {
                lock (_locker)
                {
                    return _img;
                }
            }
            set
            {
                lock (_locker)
                {
                    _img = value;
                }
            }
        }

        private Texture _texture { get; set; }

        public Container(int width, int height)
        {
            Buffer = new Image<Rgba32>(width, height);
            Buffer.Fill(Rgba32.Transparent);
            
            _texture = new Texture(Buffer);
        }
        

        public void Resize(int width, int height)
        {
            Buffer.Resize(width, height);
        }
        
        public void Rasterise()
        {
            _texture.Dispose();
            _texture = new Texture(Buffer.RotateFlip(RotateType.Rotate180, FlipType.Horizontal));
        }

        public void Apply()
        {
            _texture.Apply(0);
        }
    }
}