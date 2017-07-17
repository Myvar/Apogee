using Apogee.Engine.Core;
using OpenTK;

namespace Apogee.Resources
{
    public class Scene : IResource
    {
        public GameEngine Engine { get; set; }

        public void LoadGameEngine(ref GameEngine ge)
        {
            Engine = ge;
        }

        public virtual void Load()
        {
            
        }

        public virtual void Draw()
        {
            
        }
        
        public virtual void Update(double deltaTime)
        {
            
        }
    }
}