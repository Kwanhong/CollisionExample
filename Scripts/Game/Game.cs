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
        Matrix P0;
        Matrix Q0;
        Matrix w0;
        Matrix u;
        Matrix v;

        Font font;

        public void Initialize()
        {
            P0 = new Matrix(RowVec(150, 200));
            Q0 = new Matrix(RowVec(250, 450));
            u = new Matrix(RowVec(200, -50));
            v = new Matrix(RowVec(150, -150));
            w0 = new Matrix(Matrix.Subtract(P0, Q0));
            font = new Font("Resources/Fonts/NANUMGOTHICLIGHT.TTF");
        }

        private void Update()
        {
        }

        private void Display()
        {
            var acrl = new Color(255, 255, 255, 50);
            var line = new VertexArray(PrimitiveType.Lines, 2);

            line[0] = new Vertex(P0.ToVector2());
            line[1] = new Vertex(Matrix.Add(P0, u).ToVector2());
            window.Draw(line);

            line[0] = new Vertex(Q0.ToVector2());
            line[1] = new Vertex(Matrix.Add(Q0, v).ToVector2());
            window.Draw(line);

            CircleShape point = new CircleShape(3);
            point.Origin = new Vector2f(point.Radius, point.Radius);
            point.OutlineThickness = 0.5f;

            point.FillColor = Color.Blue;
            point.Position = Q0.ToVector2() + v.ToVector2();
            window.Draw(point);

            Matrix P1;
            Matrix Q1;
            float sc;
            float tc;

            var a = Matrix.Multiply(Matrix.Transpose(u), u).ToFloat();
            var b = Matrix.Multiply(Matrix.Transpose(u), v).ToFloat();
            var c = Matrix.Multiply(Matrix.Transpose(v), v).ToFloat();
            var d = Matrix.Multiply(Matrix.Transpose(u), w0).ToFloat();
            var e = Matrix.Multiply(Matrix.Transpose(v), w0).ToFloat();

            var sN = b * e - c * d;
            var sD = a * c - b * b;

            var tN = a * e - b * d;
            var tD = a * c - b * b;

            if (sN < 0f)
            {
                sN = 0f;
                tN = e;
                tD = c;
            }
            else if (sN > sD)
            {
                sN = sD;
                tN = e + b;
                tD = c;
            }
            if (tN < 0f)
            {
                tN = 0f;
            }
            else if (tN > tD)
            {
                tN = tD;
            }

            sc = sN / sD;
            tc = tN / tD;

            var w1 = Matrix.Subtract(Matrix.Add(w0, Matrix.Multiply(u, sc)), Matrix.Multiply(v, tc));
            //P1 = Matrix.Add(P0, Matrix.Multiply(u, sc));
            Q1 = Matrix.Add(Q0, Matrix.Multiply(v, tc));
            P1 = Matrix.Add(Q1, w1);

            var w1Mag = GetMagnitude(w1.ToVector2());
            if (w1Mag < 0.001f) w1 = new Matrix(RowVec(0, 0));

            point.FillColor = Color.Red;
            point.Position = Q1.ToVector2();
            window.Draw(point);

            line[0] = new Vertex(Q1.ToVector2(), acrl);
            line[1] = new Vertex(Matrix.Add(Q1, w1).ToVector2(), acrl);
            window.Draw(line);

            line[0] = new Vertex(Q0.ToVector2(), acrl);
            line[1] = new Vertex(Q1.ToVector2(), acrl);
            window.Draw(line);

            String label = ((int)w1Mag).ToString();
            Text text = new Text(label, font);
            text.CharacterSize = 20;
            text.Origin = new Vector2f(text.GetGlobalBounds().Width / 2, 0);
            text.Position = Q1.ToVector2() + w1.ToVector2() / 2;
            text.Rotation = ToDegree(GetAngle(w1.ToVector2()));
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
            v.Data[0][0] = pos.X - Q0.ToVector2().X;
            v.Data[1][0] = pos.Y - Q0.ToVector2().Y;
        }
        #endregion
    }
}