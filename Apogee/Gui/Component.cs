using Apogee.Resources;

namespace Apogee.Gui
{
    public abstract class Component
    {
        internal Texture _texture;

        public abstract void Generate();
        
        public void Apply()
        {
            _texture.Apply(0);
        }

        public void Dispose()
        {
            _texture.Dispose();
        }
    }
}