using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using Apogee.Resources;
using ImGuiNET;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Editor.UI
{
    public class StyleEditor : Window
    {
        public Style Style { get; set; }
        public Dictionary<string, Vector4> Colors = new Dictionary<string, Vector4>();

        public StyleEditor()
        {
            Style = ImGui.GetStyle();
            foreach (var name in Enum.GetNames(typeof(ColorTarget)))
            {
                Colors.Add(name, Style.GetColor((ColorTarget) Enum.Parse(typeof(ColorTarget), name)));
            }
        }

        public void SaveStyle(string file)
        {
        }

        public void LoadStyle(string file)
        {
        }

        public override void Draw(Scene s)
        {
            if (ImGui.BeginWindow("StyleEditor"))
            {
                if (ImGui.CollapsingHeader("Properties", TreeNodeFlags.Framed))
                {
                    foreach (var propertyInfo in Style.GetType().GetTypeInfo().GetProperties())
                    {
                        if (propertyInfo.PropertyType == typeof(Single))
                        {
                            float f = 0;
                            if (ImGui.DragFloat(propertyInfo.Name, ref f, 0.001f, 1))
                            {
                                propertyInfo.SetValue(Style, f);
                            }
                        }
                        else
                        {
                            ImGui.Text(propertyInfo.Name + ": " + propertyInfo.PropertyType);
                        }
                    }
                }

                if (ImGui.CollapsingHeader("Colors", TreeNodeFlags.Framed))
                {
                    foreach (var name in Enum.GetNames(typeof(ColorTarget)))
                    {
                        var target = (ColorTarget) Enum.Parse(typeof(ColorTarget), name);
                        var c = Style.GetColor(target);
                        if (ImGui.ColorEdit4(name, ref c, true))
                        {
                            Style.SetColor(target, c);
                        }
                    }
                }


                ImGui.EndWindow();
            }
        }
    }
}