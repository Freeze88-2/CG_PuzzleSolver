using System;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

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
        public Image<Bgr, byte> Img { get; private set; }

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

        public unsafe void CreateImage(Image<Bgr, byte> original)
        {
            Img = new Image<Bgr, byte>(Width, Height);
            Rectangle cropArea = new Rectangle(Top.x, Top.y, Width, Height);
            Img.Bitmap = original.Bitmap.Clone(cropArea, original.Bitmap.PixelFormat);
        }

        public unsafe PuzzlePiece Combine(PuzzlePiece other, Side side) 
        {
            Vector2Int newTop = Vector2Int.Zero;
            Vector2Int newBottom = Vector2Int.Zero;
            Vector2Int ogPosition = Vector2Int.Zero;
            Vector2Int otherPosition = Vector2Int.Zero;

            // Create new piece
            switch (side)
            {
                case Side.Top:
                    // Calculate new bounds
                    newTop = new Vector2Int(top.x, top.y - other.height);
                    newBottom = bottom;
                    ogPosition = new Vector2Int(0,  other.height);

                    break;
                case Side.Right:
                    newTop = top;
                    newBottom = new Vector2Int(bottom.x + other.width, bottom.y);
                    otherPosition = new Vector2Int(Width, 0);

                    break;
                case Side.Bottom:
                    newTop = top;
                    newBottom = new Vector2Int(bottom.x, bottom.y + other.height);
                    otherPosition = new Vector2Int(0, height);

                    break;
                case Side.Left:
                    newTop = new Vector2Int(top.x - other.width, top.y);
                    newBottom = bottom;
                    ogPosition = new Vector2Int(other.width, 0);

                    break;
            }

            PuzzlePiece p = new PuzzlePiece(newTop, newBottom);
            
            p.Img = new Image<Bgr, byte>(p.width, p.height);

            p.Img.ROI = new Rectangle(otherPosition.x, otherPosition.y, other.width, other.height);
            other.Img.CopyTo(p.Img);
            p.Img.ROI = new Rectangle(ogPosition.x, ogPosition.y, width, height);
            Img.CopyTo(p.Img);
            
            // Reset region of interest
            p.Img.ROI = new Rectangle(0, 0, p.width, p.height);
            return p;
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
                    translationX = top.x - other.bottom.x;

                    newTop = new Vector2Int(top.x - other.width, top.y);
                    newBottom = bottom;
                    break;
                    // newPiece.CalculateSideAverage();
            }

            ImageClass.Translation(img, imgCopy, other, translationX, translationY);
            return new PuzzlePiece(newTop, newBottom, leftDistance, rightDistance, topDistance, botDistance);
        }

        public static double ImageAngle(Vector2Int rightBottom, Vector2Int bottom)
        {
            Vector2Int diagonal = rightBottom - bottom;

            double r = Math.Atan2(diagonal.y, diagonal.x);
            double angle = r * 180.0 / Math.PI;

            return -angle;
        }

        public static unsafe double Compare(PuzzlePiece a, PuzzlePiece b, Side side) 
        {
            // right -- left
            // top -- bottom
            byte* readA = (byte*)a.Img.MIplImage.imageData.ToPointer();
            byte* readB = (byte*)b.Img.MIplImage.imageData.ToPointer();

            int widthStepA = a.Img.MIplImage.widthStep;
            int widthStepB = b.Img.MIplImage.widthStep;
            
            int nChanA = a.Img.MIplImage.nChannels;
            int nChanB = b.Img.MIplImage.nChannels;

            double distance = 0;
            switch (side)
            {
                case Side.Top:
                    if (a.width != b.width) return float.PositiveInfinity;
                    for (int i = 0; i < a.width; i++)
                    {
                        byte ra = (readA + nChanA * (i))[2];
                        byte ga = (readA + nChanA * (i))[1];
                        byte ba = (readA + nChanA * (i))[0];

                        byte rb = (readB + nChanB * (i) + widthStepB * (b.height - 1))[2];
                        byte gb = (readB + nChanB * (i) + widthStepB * (b.height - 1))[1];
                        byte bb = (readB + nChanB * (i) + widthStepB * (b.height - 1))[0];

                        float sum = (ba - bb) * (ba - bb) + (ga - gb) * (ga - gb) + (ra - rb) * (ra - rb);
                        distance += Math.Sqrt(sum);
                    }
                    break;
                case Side.Right:
                    if (a.height != b.height) return float.PositiveInfinity;
                    for (int i = 0; i < a.height; i++)
                    {
                        byte ra = (readA + nChanA * (a.width - 1) + widthStepA * (i))[2];
                        byte ga = (readA + nChanA * (a.width - 1) + widthStepA * (i))[1];
                        byte ba = (readA + nChanA * (a.width - 1) + widthStepA * (i))[0];

                        byte rb = (readB + widthStepB * (i))[2];
                        byte gb = (readB + widthStepB * (i))[1];
                        byte bb = (readB + widthStepB * (i))[0];

                        float sum = (ba - bb) * (ba - bb) + (ga - gb) * (ga - gb) + (ra - rb) * (ra - rb);
                        distance += Math.Sqrt(sum);
                    }
                    break;
                case Side.Bottom:
                    if (a.width != b.width) return float.PositiveInfinity;
                    for (int i = 0; i < a.width; i++)
                    {
                        byte ra = (readA + nChanA * (i) + widthStepA * (a.height - 1))[2];
                        byte ga = (readA + nChanA * (i) + widthStepA * (a.height - 1))[1];
                        byte ba = (readA + nChanA * (i) + widthStepA * (a.height - 1))[0];

                        byte rb = (readB + nChanB * (i))[2];
                        byte gb = (readB + nChanB * (i))[1];
                        byte bb = (readB + nChanB * (i))[0];

                        float sum = (ba - bb) * (ba - bb) + (ga - gb) * (ga - gb) + (ra - rb) * (ra - rb);
                        distance += Math.Sqrt(sum);
                    }
                    break;
                case Side.Left:
                    if (a.height != b.height) return float.PositiveInfinity;
                    for (int i = 0; i < a.height; i++)
                    {
                        byte ra = (readA + nChanA + widthStepA * (i))[2];
                        byte ga = (readA + nChanA + widthStepA * (i))[1];
                        byte ba = (readA + nChanA + widthStepA * (i))[0];

                        byte rb = (readB + nChanB * (b.width - 1) + widthStepB * (i))[2];
                        byte gb = (readB + nChanB * (b.width - 1) + widthStepB * (i))[1];
                        byte bb = (readB + nChanB * (b.width - 1) + widthStepB * (i))[0];

                        float sum = (ba - bb) * (ba - bb) + (ga - gb) * (ga - gb) + (ra - rb) * (ra - rb);
                        distance += Math.Sqrt(sum);
                    }
                    break;
            }

            return distance;
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
            float dist = float.PositiveInfinity;
            switch (side)
            {
                case Side.Top:
                    dist = Math.Abs(other.botDistance - topDistance);
                    break;

                case Side.Right:
                    dist = Math.Abs(other.leftDistance - rightDistance);
                    break;

                case Side.Bottom:
                    dist = Math.Abs(other.topDistance - botDistance);
                    break;

                case Side.Left:
                    dist = Math.Abs(other.rightDistance - leftDistance);
                    break;
            }

            return dist;
        }

        public bool MatchSide(PuzzlePiece other) 
        {
            return other.width == width || other.height == height;
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