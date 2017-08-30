using Apogee;
using Apogee.Core;
using Apogee.Resources;
using Apogee.Gui.UI;
using Apogee.Gui.UI.Controls;
using Apogee.Terrain;


public class MainScene : Scene
{
    //3D
    Camera Camera;
    Model m;
    Transform Transform;
    Shader BasicShader;
    TerrainModel Terrain;

    //2D
 
    public override void Load()
    {
        m = Engine.Assets.Load<Model>("monkey.ogex");
        Camera = new Camera(0.1f, 1000, Engine.Window.Width, Engine.Window.Height, 70, 10);
        BasicShader = new Shader("./Shaders/basic");     
        Transform = new Transform();
        Terrain = new TerrainModel("./terainmaptest.png");

        
    }

    public override void Update(double deltaTime)
    {
        Camera.Input(Engine.Input, deltaTime);
    }

    public override void UI()
    {
    }

    public override void Draw()
    {
        Terrain.Draw(Camera); 

        BasicShader.Apply();
        BasicShader.Update(Transform.GetTranformation(), Camera.GetProjection() * Transform.GetTranformation());
        BasicShader.SetUniform("viewPos", Camera.Position);

        m.Draw();
    }
}