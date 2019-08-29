using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Collections.Generic;
using static Collision.Consts;
using static Collision.Utility;
using static Collision.Data;

namespace Collision
{
    class Button
    {
        public class ButtonStyle
        {
            public ButtonStyle()
            {
                fillColor = Color.White;
                textColor = Color.Black;
                fontName = "NANUMGOTHICLIGHT";
                font = new Font("Resources\\Fonts\\" + FontName + ".TTF");
            }
            public string FontName
            {
                get => fontName; set
                {
                    fontName = value;
                    font = new Font("Resources\\Fonts\\" + fontName + ".TTF");
                }
            }
            public Color fillColor;
            public Color textColor;
            public Font font;
            private string fontName;
        }

        public Vector2f Position { get; set; }
        public Vector2f Size { get; set; }
        public string Caption { get; set; }
        public bool Enabled { get; set; }
        public bool IsPressed { get; set; }

        public List<Action> ButtonPressedEvents { get; set; }
        public List<Action> ButtonReleasedEvents { get; set; }
        public ButtonStyle Style { get; set; }

        private int brightness;

        public Button(Vector2f position, Vector2f size, string caption)
        {
            this.Position = position;
            this.Size = size;
            this.Caption = caption;

            Enabled = true;
            IsPressed = false;
            brightness = 0;

            ButtonPressedEvents = new List<Action>();
            ButtonReleasedEvents = new List<Action>();

            events.MouseMovedEvents.Add(OnMouseMoved);
            events.MouseReleasedEvents.Add(OnMouseReleased);
            events.MousePressedEvents.Add(OnMousePressed);

            this.Style = new ButtonStyle();
        }

        public void Update()
        {
        }

        public void Display()
        {
            var rectColor = new Color
            (
                (byte)Limit(Style.fillColor.R + brightness, 0, 255),
                (byte)Limit(Style.fillColor.G + brightness, 0, 255),
                (byte)Limit(Style.fillColor.B + brightness, 0, 255)
            );

            RectangleShape rect = new RectangleShape(Size);
            rect.Origin = this.Size / 2;
            rect.Position = this.Position;
            rect.FillColor = rectColor;
            window.Draw(rect);

            Text text = new Text(Caption, Style.font);
            text.Origin = new Vector2f(text.GetGlobalBounds().Width * 0.52f, text.GetGlobalBounds().Height * 0.9f);
            text.Position = rect.Position;
            text.FillColor = Style.textColor;
            window.Draw(text);
        }

        private void OnMousePressed(Vector2f mousePos, Mouse.Button button)
        {
            if (button != Mouse.Button.Left) return;

            if (OnTheButton(mousePos))
            {
                IsPressed = true;
                brightness = -10;

                foreach (var evnt in ButtonPressedEvents)
                    evnt();

            }
        }

        private void OnMouseReleased(Vector2f mousePos, Mouse.Button button)
        {
            if (!IsPressed) return;
            IsPressed = false;

            if (OnTheButton(mousePos))
            {
                brightness = 10;

                foreach (var evnt in ButtonReleasedEvents)
                    evnt();
            }
            else
            {
                brightness = 0;
            }
        }

        private void OnMouseMoved(Vector2f mousePos)
        {
            if (IsPressed) return;

            if (OnTheButton(mousePos))
            {
                brightness = 20;
            }
            else
            {
                brightness = 0;
            }
        }

        private bool OnTheButton(Vector2f mousePos)
        {
            if (mousePos.X > Position.X - Size.X / 2 &&
                mousePos.X < Position.X + Size.X / 2 &&
                mousePos.Y > Position.Y - Size.Y / 2 &&
                mousePos.Y < Position.Y + Size.Y / 2)
                return true;
            else
                return false;
        }
    }
}