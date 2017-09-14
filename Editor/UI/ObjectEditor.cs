using Apogee.API;
using Apogee.Resources;
using ImGuiNET;

namespace Editor.UI
{
    public class ObjectEditor : Window
    {
        public static WorldObject CurrentWorldObject { get; set; }

        public ObjectEditor()
        {
            Visible = true;
        }

        public override void Draw(Scene s)
        {
            ImGui.BeginWindow("ObjectEditor");



            if (CurrentWorldObject == null)
            {
                ImGui.EndWindow();
                return;
            }
            #region Transform
            if (ImGui.CollapsingHeader("Transform", TreeNodeFlags.CollapsingHeader))
            {
                {
                    ImGui.Text("Translation");
                    {
                        float f = CurrentWorldObject.Transform.X;
                        if (ImGui.DragFloat("X", ref f, float.MinValue, float.MaxValue, 0.1f))
                        {
                            CurrentWorldObject.Transform.X = f;
                        }
                    }
                    {
                        float f1 = CurrentWorldObject.Transform.Y;
                        if (ImGui.DragFloat("Y", ref f1, float.MinValue, float.MaxValue, 0.1f))
                        {
                            CurrentWorldObject.Transform.Y = f1;
                        }
                    }
                    {
                        float f2 = CurrentWorldObject.Transform.Z;
                        if (ImGui.DragFloat("Z", ref f2, float.MinValue, float.MaxValue, 0.1f))
                        {
                            CurrentWorldObject.Transform.Z = f2;
                        }
                    }
                }
                {
                    ImGui.Text("Rotation");
                    {
                        float f3 = CurrentWorldObject.Transform.RotX;
                        if (ImGui.DragFloat("RotX", ref f3, float.MinValue, float.MaxValue, 0.1f))
                        {
                            CurrentWorldObject.Transform.RotX = f3;
                        }
                    }
                    {
                        float f4 = CurrentWorldObject.Transform.RotY;
                        if (ImGui.DragFloat("RotY", ref f4, float.MinValue, float.MaxValue, 0.1f))
                        {
                            CurrentWorldObject.Transform.RotY = f4;
                        }
                    }
                    {
                        float f5 = CurrentWorldObject.Transform.RotZ;
                        if (ImGui.DragFloat("RotZ", ref f5, float.MinValue, float.MaxValue, 0.1f))
                        {
                            CurrentWorldObject.Transform.RotZ = f5;
                        }
                    }
                }
#endregion
            }
            
            if (ImGui.CollapsingHeader("Model", TreeNodeFlags.CollapsingHeader))
            {
                ImGui.Text("Scale");
                {
                    float f = CurrentWorldObject.Transform.Scale.X;
                    if (ImGui.DragFloat("ScalX", ref f, float.MinValue, float.MaxValue, 0.01f))
                    {
                        CurrentWorldObject.Transform.Scale.X = f;
                    }
                }
                {
                    float f1 = CurrentWorldObject.Transform.Scale.Y;
                    if (ImGui.DragFloat("ScalY", ref f1, float.MinValue, float.MaxValue, 0.01f))
                    {
                        CurrentWorldObject.Transform.Scale.Y = f1;
                    }
                }
                {
                    float f2 = CurrentWorldObject.Transform.Scale.Z;
                    if (ImGui.DragFloat("ScalZ", ref f2, float.MinValue, float.MaxValue, 0.01f))
                    {
                        CurrentWorldObject.Transform.Scale.Z = f2;
                    }
                }
            }
           
            if (ImGui.CollapsingHeader("Textures", TreeNodeFlags.CollapsingHeader))
            {
                
            }

            ImGui.EndWindow();
        }
    }
}