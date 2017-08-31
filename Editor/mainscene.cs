using System;
using Apogee;
using Apogee.API;
using Apogee.Core;
using Apogee.Resources;
using ImGuiNET;


public class MainScene : Scene
{
    //3D
    private Camera Camera;
    private WorldObject monkey;
    private Shader BasicShader;

    public override void Load()
    {
        Camera = new Camera(0.1f, 1000, Engine.Window.Width, Engine.Window.Height, 70, 10);
        BasicShader = new Shader("./Shaders/basic");
        monkey = new WorldObject(Engine.Assets.Load<Model>("monkey.ogex"));
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
        monkey.Draw(BasicShader, Camera);
    }
}