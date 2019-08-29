using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Collections.Generic;
using static Collision.Utility;
using static Collision.Consts;
using static Collision.Data;

namespace Collision
{
    public class ConsoleBox
    {
        public const uint MaxCharSize = 75;
        public const uint MinCharSize = 5;
        public enum ViewMode { Left, Right, Top, Bott }
        public Vector2f Position { get; set; }
        public Vector2f Origin { get; set; }
        public Vector2f Size { get; set; }
        public Color BackColor { get; set; }
        public Color TextColor { get; set; }
        public string String
        {
            get => str; set
            {
                str = value;
                if (text != null)
                {
                    text.DisplayedString = str;
                    ResetTextBox();
                }
            }
        }
        private Text text;
        private string str;
        private Font font;
        private RenderTexture texture;
        private View view;
        private bool isDragging;
        private Vector2f delta;
        private Vector2f textBox;
        private int cursor;

        public ConsoleBox(Vector2f position, Vector2f size)
        {
            this.cursor = 0;
            this.Position = position;
            this.Size = size;
            this.Origin = size / 2;

            this.font = new Font("Resources\\Fonts\\NANUMGOTHICLIGHT.TTF");
            this.text = new Text(String, font);
            this.text.CharacterSize = 15;
            this.BackColor = new Color(50, 50, 50);
            this.TextColor = Color.White;
            this.texture = new RenderTexture((uint)Size.X, (uint)Size.Y);
            this.view = new View(new FloatRect(0, 0, Size.X, Size.Y));
            this.String = "";

            this.isDragging = false;

            events.MouseMovedEvents.Add(OnMouseMoved);
            events.MouseReleasedEvents.Add(OnMouseReleased);
            events.MousePressedEvents.Add(OnMousePressed);
            events.MouseScrolledEvents.Add(OnMouseScrolled);
            events.KeyPressedEvents.Add(OnTextTyped);
        }

        public ConsoleBox(float posX, float posY, float witdh, float height)
        {
            this.cursor = 0;
            this.Position = new Vector2f(posX, posY);
            this.Size = new Vector2f(witdh, height);
            this.Origin = Size / 2;

            this.font = new Font("Resources\\Fonts\\NANUMGOTHICLIGHT.TTF");
            this.text = new Text(String, font);
            this.text.CharacterSize = 15;
            this.BackColor = new Color(50, 50, 50);
            this.TextColor = Color.White;
            this.texture = new RenderTexture((uint)Size.X, (uint)Size.Y);
            this.view = new View(new FloatRect(0, 0, Size.X, Size.Y));
            this.String = "";

            this.isDragging = false;

            events.MouseMovedEvents.Add(OnMouseMoved);
            events.MouseReleasedEvents.Add(OnMouseReleased);
            events.MousePressedEvents.Add(OnMousePressed);
            events.MouseScrolledEvents.Add(OnMouseScrolled);
            events.KeyPressedEvents.Add(OnTextTyped);
        }

        public void Update()
        {
            var newView = view.Center;

            if (textBox.X < this.Size.X)
            {
                if (newView.X - textBox.X < -(Origin.X + 5))
                    newView.X = textBox.X - (Origin.X + 5);

                if (newView.X > Origin.X + 25)
                    newView.X = Origin.X + 25;
            }
            else
            {
                if (newView.X - textBox.X > -(Origin.X + 5))
                    newView.X = textBox.X - (Origin.X + 5);

                if (newView.X < Origin.X + 25)
                    newView.X = Origin.X + 25;
            }

            if (textBox.Y < this.Size.Y)
            {
                var offset = Map(text.CharacterSize, MinCharSize, MaxCharSize, 10, -53) - 10;
                if (newView.Y - textBox.Y < -(Origin.Y + offset))
                    newView.Y = textBox.Y - (Origin.Y + offset);

                offset = Map(text.CharacterSize, MinCharSize, MaxCharSize, 10, -53) + 28;
                if (newView.Y > Origin.Y + offset)
                    newView.Y = Origin.Y + offset;
            }
            else
            {
                if (newView.Y < Origin.Y)
                    newView.Y = Origin.Y;

                if (newView.Y - textBox.Y > -Origin.Y)
                    newView.Y = textBox.Y - Origin.Y;
            }

            view.Center = newView;
        }

        public void Display()
        {
            DisplayCursor();
            DisplayConsole();
            DisplayToolbar();
            DeleteCursor();
        }

        #region  Display
        private void DisplayCursor()
        {
            if (cursor <= 0)
            {
                this.String = "_" + String;
                return;
            }

            this.String =
            String.Substring(0, cursor) + "_" +
            String.Substring(cursor, String.Length - cursor);
            cursor++;
        }
        private void DeleteCursor()
        {
            if (cursor <= 0)
            {
                if (String.Length <= 1) String = "";
                else String = String.Substring(1, String.Length - 1);
                return;
            }

            this.String =
            String.Substring(0, cursor - 1) +
            String.Substring(cursor, String.Length - cursor);
            cursor--;
        }
        private void DisplayConsole()
        {
            texture.SetView(view);
            texture.Clear(BackColor);
            text.Position = this.Position - this.Origin + new Vector2f(0, 30);
            text.FillColor = this.TextColor;
            texture.Draw(text);
            texture.Display();

            Sprite rect = new Sprite(texture.Texture);
            rect.Origin = this.Origin;
            rect.Position = this.Position;
            window.Draw(rect);
        }

        private void DisplayToolbar()
        {

            Vector2f tbSize = new Vector2f(this.Size.X, 30);
            Vector2f tbOrigin = tbSize / 2;
            Vector2f tbPosition = this.Position - this.Origin + new Vector2f(this.Origin.X, tbOrigin.Y);
            Color tbColor = new Color(43, 43, 43);
            RectangleShape toolBar = new RectangleShape(tbSize);
            toolBar.Origin = tbOrigin;
            toolBar.Position = tbPosition;
            toolBar.FillColor = tbColor;
            window.Draw(toolBar);

            Text tbText = new Text("CONSOLE", font);
            tbText.CharacterSize = 14;
            tbText.Position = toolBar.Position + new Vector2f(5, 5);
            tbText.Origin = toolBar.Origin;
            tbText.FillColor = new Color(125, 125, 125);
            window.Draw(tbText);
        }
        #endregion

        public void Write(object str)
        {
            this.String += str;
        }

        public void WriteLine(object str)
        {
            this.String += str + "\n";
        }

        public void Endline(int times = 1)
        {
            for (int i = 0; i < times; i++)
                this.String += "\n";
        }

        public void Clear()
        {
            SetView(ViewMode.Left);
            SetView(ViewMode.Top);
            this.String = "";
        }

        public void SetView(ViewMode viewMode)
        {
            if (textBox.X > this.Size.X)
            {
                if (viewMode == ViewMode.Left)
                    view.Center = new Vector2f(Origin.X, view.Center.Y);
                if (viewMode == ViewMode.Right)
                    view.Center = new Vector2f(textBox.X - Origin.X, view.Center.Y);
            }
            if (textBox.Y > this.Size.Y)
            {
                if (viewMode == ViewMode.Top)
                    view.Center = new Vector2f(view.Center.X, Origin.Y);
                if (viewMode == ViewMode.Bott)
                    view.Center = new Vector2f(view.Center.X, textBox.Y - Origin.Y);
            }
        }

        #region Events
        private void OnMousePressed(Vector2f mousePos, Mouse.Button button)
        {
            if (button != Mouse.Button.Left && button != Mouse.Button.Right) return;
            if (!OnTheConsole(mousePos)) return;

            isDragging = true;
            delta = -mousePos - view.Center;
        }

        private void OnMouseMoved(Vector2f mousePos)
        {
            if (!isDragging) return;
            var newView = -mousePos - delta;
            view.Center = newView;
        }

        private void OnMouseReleased(Vector2f mousePos, Mouse.Button button)
        {
            if (!isDragging) return;
            isDragging = false;
        }

        private void OnMouseScrolled(Vector2f mousePos, float delta)
        {
            if (!OnTheConsole(mousePos)) return;

            text.CharacterSize += (uint)(delta);
            if (delta > 0 && text.CharacterSize > MaxCharSize)
                text.CharacterSize = MaxCharSize;
            if (delta < 0 && text.CharacterSize < MinCharSize)
                text.CharacterSize = MinCharSize;

            OnMouseMoved(mousePos);
            ResetTextBox();
        }
        #endregion

        private bool OnTheConsole(Vector2f mousePos)
        {
            if (mousePos.X > Position.X - Origin.X &&
                mousePos.X < Position.X + Origin.X &&
                mousePos.Y > Position.Y - Origin.Y &&
                mousePos.Y < Position.Y + Origin.Y)
                return true;
            else
                return false;
        }

        private void ResetTextBox()
        {
            this.textBox = new Vector2f
            (
               text.GetGlobalBounds().Width + 30,
               text.GetGlobalBounds().Height + 60 - text.CharacterSize * 1.6f
            );
        }

        private void OnTextTyped(Keyboard.Key key)
        {
            if (key >= Keyboard.Key.A && key <= Keyboard.Key.Z)
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.LShift) ||
                   Keyboard.IsKeyPressed(Keyboard.Key.RShift))
                    TypeLetter(((char)(key + 65)).ToString());
                else TypeLetter(((char)(key + 97)).ToString());
            }
            else if (key == Keyboard.Key.Enter)
                TypeLetter("\n");
            else if (key == Keyboard.Key.Space)
                TypeLetter(" ");
            else if (key == Keyboard.Key.Backspace && String.Length > 0)
                Backspace();
            else if (key == Keyboard.Key.Left && cursor > 0)
                cursor--;
            else if (key == Keyboard.Key.Right && cursor < String.Length)
                cursor++;
        }

        private void TypeLetter(string letter)
        {
            this.String =
            String.Substring(0, cursor) + letter +
            String.Substring(cursor, String.Length - cursor);
            cursor++;
        }

        private void Backspace()
        {
            if (cursor <= 0) return; 

            this.String =
            String.Substring(0, cursor - 1) +
            String.Substring(cursor, String.Length - cursor);
            cursor--;
        }
    }
}