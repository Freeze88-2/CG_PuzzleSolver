using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    class PuzzlePiece
    {
        private Vector2Int top, bottom;
        private int height;
        private int width;

        public Vector2Int Top => top;
        public Vector2Int Bottom => bottom;
        public int Height => height;
        public int Width => width;

        // WARNING: add the rest of the variables for side comparison

        public PuzzlePiece(Vector2Int top, Vector2Int bottom) 
        {
            this.top = top;
            this.bottom = bottom;

            height = bottom.y - top.y;
            width = bottom.x - top.x;
        }

        public double ImageAngle(Vector2Int rightBottom) 
        {
            Vector2Int diagonal = top - bottom;
            Vector2Int diagonal2 = rightBottom - bottom;

            double r = Math.Atan2(diagonal.y, diagonal.x);
            double r2 = Math.Atan2(diagonal2.y, diagonal2.x);
            double angle = r * 180.0 / Math.PI;
            double angle2 = r2 * 180.0 / Math.PI;

            return -angle2;
        }

        // Sides comparison functions
        // ETC...
    }
}
