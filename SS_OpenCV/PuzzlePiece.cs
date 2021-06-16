using System;

namespace CG_OpenCV
{
    // Top, Bottom logic for this class:
    //  Given the rectanle:
    // Top vertice
    // !_____________
    // |             |
    // |             |
    // |             |
    // |             |
    //  _____________
    //               !
    //               Bottom Vertice
    //
    /// <summary>
    /// Defines a single puzzle piece
    /// </summary>
    internal class PuzzlePiece
    {
        private Vector2Int top, bottom;
        private int height;
        private int width;

        public Vector2Int Top => top;
        public Vector2Int Bottom => bottom;
        public int Height => height;
        public int Width => width;

        // WARNING: add the rest of the variables for side comparison
        private float leftDistance, rightDistance, topDistance, botDistance;

        private float compareTreshold;

        public PuzzlePiece(Vector2Int top, Vector2Int bottom)
        {
            this.top = top;
            this.bottom = bottom;

            height = bottom.y - top.y;
            width = bottom.x - top.x;

            compareTreshold = float.PositiveInfinity;
        }

        public static double ImageAngle(Vector2Int rightBottom, Vector2Int bottom)
        {
            Vector2Int diagonal2 = rightBottom - bottom;

            double r2 = Math.Atan2(diagonal2.y, diagonal2.x);
            double angle2 = r2 * 180.0 / Math.PI;

            return -angle2;
        }

        // Sides comparison functions
        // ETC...

        public float CompareSide(PuzzlePiece other, Side side)
        {
            switch (side)
            {
                case Side.Top:
                    break;

                case Side.Right:
                    break;

                case Side.Bottom:
                    break;

                case Side.Left:
                    break;
            }
            return 0;
        }
    }

    public enum Side
    {
        Top,
        Right,
        Bottom,
        Left
    }
}