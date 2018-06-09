using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Apogee.Components;
using Apogee.Core;
using Apogee.Ecs;
using ImGuiNET;
using Newtonsoft.Json;

namespace Apogee.UI
{
    public class EntityEditor : Window
    {
        private JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public override void Draw()
        {
            ImGui.BeginWindow("Entity Editor", ref Visible, WindowFlags.Default);

            if (ImGui.Button("New"))
            {
                Dialog.TextDialog(s =>
                {
                    var guid = Guid.NewGuid();
                    lock (Container._locker)
                    {
                        GameEngine.Container.Components.Add(
                            new Shader()
                            {
                                Entity = guid,
                                File = "basic"
                            });

                        GameEngine.Container.Components.Add(
                            new Texture()
                            {
                                Entity = guid,
                                File = "dev-orange.jpg"
                            });

                        GameEngine.Container.Components.Add(
                            new Model()
                            {
                                Entity = guid,
                                File = "cube.ogex"
                            });
                        GameEngine.Container.Components.Add(
                            new Transform()
                            {
                                Entity = guid,
                            });
                        GameEngine.Container.Components.Add(
                            new MetaData()
                            {
                                Entity = guid,
                                Name = s
                            });
                    }

                    GameEngine.Container.Update(true);
                    AssetManager.SelectedEntity = guid;
                });
            }

            ImGui.Separator();

            if (UI.AssetManager.SelectedEntity == Guid.Empty)
            {
                ImGui.Text("No Entity Selexted");
                ImGui.EndWindow();
                return;
            }


            if (ImGui.Button("Delete"))
            {
                GameEngine.Container.DeleteEntity(AssetManager.SelectedEntity);
            }

            if (ImGui.Button("Rename"))
            {
                Dialog.TextDialog(s =>
                {
                    GameEngine.Container.GetComponent<MetaData>(AssetManager.SelectedEntity)
                        .Name = s;
                });
            }

            if (ImGui.Button("Duplicate"))
            {
                Dialog.TextDialog(s =>
                {
                    var x = JsonConvert.SerializeObject(GameEngine.Container.GetEntity(AssetManager.SelectedEntity),
                        _jsonSerializerSettings);
                    var components = (List<Component>) JsonConvert.DeserializeObject(x, _jsonSerializerSettings);
                    var newid = Guid.NewGuid();
                    for (var i = 0; i < components.Count; i++)
                    {
                        var component = components[i];
                        component.Entity = newid;
                        if (component is MetaData metta)
                        {
                            metta.Name = s;
                        }

                        GameEngine.Container.Components.Add(component);
                    }

                    GameEngine.Container.Update(true);

                    AssetManager.SelectedEntity = newid;
                });
            }

            foreach (var component in GameEngine.Container.GetEntity(UI.AssetManager.SelectedEntity))
            {
                ImGui.Separator();
                ComponentEditor(component);
            }

            ImGui.EndWindow();
        }

        public void ComponentEditor(Component component)
        {
            ImGui.Text(component.GetType().Name);

            if (component is Model mdl)
            {
                var target = AssetManager.ThumnailCache[Path.Combine(Container.AssetsPath, mdl.File)];
                ImGui.Image(new IntPtr(target.Texture), new Vector2(128, 128), new Vector2(0, 1),
                    new Vector2(1, 0),
                    new Vector4(1, 1, 1, 1),
                    new Vector4(0, 0, 0, 0)
                );
            }

            if (component is Texture texture)
            {
                ImGui.Image(new IntPtr(texture.TextureId), new Vector2(128, 128),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector4(1, 1, 1, 1),
                    new Vector4(0, 0, 0, 0)
                );
            }

            foreach (var propertyInfo in component.GetType().GetTypeInfo().GetProperties())
            {
                if (propertyInfo.GetCustomAttribute<JsonIgnoreAttribute>() != null) continue;
                if (propertyInfo.Name == "Entity") continue;

                if (propertyInfo.PropertyType == typeof(int))
                {
                    var accesor = new PropertyInfoAccesor<int>(propertyInfo, component);
                    var x = accesor.Value;
                    ImGui.DragInt(component.GetType().Name + "." + propertyInfo.Name, ref x, 1,
                        Int32.MinValue, Int32.MaxValue, accesor.Value.ToString());
                    accesor.Value = x;
                }

                if (propertyInfo.PropertyType == typeof(float))
                {
                    var accesor = new PropertyInfoAccesor<float>(propertyInfo, component);
                    var x = accesor.Value;
                    ImGui.DragFloat(component.GetType().Name + "." + propertyInfo.Name, ref x, 1,
                        float.MinValue, float.MaxValue, accesor.Value.ToString());
                    accesor.Value = x;
                }

                if (propertyInfo.PropertyType == typeof(Vec3))
                {
                    ImGui.Separator();
                    ImGui.Text(component.GetType().Name + "." + propertyInfo.Name);

                    var accesor = new PropertyInfoAccesor<Vec3>(propertyInfo, component);
                    {
                        var x = accesor.Value.X;
                        ImGui.DragFloat(component.GetType().Name + "." + propertyInfo.Name + ".X", ref x,
                            float.MinValue, float.MaxValue, 0.1f);
                        accesor.Value.X = x;
                    }
                    {
                        var x = accesor.Value.Y;
                        ImGui.DragFloat(component.GetType().Name + "." + propertyInfo.Name + ".Y", ref x,
                            float.MinValue, float.MaxValue, 0.1f);
                        accesor.Value.Y = x;
                    }
                    {
                        var x = accesor.Value.Z;
                        ImGui.DragFloat(component.GetType().Name + "." + propertyInfo.Name + ".Z", ref x,
                            float.MinValue, float.MaxValue, 0.1f);
                        accesor.Value.Z = x;
                    }
                }
            }
        }
    }
}