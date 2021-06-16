using System;

namespace CG_OpenCV
{
    /// <summary>
    /// Pixel position struct
    /// Some functions to work with the positions aswell
    /// </summary>
    internal struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x = default, int y = default)
        {
            this.x = x;
            this.y = y;
        }

        // Distance between 2 points
        public static double Distance(Vector2Int a, Vector2Int b)
        {
            return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
        }

        public static Vector2Int operator +(Vector2Int a, Vector2Int b) => new Vector2Int(a.x + b.x, a.y + b.y);

        public static Vector2Int operator -(Vector2Int a, Vector2Int b) => new Vector2Int(a.x - b.x, a.y - b.y);
    }
}