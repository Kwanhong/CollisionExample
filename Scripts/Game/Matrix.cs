using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Collections.Generic;
using static Collision.Consts;
using static Collision.Data;

namespace Collision
{
    class Matrix
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public float[][] Data { get; set; }
        public static Matrix Zero
        {
            get => new Matrix(
            new float[][] {
                new float[]{0},
                new float[]{0}
            });
        }

        #region Constructor
        public Matrix(int rows, int cols)
        {
            this.Rows = rows;
            this.Cols = cols;

            Data = new float[Rows][];
            for (int x = 0; x < Rows; x++)
                Data[x] = new float[Cols];

            for (int x = 0; x < Rows; x++)
                for (int y = 0; y < Cols; y++)
                    Data[x][y] = 0;
        }

        public Matrix(float[][] data)
        {
            this.Rows = data.Length;
            this.Cols = data[0].Length;
            this.Data = data;
        }

        public Matrix(float[] data)
        {
            Rows = data.Length;
            Cols = 1;

            this.Data = new float[Rows][];
            for (int x = 0; x < Rows; x++)
                Data[x] = new float[] { data[x] };
        }

        public Matrix(Matrix copy)
        {
            this.Rows = copy.Rows;
            this.Cols = copy.Cols;

            Data = new float[Rows][];
            for (int x = 0; x < Rows; x++)
                Data[x] = new float[Cols];

            for (int x = 0; x < Rows; x++)
                for (int y = 0; y < Cols; y++)
                    this.Data[x][y] = copy.Data[x][y];
        }

        public Matrix(Vector2f vector2)
        {
            Rows = 2;
            Cols = 1;

            Data = new float[Rows][];
            Data[0] = new float[] { vector2.X };
            Data[1] = new float[] { vector2.Y };
        }
        #endregion

        #region NonStatic Methods
        public void Randomize(float min = -1, float max = 1)
        {
            Random rnd = new Random();
            for (int x = 0; x < Rows; x++)
                for (int y = 0; y < Cols; y++)
                    Data[x][y] = Utility.Map((float)rnd.NextDouble(), 0, 1, min, max);
        }

        public void Multiply(float scalar)
        {
            for (int x = 0; x < Rows; x++)
                for (int y = 0; y < Cols; y++)
                    Data[x][y] *= scalar;
        }

        public void Multiply(Matrix other)
        {
            if (this.Cols != other.Cols || this.Rows != other.Rows)
                return;

            for (int x = 0; x < Rows; x++)
                for (int y = 0; y < Cols; y++)
                    Data[x][y] *= other.Data[x][y];
        }

        public void Add(int scalar)
        {
            for (int x = 0; x < Rows; x++)
                for (int y = 0; y < Cols; y++)
                    Data[x][y] += scalar;
        }

        public void Add(Matrix other)
        {
            for (int x = 0; x < Rows; x++)
                for (int y = 0; y < Cols; y++)
                    Data[x][y] += other.Data[x][y];
        }

        public void Map(Func<float, float> func)
        {
            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Cols; y++)
                {
                    var val = this.Data[x][y];
                    this.Data[x][y] = func(val);
                }
            }
        }
        #endregion

        #region QuasiStatic Methods
        public float[] ToArray()
        {
            List<float> list = new List<float>();
            for (int x = 0; x < Rows; x++)
                for (int y = 0; y < Cols; y++)
                    list.Add(Data[x][y]);
            return list.ToArray();
        }

        public Vector2f ToVector2()
        {
            if (Rows < 2 || Cols < 1)
                return Zero.ToVector2();

            Vector2f result = new Vector2f();

            result.X = Data[0][0];
            result.Y = Data[1][0];

            return result;
        }

        public float ToFloat()
        {
            if (Cols != 1 || Rows != 1)
                return 0f;
            return Data[0][0];
        }
        #endregion

        #region Static Methods
        public static Matrix Map(Matrix one, Func<float, float> func)
        {
            var result = new Matrix(one);
            for (int x = 0; x < result.Rows; x++)
            {
                for (int y = 0; y < result.Cols; y++)
                {
                    var val = result.Data[x][y];
                    result.Data[x][y] = func(val);
                }
            }
            return result;
        }

        public static Matrix Multiply(Matrix one, Matrix other)
        {
            if (one.Cols != other.Rows) return Zero;

            var a = one;
            var b = other;
            var result = new Matrix(a.Rows, b.Cols);

            for (int x = 0; x < result.Rows; x++)
            {
                for (int y = 0; y < result.Cols; y++)
                {
                    float sum = 0;
                    for (int i = 0; i < a.Cols; i++)
                        sum += a.Data[x][i] * b.Data[i][y];
                    result.Data[x][y] = sum;
                }
            }

            return result;
        }

        public static Matrix Multiply(Matrix one, float scalar)
        {
            var result = new Matrix(one);

            for (int x = 0; x < result.Rows; x++)
                for (int y = 0; y < result.Cols; y++)
                    result.Data[x][y] *= scalar;

            return result;
        }

        public static Matrix Transpose(Matrix target)
        {
            var result = new Matrix(target.Cols, target.Rows);
            for (int x = 0; x < target.Rows; x++)
                for (int y = 0; y < target.Cols; y++)
                    result.Data[y][x] = target.Data[x][y];

            return result;
        }

        public static Matrix Add(Matrix one, Matrix other)
        {
            if (one.Cols != other.Cols || one.Rows != other.Rows)
                return Zero;

            var result = new Matrix(one.Rows, one.Cols);
            for (int x = 0; x < result.Rows; x++)
                for (int y = 0; y < result.Cols; y++)
                    result.Data[x][y] = one.Data[x][y] + other.Data[x][y];

            return result;
        }

        public static Matrix Subtract(Matrix one, Matrix other)
        {
            if (one.Cols != other.Cols || one.Rows != other.Rows)
                return Zero;

            var result = new Matrix(one.Rows, one.Cols);
            for (int x = 0; x < result.Rows; x++)
                for (int y = 0; y < result.Cols; y++)
                    result.Data[x][y] = one.Data[x][y] - other.Data[x][y];

            return result;
        }
        #endregion

    }
}
