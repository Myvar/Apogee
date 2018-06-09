using Apogee.Core;
using OpenTK;
using OpenTK.Input;

namespace Apogee
{
    public class Input
    {
        private GameWindow _window;

        public Input(GameWindow w)
        {
            _window = w;
        }

        public bool IsKeyDown(Key key) => _window.Keyboard.GetState().IsKeyDown(key);
        public bool IsKeyUp(Key key) => _window.Keyboard.GetState().IsKeyUp(key);

        public void MouseVisible(bool s)
        {
            if (s)
            {
                _window.Cursor = MouseCursor.Default;
            }
            else
            {
                _window.Cursor = MouseCursor.Empty;
            }
        }

        public bool GetMouseDown(MouseButton m) => _window.Mouse.GetState().IsButtonDown(m);

        public void CenterMouse() => Mouse.SetPosition(_window.Width / 2, _window.Height / 2);

        public Vec2 GetMouseDelta() =>
            (new Vec2(_window.Mouse.X, _window.Mouse.Y) - new Vec2(_window.Width / 2, _window.Height / 2));
    }
}