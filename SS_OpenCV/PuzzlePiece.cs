﻿using System;
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

        // WARNING: add the rest of the variables for side comparison

        public PuzzlePiece(Vector2Int top, Vector2Int bottom) 
        {
            this.top = top;
            this.bottom = bottom;

            height = bottom.y - top.y;
            width = bottom.x - top.x;
        }

        public double ImageAngle() 
        {
            Vector2Int diagonal = top - bottom;

            double r = Math.Atan2(diagonal.y, diagonal.x);
            double angle = r * (180.0 / Math.PI);

            return angle - 45;
        }

        // Sides comparison functions
        // ETC...
    }
}
