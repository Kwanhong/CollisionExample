using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Collections.Generic;
using static Collision.Data;

namespace Collision
{
    class Event
    {
        public List<Action<Vector2f, Mouse.Button>> MousePressedEvents { get; set; }
        public List<Action<Vector2f, Mouse.Button>> MouseReleasedEvents { get; set; }
        public List<Action<Vector2f>> MouseMovedEvents { get; set; }
        public List<Action<Vector2f, float>> MouseScrolledEvents { get; set; }
        public List<Action<Keyboard.Key>> KeyPressedEvents { get; set; }
        public List<Action<Keyboard.Key>> KeyReleasedEvents { get; set; }

        public void InitializeOnce()
        {
            window.Closed += OnClosed;
            window.MouseButtonPressed += OnMousePressed;
            window.MouseButtonReleased += OnMouseReleased;
            window.MouseMoved += OnMouseMoved;
            window.MouseWheelScrolled += OnMouseScrolled;
            window.KeyPressed += OnKeyPressed;
            window.KeyReleased += OnKeyReleased;
            Initialize();
        }

        public void Initialize()
        {
            MousePressedEvents = new List<Action<Vector2f, Mouse.Button>>();
            MouseReleasedEvents = new List<Action<Vector2f, Mouse.Button>>();
            MouseMovedEvents = new List<Action<Vector2f>>();
            MouseScrolledEvents = new List<Action<Vector2f, float>>();
            KeyPressedEvents = new List<Action<Keyboard.Key>>();
            KeyReleasedEvents = new List<Action<Keyboard.Key>>();
        }

        public void HandleEvents()
        {
            window.DispatchEvents();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            window.Close();
        }
        
        #region  Keyboard
        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (KeyPressedEvents == null) return;

            foreach (var evnt in KeyPressedEvents)
                evnt(e.Code);
        }
        private void OnKeyReleased(object sender, KeyEventArgs e)
        {
            if (KeyReleasedEvents == null) return;

            foreach (var evnt in KeyReleasedEvents)
                evnt(e.Code);
        }
        #endregion

        #region  Mouse
        private void OnMousePressed(object sender, MouseButtonEventArgs e)
        {
            if (MousePressedEvents == null) return;

            foreach (var evnt in MousePressedEvents)
                evnt(new Vector2f(e.X, e.Y), e.Button);
        }
        private void OnMouseReleased(object sender, MouseButtonEventArgs e)
        {
            if (MouseReleasedEvents == null) return;

            foreach (var evnt in MouseReleasedEvents)
                evnt(new Vector2f(e.X, e.Y), e.Button);
        }
        private void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (MouseMovedEvents == null) return;

            foreach (var evnt in MouseMovedEvents)
                evnt(new Vector2f(e.X, e.Y));
        }
        private void OnMouseScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            if (MouseScrolledEvents == null) return;

            foreach (var evnt in MouseScrolledEvents)
                evnt(new Vector2f(e.X, e.Y), e.Delta);
        }
        #endregion
    }
}