using System.Collections.Generic;

namespace Apogee.Gui
{
    public static class ComponentBuffer
    {
        private static List<Component> _components = new List<Component>();

        public static void Add(Component c)
        {
            foreach (var component in _components)
            {
                if(component.GetType() == c.GetType()) return;
            }
              
            _components.Add(c);
        }

        public static void RenderComponent<T>(int x, int y, int w, int h)
        {
            foreach (var component in _components)
            {
                if (component.GetType() ==  typeof(T))
                {
                    component.Apply();
                    GuiEngine.DrawImage(x, y, w, h);
                    
                    return;
                }
            }
        }
    }
}