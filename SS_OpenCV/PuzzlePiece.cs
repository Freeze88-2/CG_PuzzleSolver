using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;

namespace CG_OpenCV
{
    // Top, Bottom logic for this class:
    // Given the rectangle:
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
        private Vector2Int _top;
        private Vector2Int _bottom;

        private readonly int _height;
        private readonly int _width;

        public double Angle { get; }
        public Vector2Int Top => _top;
        public Vector2Int Bottom => _bottom;
        public int Height => _height;
        public int Width => _width;
        public Image<Bgr, byte> Img { get; private set; }

        public PuzzlePiece(Vector2Int top, Vector2Int bottom, double angle = 0)
        {
            _top = top;
            _bottom = bottom;

            _height = bottom.y - top.y + 1;
            _width = bottom.x - top.x + 1;

            Angle = angle;
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
                    newTop = new Vector2Int(_top.x, _top.y - other._height);
                    newBottom = _bottom;
                    ogPosition = new Vector2Int(0, other._height);
                    break;

                case Side.Right:
                    newTop = _top;
                    newBottom = new Vector2Int(_bottom.x + other._width, _bottom.y);
                    otherPosition = new Vector2Int(Width, 0);
                    break;

                case Side.Bottom:
                    newTop = _top;
                    newBottom = new Vector2Int(_bottom.x, _bottom.y + other._height);
                    otherPosition = new Vector2Int(0, _height);
                    break;

                case Side.Left:
                    newTop = new Vector2Int(_top.x - other._width, _top.y);
                    newBottom = _bottom;
                    ogPosition = new Vector2Int(other._width, 0);
                    break;
            }

            PuzzlePiece p = new PuzzlePiece(newTop, newBottom);

            p.Img = new Image<Bgr, byte>(p._width, p._height)
            {
                ROI = new Rectangle(otherPosition.x, otherPosition.y, other._width, other._height)
            };
            other.Img.CopyTo(p.Img);
            p.Img.ROI = new Rectangle(ogPosition.x, ogPosition.y, _width, _height);
            Img.CopyTo(p.Img);

            // Reset region of interest
            p.Img.ROI = new Rectangle(0, 0, p._width, p._height);
            return p;
        }

        public static double ImageAngle(Vector2Int rightBottom, Vector2Int bottom)
        {
            Vector2Int diagonal = rightBottom - bottom;

            double r = Math.Atan2(diagonal.y, diagonal.x);
            double angle = r * 180.0 / Math.PI;

            return -Math.Round(angle);
        }

        public static unsafe double Compare(PuzzlePiece a, PuzzlePiece b, Side side)
        {
            // right -- left
            // top -- bottom
            byte* readA = (byte*)a.Img.MIplImage.imageData.ToPointer();
            byte* readB = (byte*)b.Img.MIplImage.imageData.ToPointer();
            int widthStepA = a.Img.MIplImage.widthStep;
            int widthStepB = b.Img.MIplImage.widthStep;

            double distance = 0;
            switch (side)
            {
                case Side.Top:
                    if (a._width != b._width) return float.PositiveInfinity;
                    for (int i = 0; i < a._width; i++)
                    {
                        int ra = (readA + 3 * i)[2];
                        int ga = (readA + 3 * i)[1];
                        int ba = (readA + 3 * i)[0];

                        int rb = (readB + 3 * i + widthStepB * (b._height - 1))[2];
                        int gb = (readB + 3 * i + widthStepB * (b._height - 1))[1];
                        int bb = (readB + 3 * i + widthStepB * (b._height - 1))[0];

                        float sum = (ba - bb) * (ba - bb) + (ga - gb) * (ga - gb) + (ra - rb) * (ra - rb);
                        distance += Math.Sqrt(sum);
                    }
                    break;

                case Side.Right:
                    if (a._height != b._height) return float.PositiveInfinity;
                    for (int i = 0; i < a._height; i++)
                    {
                        int ra = (readA + 3 * (a._width - 1) + widthStepA * i)[2];
                        int ga = (readA + 3 * (a._width - 1) + widthStepA * i)[1];
                        int ba = (readA + 3 * (a._width - 1) + widthStepA * i)[0];

                        int rb = (readB + widthStepB * i)[2];
                        int gb = (readB + widthStepB * i)[1];
                        int bb = (readB + widthStepB * i)[0];

                        float sum = (ba - bb) * (ba - bb) + (ga - gb) * (ga - gb) + (ra - rb) * (ra - rb);
                        distance += Math.Sqrt(sum);
                    }
                    break;

                case Side.Bottom:
                    if (a._width != b._width) return float.PositiveInfinity;
                    for (int i = 0; i < a._width; i++)
                    {
                        int ra = (readA + 3 * i + widthStepA * (a._height - 1))[2];
                        int ga = (readA + 3 * i + widthStepA * (a._height - 1))[1];
                        int ba = (readA + 3 * i + widthStepA * (a._height - 1))[0];

                        int rb = (readB + 3 * i + widthStepB)[2];
                        int gb = (readB + 3 * i + widthStepB)[1];
                        int bb = (readB + 3 * i + widthStepB)[0];

                        float sum = (ba - bb) * (ba - bb) + (ga - gb) * (ga - gb) + (ra - rb) * (ra - rb);
                        distance += Math.Sqrt(sum);
                    }
                    break;

                case Side.Left:
                    if (a._height != b._height) return float.PositiveInfinity;
                    for (int i = 0; i < a._height; i++)
                    {
                        int ra = (readA + widthStepA * i)[2];
                        int ga = (readA + widthStepA * i)[1];
                        int ba = (readA + widthStepA * i)[0];

                        int rb = (readB + 3 * (b._width - 1) + widthStepB * i)[2];
                        int gb = (readB + 3 * (b._width - 1) + widthStepB * i)[1];
                        int bb = (readB + 3 * (b._width - 1) + widthStepB * i)[0];

                        float sum = (ba - bb) * (ba - bb) + (ga - gb) * (ga - gb) + (ra - rb) * (ra - rb);
                        distance += Math.Sqrt(sum);
                    }
                    break;
            }

            return distance;
        }

        public bool MatchSide(PuzzlePiece other)
        {
            return other._width == _width || other._height == _height;
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