using System;
using Emgu.CV;
using Emgu.CV.Structure;

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

        public double Angle { get; }
        public Vector2Int Top => top;
        public Vector2Int Bottom => bottom;
        public int Height => height;
        public int Width => width;

        public bool MergedPiece { get; }

        // WARNING: add the rest of the variables for side comparison
        private float leftDistance, rightDistance, topDistance, botDistance;

        public PuzzlePiece(Vector2Int top, Vector2Int bottom, double angle = 0, bool merged = false)
        {
            this.top = top;
            this.bottom = bottom;

            height = bottom.y - top.y;
            width = bottom.x - top.x;

            leftDistance = rightDistance = topDistance = botDistance = 0;
            MergedPiece = merged;
            Angle = angle;
        }

        public PuzzlePiece(Vector2Int top, Vector2Int bottom,
                float leftDistance, float rightDistance, float topDistance, float botDistance)
        {
            this.top = top;
            this.bottom = bottom;

            height = bottom.y - top.y;
            width = bottom.x - top.x;

            this.leftDistance = leftDistance;
            this.rightDistance = rightDistance;
            this.topDistance = topDistance;
            this.botDistance = botDistance;
            MergedPiece = true;
        }

        /// <summary>
        /// Connects this piece to a given puzzle piece by side,
        /// Connected piece is written into a temporary texture of connected pieces
        /// </summary>
        /// <param name="other"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public unsafe PuzzlePiece Combine(PuzzlePiece other, Side side, Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            // Side references to this piece and not other.
            // if side == top then combine with bottom of other
            int translationX = 0;
            int translationY = 0;

            Vector2Int newTop = Vector2Int.Zero;
            Vector2Int newBottom = Vector2Int.Zero;

            // Create new piece
            switch (side)
            {
                case Side.Top:
                    // Compute new side distances
                    leftDistance += other.leftDistance;
                    rightDistance += other.rightDistance;
                    topDistance = other.topDistance;

                    // Translation to connect the pieces
                    translationY = top.y - other.bottom.y;
                    translationX = top.x - other.top.x;

                    // Calculate new bounds
                    newTop = new Vector2Int(top.x, top.y - other.height);
                    newBottom = bottom;

                    break;

                case Side.Right:
                    topDistance += other.topDistance;
                    botDistance += other.botDistance;
                    rightDistance = other.rightDistance;

                    translationY = top.y - other.top.y;
                    translationX = bottom.x - other.top.x;

                    newTop = top;
                    newBottom = new Vector2Int(bottom.x + other.width, bottom.y);
                    break;

                case Side.Bottom:
                    leftDistance += other.leftDistance;
                    rightDistance += other.rightDistance;
                    botDistance = other.botDistance;

                    translationY = bottom.y - other.top.y;
                    translationX = top.x - other.top.x;

                    newTop = top;
                    newBottom = new Vector2Int(bottom.x, bottom.y + other.height);
                    break;

                case Side.Left:
                    topDistance += other.topDistance;
                    botDistance += other.botDistance;
                    leftDistance = other.leftDistance;

                    translationY = top.y - other.top.y;
                    translationX = bottom.x - other.top.x;

                    newTop = new Vector2Int(top.x - other.width, top.y);
                    newBottom = bottom;
                    break;
                    // newPiece.CalculateSideAverage();
            }

            ImageClass.Translation(img, imgCopy, other, translationX, translationY);
            return new PuzzlePiece(newTop, newBottom, leftDistance, rightDistance, topDistance, botDistance);
        }

        public unsafe void CalculateSideAverage(byte* dataPtrRead, int widthStep)
        {
            for (int x = top.x; x < bottom.x; x++)
            {
                byte* pxTop = (dataPtrRead + 3 * x + widthStep * top.y);
                topDistance += pxTop[0] + pxTop[1] + pxTop[2];

                byte* pxBot = (dataPtrRead + 3 * x + widthStep * bottom.y);
                botDistance += pxBot[0] + pxBot[1] + pxBot[2];
            }

            for (int y = top.y; y < bottom.y; y++)
            {
                byte* pxLeft = (dataPtrRead + 3 * top.x + widthStep * y);
                leftDistance += pxLeft[0] + pxLeft[1] + pxLeft[2];

                byte* pxRight = (dataPtrRead + 3 * top.y + widthStep * y);
                rightDistance += pxRight[0] + pxRight[1] + pxRight[2];
            }
        }

        public static double ImageAngle(Vector2Int rightBottom, Vector2Int bottom)
        {
            Vector2Int diagonal = rightBottom - bottom;

            double r = Math.Atan2(diagonal.y, diagonal.x);
            double angle = r * 180.0 / Math.PI;

            return -angle;
        }

        // Sides comparison functions
        // ETC...

        public PuzzlePiece CompareSide(PuzzlePiece[] others, Side side)
        {
            float min = float.PositiveInfinity;
            int minIndex = -1;
            for (int i = 0; i < others.Length; i++)
            {
                // Protect from comparing with the same puzzle piece
                if (others[i] == this) continue;

                float dist = 0;
                dist = CompareSide(others[i], side);

                if (dist < min)
                {
                    min = dist;
                    minIndex = i;
                }
            }

            return others[minIndex];
        }

        public float CompareSide(PuzzlePiece other, Side side)
        {
            float dist = 0;
            switch (side)
            {
                case Side.Top:
                    dist = Math.Abs(topDistance - other.botDistance);
                    break;

                case Side.Right:
                    dist = Math.Abs(rightDistance - other.leftDistance);
                    break;

                case Side.Bottom:
                    dist = Math.Abs(botDistance - other.topDistance);
                    break;

                case Side.Left:
                    dist = Math.Abs(leftDistance - other.rightDistance);
                    break;
            }

            return dist;
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