using System.Net.Mime;
using Apogee.Components;
using Apogee.Core;

namespace Apogee.Systems
{
    public class Render : Ecs.System
    {
        public void RenderEntity(Shader shader, Model model,
            Transform transform)
        {
            var mvp = transform.GetTranformation();

            shader.Apply();
            shader.Update(mvp, Container.Camera.GetProjection() * mvp);
            shader.SetUniform("viewPos", Container.Camera.Position);

            model.Draw();
        }

        public void RenderEntity(Shader shader, Model model, Transform transform,
            Texture texture)
        {
            texture.Apply(0);
            RenderEntity(shader, model, transform);
        }
    }
}