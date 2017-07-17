using Apogee;
using Apogee.Core;
using Apogee.Resources;

public class MainScene : Scene
{
    Camera Camera;
    Model m;
    Transform Transform;
    Shader BasicShader;
 
    public override void Load()
    {
       m = Engine.Assets.Load<Model>("monkey.ogex");
       Camera = new Camera(0.1f, 1000, Engine.Window.Width, Engine.Window.Height, 70, 10);
       BasicShader = new Shader("./Shaders/basic");     
       Transform = new Transform();
    }

    public override void Update(double deltaTime)
    {
        Camera.Input(Engine.Input, deltaTime);
    }

    public override void Draw()
    {
       BasicShader.Apply();
       BasicShader.Update(Transform.GetTranformation(), Camera.GetProjection() * Transform.GetTranformation());
       BasicShader.SetUniform("viewPos", Camera.Position);
        
       m.Draw();
    }
}