using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.IO;
using static Collision.Consts;
using static Collision.Utility;
using static Collision.Data;

namespace Collision
{
    class Game
    {
        Matrix P;
        Matrix Q1;
        Matrix w;
        Matrix v;
        Font font;

        ConsoleBox console;

        public void Initialize()
        {
            console = new ConsoleBox(700, 100, 200, 200);
            P = new Matrix(RowVec(250, 200));
            v = new Matrix(RowVec(300, 200));
            w = new Matrix(RowVec(300, 50));
            Q1 = Matrix.Add(P, w);
            font = new Font("Resources/Fonts/NANUMGOTHICLIGHT.TTF");
        }

        private void Update()
        {
            console.Update();
        }

        private void Display()
        {
            console.Display();
            
            var line = new VertexArray(PrimitiveType.Lines, 2);

            line[0] = new Vertex(P.ToVector2());
            line[1] = new Vertex(Matrix.Add(P, v).ToVector2());
            window.Draw(line);

            line[0] = new Vertex(P.ToVector2(), new Color(255, 255, 255, 100));
            line[1] = new Vertex(Q1.ToVector2(), new Color(255, 255, 255, 100));
            window.Draw(line);

            var point = new CircleShape(3);
            point.Origin = new Vector2f(point.Radius, point.Radius);
            point.OutlineThickness = 0.5f;

            point.FillColor = Color.Blue;
            point.Position = Q1.ToVector2();
            window.Draw(point);

            var vsqr = Matrix.Multiply(Matrix.Transpose(v), v).ToFloat();
            var proj = Matrix.Multiply(Matrix.Transpose(w), v).ToFloat();
            var Q2 = Matrix.Add(P, Matrix.Multiply(v, (proj / vsqr)));

            var den = Matrix.Multiply(Matrix.Transpose(v), v).ToFloat();
            var num = Matrix.Multiply(Matrix.Transpose(w), v).ToFloat();
            var t = num / den;
            if (t < 0) Q2 = P;
            else if (t > 1) Q2 = Matrix.Add(P,v);

            point.FillColor = Color.Red;
            point.Position = Q2.ToVector2();
            window.Draw(point);

            line[0] = new Vertex(Q1.ToVector2(), new Color(255, 255, 255, 100));
            line[1] = new Vertex(Q2.ToVector2(), new Color(255, 255, 255, 100));
            window.Draw(line);

            line[0] = new Vertex(P.ToVector2(), new Color(255, 255, 255, 100));
            line[1] = new Vertex(Q2.ToVector2(), new Color(255, 255, 255, 100));
            window.Draw(line);

            var dist = (int)Distnace(Q1.ToVector2(), Q2.ToVector2());
            Text text = new Text(dist.ToString(), font);
            text.Origin = new Vector2f(text.GetGlobalBounds().Width / 2, 0);
            text.Position = Q1.ToVector2() - (Q1.ToVector2() - Q2.ToVector2()) / 2;
            text.Rotation = ToDegree(GetAngle((Q1.ToVector2() - Q2.ToVector2())));
            text.CharacterSize = 20;
            window.Draw(text);
        }

        private void LateUpdate()
        {
        }

        #region GameBase
        public void Run()
        {
            while (window.IsOpen)
            {
                events.HandleEvents();
                Update();
                Display();
                window.Display();
                window.Clear(winBackColor);
                LateUpdate();
            }
        }

        public void InitializeOnce()
        {
            events.KeyPressedEvents.Add(OnKeyPressed);
            events.MouseMovedEvents.Add(OnMouseMoved);
            window.SetFramerateLimit(winFrameLimit);
            window.SetMouseCursorVisible(false);
            Initialize();
        }

        private void OnKeyPressed(Keyboard.Key key)
        {
            if (key == Keyboard.Key.Escape)
            {
                window.Close();
            }
            else if (key == Keyboard.Key.F5)
            {
                game.Initialize();
            }
        }

        private void OnMouseMoved(Vector2f pos)
        {
            Q1 = new Matrix(RowVec(pos.X, pos.Y));
            w = Matrix.Subtract(Q1, P);
        }
        #endregion
    }
}