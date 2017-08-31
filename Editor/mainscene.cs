using System;
using Apogee;
using Apogee.API;
using Apogee.Core;
using Apogee.Resources;
using ImGuiNET;
using OpenTK;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;


public class MainScene : Scene
{
    //3D
    private Camera Camera;

    private WorldObject monkey;
    private Shader BasicShader;
    private RenderTarget MainView;

    public override void Load()
    {
        Camera = new Camera(0.1f, 1000, Engine.Window.Width, Engine.Window.Height, 70, 10);
        BasicShader = new Shader("./Shaders/basic");
        monkey = new WorldObject(Engine.Assets.Load<Model>("monkey.ogex"));
        monkey.Transform.RotX = 90;
        monkey.Transform.RotZ = 180;
        monkey.Transform.Z = 3;
        MainView = new RenderTarget(800, 600);
        Engine.Window.WindowState = WindowState.Maximized;
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


        ImGui.BeginWindow("MainViewPort", WindowFlags.AlwaysAutoResize);

        int x = MainView.Texture;
        ImGui.Image(new IntPtr(x), new Vector2(800, 600), new Vector2(0, 1), new Vector2(1, 0), new Vector4(1, 1, 1, 1),
            new Vector4(0, 0, 0, 0));

        ImGui.EndWindow();
    }

    public override void Draw()
    {
        MainView.Bind();

        monkey.Draw(BasicShader, Camera);

        MainView.BindMainWindow();
    }
}