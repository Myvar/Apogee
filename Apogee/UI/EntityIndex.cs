using Apogee.Components;
using ImGuiNET;

namespace Apogee.UI
{
    public class EntityIndex : Window
    {
        public override void Draw()
        {
            ImGui.BeginWindow("Entity Index", ref Visible, WindowFlags.Default);

            foreach (var entity in GameEngine.Container.BuildEntityIndex())
            {
                var x = GameEngine.Container.GetComponent<MetaData>(entity.id);

                if (ImGui.Button(x.Name))
                {
                    UI.AssetManager.SelectedEntity = entity.id;
                }
            }

            ImGui.EndWindow();
        }
    }
}