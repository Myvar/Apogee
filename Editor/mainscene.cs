using System;
using Apogee;
using Apogee.Core;
using Apogee.Resources;
using Apogee.Terrain;
using ImGuiNET;


public class MainScene : Scene
{
    //3D
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

    public override void UI()
    {
        ImGui.BeginWindow("Debug", WindowFlags.AlwaysAutoResize);

        if (ImGui.CollapsingHeader("Proformance", TreeNodeFlags.CollapsingHeader))
        {
            ImGui.Text("FPS: " + Math.Round(Engine.Window.RenderFrequency, 0));
        }

        if (ImGui.CollapsingHeader("Rendering", TreeNodeFlags.DefaultOpen))
        {
            //ImGui.Checkbox("Wire frame", ref Scope.WireFrame);                  
        }

        ImGui.EndWindow();
    }

    public override void Draw()
    {
        BasicShader.Apply();
        BasicShader.Update(Transform.GetTranformation(), Camera.GetProjection() * Transform.GetTranformation());
        BasicShader.SetUniform("viewPos", Camera.Position);

        m.Draw();
    }
}