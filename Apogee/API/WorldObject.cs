using Apogee.Core;
using Apogee.Resources;

namespace Apogee.API
{
    public class WorldObject
    {
        public Transform Transform { get; set; } = new Transform();
        public Model Model { get; set; }

        public WorldObject(Model model)
        {
            Model = model;
        }

        public void Draw(Shader shader, Camera camera)
        {
            shader.Apply();
            shader.Update(Transform.GetTranformation(), camera.GetProjection() * Transform.GetTranformation());
            shader.SetUniform("viewPos", camera.Position);

            Model.Draw();
        }
    }
}