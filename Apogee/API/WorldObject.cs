using Apogee.Core;
using Apogee.Resources;

namespace Apogee.API
{
    public class WorldObject
    {
        public Transform Transform { get; set; } = new Transform();
        public Model Model { get; set; }
        public Texture Texture { get; set; }

        public WorldObject(Model model)
        {
            Model = model;
        }
        
        public WorldObject(Model model, Texture t)
        {
            Model = model;
            Texture = t;
        }

        public void Draw(Shader shader, Camera camera)
        {
            Texture?.Apply(0);
            
            shader.Apply();
            shader.Update(Transform.GetTranformation(), camera.GetProjection() * Transform.GetTranformation());
            shader.SetUniform("viewPos", camera.Position);

            Model.Draw();
        }
    }
}