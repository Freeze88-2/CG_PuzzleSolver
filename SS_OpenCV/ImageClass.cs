﻿using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CG_OpenCV
{
    internal class ImageClass
    {
        /// <summary>
        /// Image Negative using EmguCV library
        /// Slower method
        /// </summary>
        /// <param name="img">Image</param>
        public static void NegativeOld(Image<Bgr, byte> img)
        {
            int x, y;

            Bgr aux;
            for (y = 0; y < img.Height; y++)
            {
                for (x = 0; x < img.Width; x++)
                {
                    // acesso directo : mais lento
                    aux = img[y, x];
                    img[y, x] = new Bgr(255 - aux.Blue, 255 - aux.Green, 255 - aux.Red);
                }
            }
        }

        /// <summary>
        /// Negative direct acess
        /// </summary>
        /// <param name="img">Image</param>
        public static void Negative(Image<Bgr, byte> img)
        {
            int x, y;
            MIplImage m = img.MIplImage;
            int padding = m.widthStep - m.nChannels * m.width;

            unsafe
            {
                // optains pointer
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                for (y = 0; y < m.height; y++)
                {
                    for (x = 0; x < m.width; x++)
                    {
                        dataPtr[0] = (byte)(255 - dataPtr[0]);
                        dataPtr[1] = (byte)(255 - dataPtr[1]);
                        dataPtr[2] = (byte)(255 - dataPtr[2]);

                        dataPtr += m.nChannels;
                    }
                    dataPtr += padding;
                }
            }
        }

        /// <summary>
        /// Gray direct acess
        /// </summary>
        /// <param name="img">Image</param>
        public static void BrightContrast(Image<Bgr, byte> img, int bright, double contrast)
        {
            int x, y;
            MIplImage m = img.MIplImage;
            int padding = m.widthStep - m.nChannels * m.width;

            unsafe
            {
                // optains pointer
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                for (y = 0; y < m.height; y++)
                {
                    for (x = 0; x < m.width; x++)
                    {
                        dataPtr[0] = (byte)Math.Max(Math.Min(Math.Round(contrast * dataPtr[0] + bright), 255), 0);
                        dataPtr[1] = (byte)Math.Max(Math.Min(Math.Round(contrast * dataPtr[1] + bright), 255), 0);
                        dataPtr[2] = (byte)Math.Max(Math.Min(Math.Round(contrast * dataPtr[2] + bright), 255), 0);

                        dataPtr += m.nChannels;
                    }
                    dataPtr += padding;
                }
            }
        }

        /// <summary>
        /// Translates the image direct acess
        /// </summary>
        /// <param name="img">Image</param>
        public static void Translation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int dx, int dy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrRead = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrWrite = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int heigh = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;
                byte red, green, blue;
                int x0, y0;

                for (int y = 0; y < heigh; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        x0 = x - dx;
                        y0 = y - dy;

                        if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < heigh)
                        {
                            red = (dataPtrRead + nChan * x0 + widthStep * y0)[2];
                            green = (dataPtrRead + nChan * x0 + widthStep * y0)[1];
                            blue = (dataPtrRead + nChan * x0 + widthStep * y0)[0];
                        }
                        else
                        {
                            red = 0;
                            green = 0;
                            blue = 0;
                        }

                        (dataPtrWrite + nChan * x + widthStep * y)[2] = red;
                        (dataPtrWrite + nChan * x + widthStep * y)[1] = green;
                        (dataPtrWrite + nChan * x + widthStep * y)[0] = blue;
                    }
                }
            }
        }

        /// <summary>
        /// Scales the image direct acess
        /// </summary>
        /// <param name="img">Image</param>
        public static void Scale(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrRead = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrWrite = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int heigh = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;
                byte red, green, blue;
                int x0, y0;

                for (int y = 0; y < heigh; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        x0 = (int)Math.Round(x / scaleFactor);
                        y0 = (int)Math.Round(y / scaleFactor);

                        if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < heigh)
                        {
                            red = (dataPtrRead + nChan * x0 + widthStep * y0)[2];
                            green = (dataPtrRead + nChan * x0 + widthStep * y0)[1];
                            blue = (dataPtrRead + nChan * x0 + widthStep * y0)[0];
                        }
                        else
                        {
                            red = 0;
                            green = 0;
                            blue = 0;
                        }

                        (dataPtrWrite + nChan * x + widthStep * y)[2] = red;
                        (dataPtrWrite + nChan * x + widthStep * y)[1] = green;
                        (dataPtrWrite + nChan * x + widthStep * y)[0] = blue;
                    }
                }
            }
        }

        /// <summary>
        /// Scales the image where the center is a given point direct acess
        /// </summary>
        /// <param name="img">Image</param>
        public static void Scale_point_xy(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor, int centerX, int centerY)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrRead = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrWrite = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int heigh = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;
                byte red, green, blue;
                int x0, y0;

                for (int y = 0; y < heigh; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        x0 = (int)Math.Round(((x - width / 2) / scaleFactor) + centerX);
                        y0 = (int)Math.Round(((y - heigh / 2) / scaleFactor) + centerY);

                        if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < heigh)
                        {
                            red = (dataPtrRead + nChan * x0 + widthStep * y0)[2];
                            green = (dataPtrRead + nChan * x0 + widthStep * y0)[1];
                            blue = (dataPtrRead + nChan * x0 + widthStep * y0)[0];
                        }
                        else
                        {
                            red = 0;
                            green = 0;
                            blue = 0;
                        }

                        (dataPtrWrite + nChan * x + widthStep * y)[2] = red;
                        (dataPtrWrite + nChan * x + widthStep * y)[1] = green;
                        (dataPtrWrite + nChan * x + widthStep * y)[0] = blue;
                    }
                }
            }
        }

        /// <summary>
        /// Non uniform matrix median filter direct acess
        /// </summary>
        /// <param name="img">Image</param>
        public static void NonUniform(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float[,] matrix, float matrixWeight)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrRead = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrWrite = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int heigh = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;

                (dataPtrWrite)[0] = (byte)Math.Min(Math.Max(Math.Round((
                        dataPtrRead[0] * matrix[0, 0] +
                        dataPtrRead[0] * matrix[0, 1] +
                        dataPtrRead[0] * matrix[1, 0] +
                        dataPtrRead[0] * matrix[1, 1] +

                        ((dataPtrRead + nChan)[0] * matrix[2, 0]) +
                        ((dataPtrRead + nChan)[0] * matrix[2, 1]) +

                        ((dataPtrRead + widthStep)[0] * matrix[0, 2]) +
                        ((dataPtrRead + widthStep)[0] * matrix[1, 2]) +

                        ((dataPtrRead + nChan + widthStep)[0] * matrix[2, 2]))
                        / matrixWeight), 0), 255);

                (dataPtrWrite)[1] = (byte)Math.Min(Math.Max(Math.Round((
                        dataPtrRead[1] * matrix[0, 0] +
                        dataPtrRead[1] * matrix[0, 1] +
                        dataPtrRead[1] * matrix[1, 0] +
                        dataPtrRead[1] * matrix[1, 1] +

                        ((dataPtrRead + nChan)[1] * matrix[2, 0]) +
                        ((dataPtrRead + nChan)[1] * matrix[2, 1]) +

                        ((dataPtrRead + widthStep)[1] * matrix[0, 2]) +
                        ((dataPtrRead + widthStep)[1] * matrix[1, 2]) +

                        ((dataPtrRead + nChan + widthStep)[1] * matrix[2, 2]))
                        / matrixWeight), 0), 255);

                (dataPtrWrite)[2] = (byte)Math.Min(Math.Max(Math.Round((
                        dataPtrRead[2] * matrix[0, 0] +
                        dataPtrRead[2] * matrix[0, 1] +
                        dataPtrRead[2] * matrix[1, 0] +
                        dataPtrRead[2] * matrix[1, 1] +

                        ((dataPtrRead + nChan)[2] * matrix[2, 0]) +
                        ((dataPtrRead + nChan)[2] * matrix[2, 1]) +

                        ((dataPtrRead + widthStep)[2] * matrix[0, 2]) +
                        ((dataPtrRead + widthStep)[2] * matrix[1, 2]) +

                        ((dataPtrRead + nChan + widthStep)[2] * matrix[2, 2]))
                        / matrixWeight), 0), 255);

                (dataPtrWrite + nChan * (width - 1) + widthStep * (heigh - 1))[0] =
                   (byte)Math.Min(Math.Max(Math.Round((
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[0] * matrix[2, 2]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[0] * matrix[2, 1]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[0] * matrix[1, 2]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[0] * matrix[1, 1]) +

                        ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 2))[0] * matrix[1, 0]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 2))[0] * matrix[2, 0]) +

                        ((dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 1))[0] * matrix[0, 1]) +
                        ((dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 1))[0] * matrix[0, 2]) +

                        ((dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 2))[0] * matrix[0, 0]))
                        / matrixWeight), 0), 255);

                (dataPtrWrite + nChan * (width - 1) + widthStep * (heigh - 1))[1] =
                   (byte)Math.Min(Math.Max(Math.Round((
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[1] * matrix[2, 2]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[1] * matrix[2, 1]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[1] * matrix[1, 2]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[1] * matrix[1, 1]) +

                        ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 2))[1] * matrix[1, 0]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 2))[1] * matrix[2, 0]) +

                        ((dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 1))[1] * matrix[0, 1]) +
                        ((dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 1))[1] * matrix[0, 2]) +

                        ((dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 2))[1] * matrix[0, 0]))
                        / matrixWeight), 0), 255);

                (dataPtrWrite + nChan * (width - 1) + widthStep * (heigh - 1))[2] =
                    (byte)Math.Min(Math.Max(Math.Round((
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[2] * matrix[2, 2]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[2] * matrix[2, 1]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[2] * matrix[1, 2]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[2] * matrix[1, 1]) +

                        ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 2))[2] * matrix[1, 0]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 2))[2] * matrix[2, 0]) +

                        ((dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 1))[2] * matrix[0, 1]) +
                        ((dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 1))[2] * matrix[0, 2]) +

                        ((dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 2))[2] * matrix[0, 0]))
                        / matrixWeight), 0), 255);

                (dataPtrWrite + nChan * (width - 1))[0] = (byte)Math.Min(Math.Max(Math.Round((
                    ((dataPtrRead + nChan * (width - 1))[0] * matrix[1, 0]) +
                    ((dataPtrRead + nChan * (width - 1))[0] * matrix[2, 0]) +
                    ((dataPtrRead + nChan * (width - 1))[0] * matrix[2, 1]) +
                    ((dataPtrRead + nChan * (width - 1))[0] * matrix[1, 1]) +

                    ((dataPtrRead + nChan * (width - 3))[0] * matrix[0, 0]) +
                    ((dataPtrRead + nChan * (width - 3))[0] * matrix[0, 1]) +

                    ((dataPtrRead + nChan * (width - 1) + widthStep)[0] * matrix[1, 2]) +
                    ((dataPtrRead + nChan * (width - 1) + widthStep)[0] * matrix[2, 2]) +

                    ((dataPtrRead + nChan * (width - 3) + widthStep)[0] * matrix[0, 2]))
                    / matrixWeight), 0), 255);

                (dataPtrWrite + nChan * (width - 1))[1] = (byte)Math.Min(Math.Max(Math.Round((
                    ((dataPtrRead + nChan * (width - 1))[1] * matrix[1, 0]) +
                    ((dataPtrRead + nChan * (width - 1))[1] * matrix[2, 0]) +
                    ((dataPtrRead + nChan * (width - 1))[1] * matrix[2, 1]) +
                    ((dataPtrRead + nChan * (width - 1))[1] * matrix[1, 1]) +

                    ((dataPtrRead + nChan * (width - 3))[1] * matrix[0, 0]) +
                    ((dataPtrRead + nChan * (width - 3))[1] * matrix[0, 1]) +

                    ((dataPtrRead + nChan * (width - 1) + widthStep)[1] * matrix[1, 2]) +
                    ((dataPtrRead + nChan * (width - 1) + widthStep)[1] * matrix[2, 2]) +

                    ((dataPtrRead + nChan * (width - 3) + widthStep)[1] * matrix[0, 2]))
                    / matrixWeight), 0), 255);

                (dataPtrWrite + nChan * (width - 1))[2] = (byte)Math.Min(Math.Max(Math.Round((
                    ((dataPtrRead + nChan * (width - 1))[2] * matrix[1, 0]) +
                    ((dataPtrRead + nChan * (width - 1))[2] * matrix[2, 0]) +
                    ((dataPtrRead + nChan * (width - 1))[2] * matrix[2, 1]) +
                    ((dataPtrRead + nChan * (width - 1))[2] * matrix[1, 1]) +

                    ((dataPtrRead + nChan * (width - 3))[2] * matrix[0, 0]) +
                    ((dataPtrRead + nChan * (width - 3))[2] * matrix[0, 1]) +

                    ((dataPtrRead + nChan * (width - 1) + widthStep)[2] * matrix[1, 2]) +
                    ((dataPtrRead + nChan * (width - 1) + widthStep)[2] * matrix[2, 2]) +

                    ((dataPtrRead + nChan * (width - 3) + widthStep)[2] * matrix[0, 2]))
                    / matrixWeight), 0), 255);

                (dataPtrWrite + widthStep * (heigh - 1))[0] = (byte)Math.Min(Math.Max(Math.Round((
                    ((dataPtrRead + widthStep * (heigh - 1))[0] * matrix[0, 2]) +
                    ((dataPtrRead + widthStep * (heigh - 1))[0] * matrix[0, 1]) +
                    ((dataPtrRead + widthStep * (heigh - 1))[0] * matrix[1, 2]) +
                    ((dataPtrRead + widthStep * (heigh - 1))[0] * matrix[1, 1]) +

                    ((dataPtrRead + widthStep * (heigh - 2))[0] * matrix[0, 0]) +
                    ((dataPtrRead + widthStep * (heigh - 2))[0] * matrix[1, 0]) +

                    ((dataPtrRead + nChan + widthStep * (heigh - 1))[0] * matrix[2, 2]) +
                    ((dataPtrRead + nChan + widthStep * (heigh - 1))[0] * matrix[2, 1]) +

                    ((dataPtrRead + nChan + widthStep * (heigh - 2))[0] * matrix[2, 0]))
                    / matrixWeight), 0), 255);

                (dataPtrWrite + widthStep * (heigh - 1))[1] = (byte)Math.Min(Math.Max(Math.Round((
                    ((dataPtrRead + widthStep * (heigh - 1))[1] * matrix[0, 2]) +
                    ((dataPtrRead + widthStep * (heigh - 1))[1] * matrix[0, 1]) +
                    ((dataPtrRead + widthStep * (heigh - 1))[1] * matrix[1, 2]) +
                    ((dataPtrRead + widthStep * (heigh - 1))[1] * matrix[1, 1]) +

                    ((dataPtrRead + widthStep * (heigh - 2))[1] * matrix[0, 0]) +
                    ((dataPtrRead + widthStep * (heigh - 2))[1] * matrix[1, 0]) +

                    ((dataPtrRead + nChan + widthStep * (heigh - 1))[1] * matrix[2, 2]) +
                    ((dataPtrRead + nChan + widthStep * (heigh - 1))[1] * matrix[2, 1]) +

                    ((dataPtrRead + nChan + widthStep * (heigh - 2))[1] * matrix[2, 0]))
                    / matrixWeight), 0), 255);

                (dataPtrWrite + widthStep * (heigh - 1))[2] = (byte)Math.Min(Math.Max(Math.Round((
                    ((dataPtrRead + widthStep * (heigh - 1))[2] * matrix[0, 2]) +
                    ((dataPtrRead + widthStep * (heigh - 1))[2] * matrix[0, 1]) +
                    ((dataPtrRead + widthStep * (heigh - 1))[2] * matrix[1, 2]) +
                    ((dataPtrRead + widthStep * (heigh - 1))[2] * matrix[1, 1]) +

                    ((dataPtrRead + widthStep * (heigh - 2))[2] * matrix[0, 0]) +
                    ((dataPtrRead + widthStep * (heigh - 2))[2] * matrix[1, 0]) +

                    ((dataPtrRead + nChan + widthStep * (heigh - 1))[2] * matrix[2, 2]) +
                    ((dataPtrRead + nChan + widthStep * (heigh - 1))[2] * matrix[2, 1]) +

                    ((dataPtrRead + nChan + widthStep * (heigh - 2))[2] * matrix[2, 0]))
                    / matrixWeight), 0), 255);

                for (int x = 1; x < width - 1; x++)
                {
                    (dataPtrWrite + nChan * x)[0] = (byte)Math.Min(Math.Max(Math.Round((((dataPtrRead + nChan * (x - 1))[0] * matrix[0, 0]) + ((dataPtrRead + nChan * x)[0] * matrix[1, 0]) + ((dataPtrRead + nChan * (x + 1))[0] * matrix[2, 0]) +
                                                           ((dataPtrRead + nChan * (x - 1))[0] * matrix[0, 1]) + ((dataPtrRead + nChan * x)[0] * matrix[1, 1]) + ((dataPtrRead + nChan * (x + 1))[0] * matrix[2, 1]) +
                                                           ((dataPtrRead + nChan * (x - 1) + widthStep * 1)[0] * matrix[0, 2]) + ((dataPtrRead + nChan * x + widthStep * 1)[0] * matrix[1, 2]) + ((dataPtrRead + nChan * (x + 1) + widthStep * 1)[0] * matrix[2, 2])) / matrixWeight), 0), 255);

                    (dataPtrWrite + nChan * x)[1] = (byte)Math.Min(Math.Max(Math.Round((((dataPtrRead + nChan * (x - 1))[1] * matrix[0, 0]) + ((dataPtrRead + nChan * x)[1] * matrix[1, 0]) + ((dataPtrRead + nChan * (x + 1))[1] * matrix[2, 0]) +
                                                           ((dataPtrRead + nChan * (x - 1))[1] * matrix[0, 1]) + ((dataPtrRead + nChan * x)[1] * matrix[1, 1]) + ((dataPtrRead + nChan * (x + 1))[1] * matrix[2, 1]) +
                                                           ((dataPtrRead + nChan * (x - 1) + widthStep * 1)[1] * matrix[0, 2]) + ((dataPtrRead + nChan * x + widthStep * 1)[1] * matrix[1, 2]) + ((dataPtrRead + nChan * (x + 1) + widthStep * 1)[1] * matrix[2, 2])) / matrixWeight), 0), 255);

                    (dataPtrWrite + nChan * x)[2] = (byte)Math.Min(Math.Max(Math.Round((((dataPtrRead + nChan * (x - 1))[2] * matrix[0, 0]) + ((dataPtrRead + nChan * x)[2] * matrix[1, 0]) + ((dataPtrRead + nChan * (x + 1))[2] * matrix[2, 0]) +
                                                           ((dataPtrRead + nChan * (x - 1))[2] * matrix[0, 1]) + ((dataPtrRead + nChan * x)[2] * matrix[1, 1]) + ((dataPtrRead + nChan * (x + 1))[2] * matrix[2, 1]) +
                                                           ((dataPtrRead + nChan * (x - 1) + widthStep * 1)[2] * matrix[0, 2]) + ((dataPtrRead + nChan * x + widthStep * 1)[2] * matrix[1, 2]) + ((dataPtrRead + nChan * (x + 1) + widthStep * 1)[2] * matrix[2, 2])) / matrixWeight), 0), 255);

                    (dataPtrWrite + nChan * x + widthStep * (heigh - 1))[0] = (byte)Math.Min(Math.Max(Math.Round((((dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 1))[0] * matrix[0, 2]) + ((dataPtrRead + nChan * x + widthStep * (heigh - 1))[0] * matrix[1, 2]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 1))[0] * matrix[2, 2]) +
                                                                                     ((dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 1))[0] * matrix[0, 1]) + ((dataPtrRead + nChan * x + widthStep * (heigh - 1))[0] * matrix[1, 1]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 1))[0] * matrix[2, 1]) +
                                                                                     ((dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 2))[0] * matrix[0, 0]) + ((dataPtrRead + nChan * x + widthStep * (heigh - 2))[0] * matrix[1, 0]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 2))[0] * matrix[2, 0])) / matrixWeight), 0), 255);

                    (dataPtrWrite + nChan * x + widthStep * (heigh - 1))[1] = (byte)Math.Min(Math.Max(Math.Round((((dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 1))[1] * matrix[0, 2]) + ((dataPtrRead + nChan * x + widthStep * (heigh - 1))[1] * matrix[1, 2]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 1))[1] * matrix[2, 2]) +
                                                                                     ((dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 1))[1] * matrix[0, 1]) + ((dataPtrRead + nChan * x + widthStep * (heigh - 1))[1] * matrix[1, 1]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 1))[1] * matrix[2, 1]) +
                                                                                     ((dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 2))[1] * matrix[0, 0]) + ((dataPtrRead + nChan * x + widthStep * (heigh - 2))[1] * matrix[1, 0]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 2))[1] * matrix[2, 0])) / matrixWeight), 0), 255);

                    (dataPtrWrite + nChan * x + widthStep * (heigh - 1))[2] = (byte)Math.Min(Math.Max(Math.Round((((dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 1))[2] * matrix[0, 2]) + ((dataPtrRead + nChan * x + widthStep * (heigh - 1))[2] * matrix[1, 2]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 1))[2] * matrix[2, 2]) +
                                                                                     ((dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 1))[2] * matrix[0, 1]) + ((dataPtrRead + nChan * x + widthStep * (heigh - 1))[2] * matrix[1, 1]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 1))[2] * matrix[2, 1]) +
                                                                                     ((dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 2))[2] * matrix[0, 0]) + ((dataPtrRead + nChan * x + widthStep * (heigh - 2))[2] * matrix[1, 0]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 2))[2] * matrix[2, 0])) / matrixWeight), 0), 255);
                }
                for (int y = 1; y < heigh - 1; y++)
                {
                    (dataPtrWrite + widthStep * y)[0] = (byte)Math.Min(Math.Max(Math.Round((((dataPtrRead + widthStep * (y - 1))[0] * matrix[0, 0]) + ((dataPtrRead + widthStep * (y - 1))[0] * matrix[1, 0]) + ((dataPtrRead + nChan + widthStep * (y - 1))[0] * matrix[2, 0]) +
                                                                                             ((dataPtrRead + widthStep * (y))[0] * matrix[0, 1]) + ((dataPtrRead + widthStep * (y))[0] * matrix[1, 1]) + ((dataPtrRead + nChan + widthStep * (y))[0] * matrix[2, 1]) +
                                                                                             ((dataPtrRead + widthStep * (y + 1))[0] * matrix[0, 2]) + ((dataPtrRead + widthStep * (y + 1))[0] * matrix[1, 2]) + ((dataPtrRead + nChan + widthStep * (y + 1))[0] * matrix[2, 2])) / matrixWeight), 0), 255);

                    (dataPtrWrite + widthStep * y)[1] = (byte)Math.Min(Math.Max(Math.Round((((dataPtrRead + widthStep * (y - 1))[1] * matrix[0, 0]) + ((dataPtrRead + widthStep * (y - 1))[1] * matrix[1, 0]) + ((dataPtrRead + nChan + widthStep * (y - 1))[1] * matrix[2, 0]) +
                                                                                             ((dataPtrRead + widthStep * (y))[1] * matrix[0, 1]) + ((dataPtrRead + widthStep * (y))[1] * matrix[1, 1]) + ((dataPtrRead + nChan + widthStep * (y))[1] * matrix[2, 1]) +
                                                                                             ((dataPtrRead + widthStep * (y + 1))[1] * matrix[0, 2]) + ((dataPtrRead + widthStep * (y + 1))[1] * matrix[1, 2]) + ((dataPtrRead + nChan + widthStep * (y + 1))[1] * matrix[2, 2])) / matrixWeight), 0), 255);

                    (dataPtrWrite + widthStep * y)[2] = (byte)Math.Min(Math.Max(Math.Round((((dataPtrRead + widthStep * (y - 1))[2] * matrix[0, 0]) + ((dataPtrRead + widthStep * (y - 1))[2] * matrix[1, 0]) + ((dataPtrRead + nChan + widthStep * (y - 1))[2] * matrix[2, 0]) +
                                                                                             ((dataPtrRead + widthStep * (y))[2] * matrix[0, 1]) + ((dataPtrRead + widthStep * (y))[2] * matrix[1, 1]) + ((dataPtrRead + nChan + widthStep * (y))[2] * matrix[2, 1]) +
                                                                                             ((dataPtrRead + widthStep * (y + 1))[2] * matrix[0, 2]) + ((dataPtrRead + widthStep * (y + 1))[2] * matrix[1, 2]) + ((dataPtrRead + nChan + widthStep * (y + 1))[2] * matrix[2, 2])) / matrixWeight), 0), 255);

                    (dataPtrWrite + nChan * (width - 1) + widthStep * y)[0] = (byte)Math.Min(Math.Max(Math.Round((((dataPtrRead + nChan * (width - 1) + widthStep * (y - 1))[0] * matrix[2, 0]) + ((dataPtrRead + nChan * (width - 1) + widthStep * (y - 1))[0] * matrix[1, 0]) + ((dataPtrRead + nChan * (width - 3) + nChan + widthStep * (y - 1))[0] * matrix[0, 0]) +
                                                                                                                   ((dataPtrRead + nChan * (width - 1) + widthStep * (y))[0] * matrix[2, 1]) + ((dataPtrRead + nChan * (width - 1) + widthStep * (y))[0] * matrix[1, 1]) + ((dataPtrRead + nChan * (width - 3) + nChan + widthStep * (y))[0] * matrix[0, 1]) +
                                                                                                                   ((dataPtrRead + nChan * (width - 1) + widthStep * (y + 1))[0] * matrix[2, 2]) + ((dataPtrRead + nChan * (width - 1) + widthStep * (y + 1))[0] * matrix[1, 2]) + ((dataPtrRead + nChan * (width - 3) + nChan + widthStep * (y + 1))[0] * matrix[0, 2])) / matrixWeight), 0), 255);

                    (dataPtrWrite + nChan * (width - 1) + widthStep * y)[1] = (byte)Math.Min(Math.Max(Math.Round((((dataPtrRead + nChan * (width - 1) + widthStep * (y - 1))[1] * matrix[2, 0]) + ((dataPtrRead + nChan * (width - 1) + widthStep * (y - 1))[1] * matrix[1, 0]) + ((dataPtrRead + nChan * (width - 3) + nChan + widthStep * (y - 1))[1] * matrix[0, 0]) +
                                                                                                                   ((dataPtrRead + nChan * (width - 1) + widthStep * (y))[1] * matrix[2, 1]) + ((dataPtrRead + nChan * (width - 1) + widthStep * (y))[1] * matrix[1, 1]) + ((dataPtrRead + nChan * (width - 3) + nChan + widthStep * (y))[1] * matrix[0, 1]) +
                                                                                                                   ((dataPtrRead + nChan * (width - 1) + widthStep * (y + 1))[1] * matrix[2, 2]) + ((dataPtrRead + nChan * (width - 1) + widthStep * (y + 1))[1] * matrix[1, 2]) + ((dataPtrRead + nChan * (width - 3) + nChan + widthStep * (y + 1))[1] * matrix[0, 2])) / matrixWeight), 0), 255);

                    (dataPtrWrite + nChan * (width - 1) + widthStep * y)[2] = (byte)Math.Min(Math.Max(Math.Round((((dataPtrRead + nChan * (width - 1) + widthStep * (y - 1))[2] * matrix[2, 0]) + ((dataPtrRead + nChan * (width - 1) + widthStep * (y - 1))[2] * matrix[1, 0]) + ((dataPtrRead + nChan * (width - 3) + nChan + widthStep * (y - 1))[2] * matrix[0, 0]) +
                                                                                                                   ((dataPtrRead + nChan * (width - 1) + widthStep * (y))[2] * matrix[2, 1]) + ((dataPtrRead + nChan * (width - 1) + widthStep * (y))[2] * matrix[1, 1]) + ((dataPtrRead + nChan * (width - 3) + nChan + widthStep * (y))[2] * matrix[0, 1]) +
                                                                                                                   ((dataPtrRead + nChan * (width - 1) + widthStep * (y + 1))[2] * matrix[2, 2]) + ((dataPtrRead + nChan * (width - 1) + widthStep * (y + 1))[2] * matrix[1, 2]) + ((dataPtrRead + nChan * (width - 3) + nChan + widthStep * (y + 1))[2] * matrix[0, 2])) / matrixWeight), 0), 255);
                }

                for (int y = 1; y < heigh - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        (dataPtrWrite + nChan * x + widthStep * y)[0] = (byte)Math.Min(Math.Max(Math.Round((((dataPtrRead + nChan * (x - 1) + widthStep * (y - 1))[0] * matrix[0, 0]) + ((dataPtrRead + nChan * (x) + widthStep * (y - 1))[0] * matrix[1, 0]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (y - 1))[0] * matrix[2, 0]) +
                                                                                           ((dataPtrRead + nChan * (x - 1) + widthStep * (y))[0] * matrix[0, 1]) + ((dataPtrRead + nChan * (x) + widthStep * (y))[0] * matrix[1, 1]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (y))[0] * matrix[2, 1]) +
                                                                                           ((dataPtrRead + nChan * (x - 1) + widthStep * (y + 1))[0] * matrix[0, 2]) + ((dataPtrRead + nChan * (x) + widthStep * (y + 1))[0] * matrix[1, 2]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (y + 1))[0] * matrix[2, 2])) / matrixWeight), 0), 255);

                        (dataPtrWrite + nChan * x + widthStep * y)[1] = (byte)Math.Min(Math.Max(Math.Round((((dataPtrRead + nChan * (x - 1) + widthStep * (y - 1))[1] * matrix[0, 0]) + ((dataPtrRead + nChan * (x) + widthStep * (y - 1))[1] * matrix[1, 0]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (y - 1))[1] * matrix[2, 0]) +
                                                                                           ((dataPtrRead + nChan * (x - 1) + widthStep * (y))[1] * matrix[0, 1]) + ((dataPtrRead + nChan * (x) + widthStep * (y))[1] * matrix[1, 1]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (y))[1] * matrix[2, 1]) +
                                                                                           ((dataPtrRead + nChan * (x - 1) + widthStep * (y + 1))[1] * matrix[0, 2]) + ((dataPtrRead + nChan * (x) + widthStep * (y + 1))[1] * matrix[1, 2]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (y + 1))[1] * matrix[2, 2])) / matrixWeight), 0), 255);

                        (dataPtrWrite + nChan * x + widthStep * y)[2] = (byte)Math.Min(Math.Max(Math.Round((((dataPtrRead + nChan * (x - 1) + widthStep * (y - 1))[2] * matrix[0, 0]) + ((dataPtrRead + nChan * (x) + widthStep * (y - 1))[2] * matrix[1, 0]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (y - 1))[2] * matrix[2, 0]) +
                                                                                           ((dataPtrRead + nChan * (x - 1) + widthStep * (y))[2] * matrix[0, 1]) + ((dataPtrRead + nChan * (x) + widthStep * (y))[2] * matrix[1, 1]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (y))[2] * matrix[2, 1]) +
                                                                                           ((dataPtrRead + nChan * (x - 1) + widthStep * (y + 1))[2] * matrix[0, 2]) + ((dataPtrRead + nChan * (x) + widthStep * (y + 1))[2] * matrix[1, 2]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (y + 1))[2] * matrix[2, 2])) / matrixWeight), 0), 255);
                    }
                }
            }
        }

        /// <summary>
        /// Average filter direct acess
        /// </summary>
        /// <param name="img">Image</param>
        public static void Mean(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrRead = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrWrite = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int heigh = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;

                (dataPtrWrite)[0] = (byte)Math.Round(
                        (4 * dataPtrRead[0] +
                        2 * (dataPtrRead + nChan)[0] +
                        2 * (dataPtrRead + widthStep)[0]
                        + (dataPtrRead + nChan + widthStep)[0]) / 9.0);

                (dataPtrWrite)[1] = (byte)Math.Round(
                    (4 * dataPtrRead[1] +
                    2 * (dataPtrRead + nChan)[1] +
                    2 * (dataPtrRead + widthStep)[1]
                    + (dataPtrRead + nChan + widthStep)[1]) / 9.0);

                (dataPtrWrite)[2] = (byte)Math.Round(
                    (4 * dataPtrRead[2] +
                    2 * (dataPtrRead + nChan)[2] +
                    2 * (dataPtrRead + widthStep)[2] +
                    (dataPtrRead + nChan + widthStep)[2]) / 9.0);

                (dataPtrWrite + nChan * (width - 1) + widthStep * (heigh - 1))[0] =
                    (byte)Math.Round(
                        (4 * (dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[0] +
                        2 * (dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 2))[0] +
                        2 * (dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 1))[0] +
                        (dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 2))[0]) / 9.0);

                (dataPtrWrite + nChan * (width - 1) + widthStep * (heigh - 1))[1] =
                    (byte)Math.Round(
                        (4 * (dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[1] +
                        2 * (dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 2))[1] +
                        2 * (dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 1))[1] +
                        (dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 2))[1]) / 9.0);

                (dataPtrWrite + nChan * (width - 1) + widthStep * (heigh - 1))[2] =
                    (byte)Math.Round(
                        (4 * (dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[2] +
                        2 * (dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 2))[2] +
                        2 * (dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 1))[2] +
                        (dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 2))[2]) / 9.0);

                (dataPtrWrite + nChan * (width - 1))[0] = (byte)Math.Round(
                    (4 * (dataPtrRead + nChan * (width - 1))[0] +
                    2 * (dataPtrRead + nChan * (width - 2))[0] +
                    2 * (dataPtrRead + nChan * (width - 1) + widthStep)[0]
                    + (dataPtrRead + nChan * (width - 2) + widthStep)[0]) / 9.0);

                (dataPtrWrite + nChan * (width - 1))[1] =
                    (byte)Math.Round(
                        (4 * (dataPtrRead + nChan * (width - 1))[1] +
                        2 * (dataPtrRead + nChan * (width - 2))[1] +
                        2 * (dataPtrRead + nChan * (width - 1) + widthStep)[1]
                        + (dataPtrRead + nChan * (width - 2) + widthStep)[1]) / 9.0);

                (dataPtrWrite + nChan * (width - 1))[2] =
                    (byte)Math.Round(
                        (4 * (dataPtrRead + nChan * (width - 1))[2] +
                        2 * (dataPtrRead + nChan * (width - 2))[2] +
                        2 * (dataPtrRead + nChan * (width - 1) + widthStep)[2]
                        + (dataPtrRead + nChan * (width - 2) + widthStep)[2]) / 9.0);

                (dataPtrWrite + widthStep * (heigh - 1))[0] = (byte)Math.Round((4 * (dataPtrRead + widthStep * (heigh - 1))[0] + 2 * (dataPtrRead + widthStep * (heigh - 2))[0] + 2 * (dataPtrRead + nChan + widthStep * (heigh - 1))[0] + (dataPtrRead + nChan + widthStep * (heigh - 2))[0]) / 9.0);
                (dataPtrWrite + widthStep * (heigh - 1))[1] = (byte)Math.Round((4 * (dataPtrRead + widthStep * (heigh - 1))[1] + 2 * (dataPtrRead + widthStep * (heigh - 2))[1] + 2 * (dataPtrRead + nChan + widthStep * (heigh - 1))[1] + (dataPtrRead + nChan + widthStep * (heigh - 2))[1]) / 9.0);
                (dataPtrWrite + widthStep * (heigh - 1))[2] = (byte)Math.Round((4 * (dataPtrRead + widthStep * (heigh - 1))[2] + 2 * (dataPtrRead + widthStep * (heigh - 2))[2] + 2 * (dataPtrRead + nChan + widthStep * (heigh - 1))[2] + (dataPtrRead + nChan + widthStep * (heigh - 2))[2]) / 9.0);

                for (int x = 1; x < width - 1; x++)
                {
                    (dataPtrWrite + nChan * x)[0] = (byte)Math.Round(((dataPtrRead + nChan * (x - 1))[0] + (dataPtrRead + nChan * x)[0] + (dataPtrRead + nChan * (x + 1))[0] +
                                                       (dataPtrRead + nChan * (x - 1))[0] + (dataPtrRead + nChan * x)[0] + (dataPtrRead + nChan * (x + 1))[0] +
                                                       (dataPtrRead + nChan * (x - 1) + widthStep * 1)[0] + (dataPtrRead + nChan * x + widthStep * 1)[0] + (dataPtrRead + nChan * (x + 1) + widthStep * 1)[0]) / 9.0);

                    (dataPtrWrite + nChan * x)[1] = (byte)Math.Round(((dataPtrRead + nChan * (x - 1))[1] + (dataPtrRead + nChan * x)[1] + (dataPtrRead + nChan * (x + 1))[1] +
                                                       (dataPtrRead + nChan * (x - 1))[1] + (dataPtrRead + nChan * x)[1] + (dataPtrRead + nChan * (x + 1))[1] +
                                                       (dataPtrRead + nChan * (x - 1) + widthStep * 1)[1] + (dataPtrRead + nChan * x + widthStep * 1)[1] + (dataPtrRead + nChan * (x + 1) + widthStep * 1)[1]) / 9.0);

                    (dataPtrWrite + nChan * x)[2] = (byte)Math.Round(((dataPtrRead + nChan * (x - 1))[2] + (dataPtrRead + nChan * x)[2] + (dataPtrRead + nChan * (x + 1))[2] +
                                                       (dataPtrRead + nChan * (x - 1))[2] + (dataPtrRead + nChan * x)[2] + (dataPtrRead + nChan * (x + 1))[2] +
                                                       (dataPtrRead + nChan * (x - 1) + widthStep * 1)[2] + (dataPtrRead + nChan * x + widthStep * 1)[2] + (dataPtrRead + nChan * (x + 1) + widthStep * 1)[2]) / 9.0);

                    (dataPtrWrite + nChan * x + widthStep * (heigh - 1))[0] = (byte)Math.Round(((dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 1))[0] + (dataPtrRead + nChan * x + widthStep * (heigh - 1))[0] + (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 1))[0] +
                                                                                 (dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 1))[0] + (dataPtrRead + nChan * x + widthStep * (heigh - 1))[0] + (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 1))[0] +
                                                                                 (dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 2))[0] + (dataPtrRead + nChan * x + widthStep * (heigh - 2))[0] + (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 2))[0]) / 9.0);

                    (dataPtrWrite + nChan * x + widthStep * (heigh - 1))[1] = (byte)Math.Round(((dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 1))[1] + (dataPtrRead + nChan * x + widthStep * (heigh - 1))[1] + (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 1))[1] +
                                                                                 (dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 1))[1] + (dataPtrRead + nChan * x + widthStep * (heigh - 1))[1] + (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 1))[1] +
                                                                                 (dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 2))[1] + (dataPtrRead + nChan * x + widthStep * (heigh - 2))[1] + (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 2))[1]) / 9.0);

                    (dataPtrWrite + nChan * x + widthStep * (heigh - 1))[2] = (byte)Math.Round(((dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 1))[2] + (dataPtrRead + nChan * x + widthStep * (heigh - 1))[2] + (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 1))[2] +
                                                                                 (dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 1))[2] + (dataPtrRead + nChan * x + widthStep * (heigh - 1))[2] + (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 1))[2] +
                                                                                 (dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 2))[2] + (dataPtrRead + nChan * x + widthStep * (heigh - 2))[2] + (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 2))[2]) / 9.0);
                }
                for (int y = 1; y < heigh - 1; y++)
                {
                    (dataPtrWrite + widthStep * y)[0] = (byte)Math.Round(((dataPtrRead + widthStep * (y - 1))[0] + (dataPtrRead + widthStep * (y - 1))[0] + (dataPtrRead + nChan + widthStep * (y - 1))[0] +
                                                           (dataPtrRead + widthStep * (y))[0] + (dataPtrRead + widthStep * (y))[0] + (dataPtrRead + nChan + widthStep * (y))[0] +
                                                           (dataPtrRead + widthStep * (y + 1))[0] + (dataPtrRead + widthStep * (y + 1))[0] + (dataPtrRead + nChan + widthStep * (y + 1))[0]) / 9.0);

                    (dataPtrWrite + widthStep * y)[1] = (byte)Math.Round(((dataPtrRead + widthStep * (y - 1))[1] + (dataPtrRead + widthStep * (y - 1))[1] + (dataPtrRead + nChan + widthStep * (y - 1))[1] +
                                                           (dataPtrRead + widthStep * (y))[1] + (dataPtrRead + widthStep * (y))[1] + (dataPtrRead + nChan + widthStep * (y))[1] +
                                                           (dataPtrRead + widthStep * (y + 1))[1] + (dataPtrRead + widthStep * (y + 1))[1] + (dataPtrRead + nChan + widthStep * (y + 1))[1]) / 9.0);

                    (dataPtrWrite + widthStep * y)[2] = (byte)Math.Round(((dataPtrRead + widthStep * (y - 1))[2] + (dataPtrRead + widthStep * (y - 1))[2] + (dataPtrRead + nChan + widthStep * (y - 1))[2] +
                                                           (dataPtrRead + widthStep * (y))[2] + (dataPtrRead + widthStep * (y))[2] + (dataPtrRead + nChan + widthStep * (y))[2] +
                                                           (dataPtrRead + widthStep * (y + 1))[2] + (dataPtrRead + widthStep * (y + 1))[2] + (dataPtrRead + nChan + widthStep * (y + 1))[2]) / 9.0);

                    (dataPtrWrite + nChan * (width - 1) + widthStep * y)[0] = (byte)Math.Round(((dataPtrRead + nChan * (width - 1) + widthStep * (y - 1))[0] + (dataPtrRead + nChan * (width - 1) + widthStep * (y - 1))[0] + (dataPtrRead + nChan * (width - 2) + widthStep * (y - 1))[0] +
                                                                                                 (dataPtrRead + nChan * (width - 1) + widthStep * y)[0] + (dataPtrRead + nChan * (width - 1) + widthStep * y)[0] + (dataPtrRead + nChan * (width - 2) + widthStep * y)[0] +
                                                                                                 (dataPtrRead + nChan * (width - 1) + widthStep * (y + 1))[0] + (dataPtrRead + nChan * (width - 1) + widthStep * (y + 1))[0] + (dataPtrRead + nChan * (width - 2) + widthStep * (y + 1))[0]) / 9.0);

                    (dataPtrWrite + nChan * (width - 1) + widthStep * y)[1] = (byte)Math.Round(((dataPtrRead + nChan * (width - 1) + widthStep * (y - 1))[1] + (dataPtrRead + nChan * (width - 1) + widthStep * (y - 1))[1] + (dataPtrRead + nChan * (width - 2) + widthStep * (y - 1))[1] +
                                                                                                      (dataPtrRead + nChan * (width - 1) + widthStep * y)[1] + (dataPtrRead + nChan * (width - 1) + widthStep * y)[1] + (dataPtrRead + nChan * (width - 2) + widthStep * y)[1] +
                                                                                                      (dataPtrRead + nChan * (width - 1) + widthStep * (y + 1))[1] + (dataPtrRead + nChan * (width - 1) + widthStep * (y + 1))[1] + (dataPtrRead + nChan * (width - 2) + widthStep * (y + 1))[1]) / 9.0);

                    (dataPtrWrite + nChan * (width - 1) + widthStep * y)[2] = (byte)Math.Round(((dataPtrRead + nChan * (width - 1) + widthStep * (y - 1))[2] + (dataPtrRead + nChan * (width - 1) + widthStep * (y - 1))[2] + (dataPtrRead + nChan * (width - 2) + widthStep * (y - 1))[2] +
                                                                                                      (dataPtrRead + nChan * (width - 1) + widthStep * y)[2] + (dataPtrRead + nChan * (width - 1) + widthStep * y)[2] + (dataPtrRead + nChan * (width - 2) + widthStep * y)[2] +
                                                                                                      (dataPtrRead + nChan * (width - 1) + widthStep * (y + 1))[2] + (dataPtrRead + nChan * (width - 1) + widthStep * (y + 1))[2] + (dataPtrRead + nChan * (width - 2) + widthStep * (y + 1))[2]) / 9.0);
                }

                for (int y = 1; y < heigh - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        (dataPtrWrite + nChan * x + widthStep * y)[0] = (byte)Math.Round(((dataPtrRead + nChan * (x - 1) + widthStep * (y - 1))[0] + (dataPtrRead + nChan * (x) + widthStep * (y - 1))[0] + (dataPtrRead + nChan * (x + 1) + widthStep * (y - 1))[0] +
                                                                                           (dataPtrRead + nChan * (x - 1) + widthStep * (y))[0] + (dataPtrRead + nChan * (x) + widthStep * (y))[0] + (dataPtrRead + nChan * (x + 1) + widthStep * (y))[0] +
                                                                                           (dataPtrRead + nChan * (x - 1) + widthStep * (y + 1))[0] + (dataPtrRead + nChan * (x) + widthStep * (y + 1))[0] + (dataPtrRead + nChan * (x + 1) + widthStep * (y + 1))[0]) / 9.0);

                        (dataPtrWrite + nChan * x + widthStep * y)[1] = (byte)Math.Round(((dataPtrRead + nChan * (x - 1) + widthStep * (y - 1))[1] + (dataPtrRead + nChan * (x) + widthStep * (y - 1))[1] + (dataPtrRead + nChan * (x + 1) + widthStep * (y - 1))[1] +
                                                                                           (dataPtrRead + nChan * (x - 1) + widthStep * (y))[1] + (dataPtrRead + nChan * (x) + widthStep * (y))[1] + (dataPtrRead + nChan * (x + 1) + widthStep * (y))[1] +
                                                                                           (dataPtrRead + nChan * (x - 1) + widthStep * (y + 1))[1] + (dataPtrRead + nChan * (x) + widthStep * (y + 1))[1] + (dataPtrRead + nChan * (x + 1) + widthStep * (y + 1))[1]) / 9.0);

                        (dataPtrWrite + nChan * x + widthStep * y)[2] = (byte)Math.Round(((dataPtrRead + nChan * (x - 1) + widthStep * (y - 1))[2] + (dataPtrRead + nChan * (x) + widthStep * (y - 1))[2] + (dataPtrRead + nChan * (x + 1) + widthStep * (y - 1))[2] +
                                                                                           (dataPtrRead + nChan * (x - 1) + widthStep * (y))[2] + (dataPtrRead + nChan * (x) + widthStep * (y))[2] + (dataPtrRead + nChan * (x + 1) + widthStep * (y))[2] +
                                                                                           (dataPtrRead + nChan * (x - 1) + widthStep * (y + 1))[2] + (dataPtrRead + nChan * (x) + widthStep * (y + 1))[2] + (dataPtrRead + nChan * (x + 1) + widthStep * (y + 1))[2]) / 9.0);
                    }
                }
            }
        }

        /// <summary>
        /// Rotates the image by a given angle
        /// </summary>
        /// <param name="img">Image</param>
        public static void Rotation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrRead = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrWrite = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int heigh = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;
                byte red, green, blue;
                int x0, y0;

                double W = width / 2f;
                double H = heigh / 2f;

                for (int y = 0; y < heigh; y++)
                {
                    for (int x = 0; x < width; x++)
                    {

                        double cos = Math.Cos(angle);
                        double sin = Math.Sin(angle);

                        x0 = (int)Math.Round((x - W) * cos - (H - y) * sin + W);
                        y0 = (int)Math.Round(H - (x - W) * sin - (H - y) * cos);

                        if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < heigh)
                        {
                            red = (dataPtrRead + nChan * x0 + widthStep * y0)[2];
                            green = (dataPtrRead + nChan * x0 + widthStep * y0)[1];
                            blue = (dataPtrRead + nChan * x0 + widthStep * y0)[0];
                        }
                        else
                        {
                            red = 0;
                            green = 0;
                            blue = 0;
                        }

                        (dataPtrWrite + nChan * x + widthStep * y)[2] = red;
                        (dataPtrWrite + nChan * x + widthStep * y)[1] = green;
                        (dataPtrWrite + nChan * x + widthStep * y)[0] = blue;
                    }
                }
            }
        }

        /// <summary>
        /// Convert to gray
        /// Direct access to memory - faster method
        /// </summary>
        /// <param name="img">image</param>
        public static void ConvertToGray(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red, gray;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrive 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            // convert to gray
                            gray = (byte)Math.Round((blue + green + red) / 3.0);

                            // store in the image
                            dataPtr[0] = gray;
                            dataPtr[1] = gray;
                            dataPtr[2] = gray;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        /// <summary>
        /// Media filter direct acess
        /// </summary>
        /// <param name="img">Imge</param>
        public static void Median(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrRead = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrWrite = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int heigh = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;

                double[,] distsTotal = new double[3, 3];

                for (int y = 1; y < heigh - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        double dist = 0;

                        for (int OffsetX = -1; OffsetX <= 1; OffsetX++)
                        {
                            for (int OffsetY = -1; OffsetY <= 1; OffsetY++)
                            {
                                for (int inX = -1; inX <= 1; inX++)
                                {
                                    for (int inY = -1; inY <= 1; inY++)
                                    {
                                        dist += (
                                            Math.Abs((dataPtrRead + nChan * (x + OffsetX) + widthStep * (y + OffsetY))[0] - (dataPtrRead + nChan * (x + inX) + widthStep * (y + inY))[0]) +
                                            Math.Abs((dataPtrRead + nChan * (x + OffsetX) + widthStep * (y + OffsetY))[1] - (dataPtrRead + nChan * (x + inX) + widthStep * (y + inY))[1]) +
                                            Math.Abs((dataPtrRead + nChan * (x + OffsetX) + widthStep * (y + OffsetY))[2] - (dataPtrRead + nChan * (x + inX) + widthStep * (y + inY))[2])) / 9;
                                    }
                                }
                                distsTotal[OffsetX + 1, OffsetY + 1] = dist;
                                dist = 0;
                            }
                        }

                        double lowest = distsTotal[0, 0];

                        int indexX = 0;
                        int indexY = 0;

                        for (int i = 0; i < distsTotal.GetLength(0); i++)
                        {
                            for (int b = 0; b < distsTotal.GetLength(1); b++)
                            {
                                if (distsTotal[i, b] < lowest)
                                {
                                    lowest = distsTotal[i, b];
                                    indexX = i;
                                    indexY = b;
                                }
                            }
                        }
                        indexX -= 1;
                        indexY -= 1;

                        (dataPtrWrite + nChan * x + widthStep * y)[0] = (dataPtrRead + nChan * (x + indexX) + widthStep * (y + indexY))[0];
                        (dataPtrWrite + nChan * x + widthStep * y)[1] = (dataPtrRead + nChan * (x + indexX) + widthStep * (y + indexY))[1];
                        (dataPtrWrite + nChan * x + widthStep * y)[2] = (dataPtrRead + nChan * (x + indexX) + widthStep * (y + indexY))[2];

                        //int[,] vals = new int[3, 3];
                        //double val = 0;

                        //for (int inX = -1; inX <= 1; inX++)
                        //{
                        //    for (int inY = -1; inY <= 1; inY++)
                        //    {
                        //        vals[inX + 1, inY + 1] =
                        //            (dataPtrRead + nChan * (x + indexX + inX) + widthStep * (y + indexY + inY))[0] +
                        //            (dataPtrRead + nChan * (x + indexX + inX) + widthStep * (y + indexY + inY))[1] +
                        //            (dataPtrRead + nChan * (x + indexX + inX) + widthStep * (y + indexY + inY))[2];

                        //        val += vals[inX + 1, inY + 1];
                        //    }
                        //}

                        //val /= 2;
                        //double distance = 1000000;

                        //int indexerX = 0;
                        //int indexerY = 0;

                        //for (int i = 0; i < vals.GetLength(0); i++)
                        //{
                        //    for (int b = 0; b < vals.GetLength(1); b++)
                        //    {
                        //        if (vals[i, b] - val < distance)
                        //        {
                        //            distance = vals[i, b] - val;
                        //            indexerX = i;
                        //            indexerY = b;
                        //        }
                        //    }
                        //}
                        //indexerX -= 1;
                        //indexerY -= 1;

                        //(dataPtrWrite + nChan * x + widthStep * y)[0] = (dataPtrRead + nChan * (x + indexX + indexerX) + widthStep * (y + indexY + indexerY))[0];
                        //(dataPtrWrite + nChan * x + widthStep * y)[1] = (dataPtrRead + nChan * (x + indexX + indexerX) + widthStep * (y + indexY + indexerY))[1];
                        //(dataPtrWrite + nChan * x + widthStep * y)[2] = (dataPtrRead + nChan * (x + indexX + indexerX) + widthStep * (y + indexY + indexerY))[2];
                    }
                }
            }
        }

        /// <summary>
        /// Sobel filter direct acess
        /// </summary>
        /// <param name="img">Image</param>
        public static void Sobel(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrRead = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrWrite = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int heigh = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;

                // Top left
                double Sx = ((3 * dataPtrRead[0]) + (dataPtrRead + widthStep)[0]) -
                            ((3 * (dataPtrRead + nChan)[0]) + (dataPtrRead + nChan + widthStep)[0]);

                double Sy = ((3 * dataPtrRead[0]) + (dataPtrRead + nChan)[0]) -
                            ((3 * (dataPtrRead + widthStep)[0]) + (dataPtrRead + nChan + widthStep)[0]);

                (dataPtrWrite)[0] = (byte)Math.Min(Math.Max(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 0), 255);

                Sx = ((3 * dataPtrRead[1]) + (dataPtrRead + widthStep)[1]) -
                     ((3 * (dataPtrRead + nChan)[1]) + (dataPtrRead + nChan + widthStep)[1]);

                Sy = ((3 * dataPtrRead[1]) + (dataPtrRead + nChan)[1]) -
                     ((3 * (dataPtrRead + widthStep)[1]) + (dataPtrRead + nChan + widthStep)[1]);

                (dataPtrWrite)[1] = (byte)Math.Min(Math.Max(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 0), 255);

                Sx = ((3 * dataPtrRead[2]) + (dataPtrRead + widthStep)[2]) -
                     ((3 * (dataPtrRead + nChan)[2]) + (dataPtrRead + nChan + widthStep)[2]);

                Sy = ((3 * dataPtrRead[2]) + (dataPtrRead + nChan)[2]) -
                     ((3 * (dataPtrRead + widthStep)[2]) + (dataPtrRead + nChan + widthStep)[2]);

                (dataPtrWrite)[2] = (byte)Math.Min(Math.Max(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 0), 255);

                // Bottom right
                Sx = ((dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 2))[0] + (3 * (dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 1))[0])) -
                     ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 2))[0] + (3 * (dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[0]));

                Sy = (((3 * (dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[0]) + (dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 1))[0])) -
                     (((3 * (dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 2))[0]) + (dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 2))[0]));

                (dataPtrWrite + nChan * (width - 1) + widthStep * (heigh - 1))[0] =
                   (byte)Math.Min(Math.Max(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 0), 255);

                Sx = ((dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 2))[1] + (3 * (dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 1))[1])) -
                     ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 2))[1] + (3 * (dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[1]));

                Sy = (((3 * (dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[1]) + (dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 1))[1])) -
                     (((3 * (dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 2))[1]) + (dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 2))[1]));

                (dataPtrWrite + nChan * (width - 1) + widthStep * (heigh - 1))[1] =
                   (byte)Math.Min(Math.Max(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 0), 255);

                Sx = ((dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 2))[2] + (3 * (dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 1))[2])) -
                     ((dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 2))[2] + (3 * (dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[2]));

                Sy = (((3 * (dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 1))[2]) + (dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 1))[2])) -
                     (((3 * (dataPtrRead + nChan * (width - 1) + widthStep * (heigh - 2))[2]) + (dataPtrRead + nChan * (width - 2) + widthStep * (heigh - 2))[2]));

                (dataPtrWrite + nChan * (width - 1) + widthStep * (heigh - 1))[2] =
                   (byte)Math.Min(Math.Max(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 0), 255);

                // Top Right
                Sx = ((dataPtrRead + nChan * (width - 2) + widthStep)[0] + (3 * (dataPtrRead + nChan * (width - 2))[0])) -
                     ((dataPtrRead + nChan * (width - 1) + widthStep)[0] + (3 * (dataPtrRead + nChan * (width - 1))[0]));

                Sy = ((dataPtrRead + nChan * (width - 2) + widthStep)[0] + (3 * (dataPtrRead + nChan * (width - 1) + widthStep)[0])) -
                     ((dataPtrRead + nChan * (width - 2))[0] + (3 * (dataPtrRead + nChan * (width - 1))[0]));

                (dataPtrWrite + nChan * (width - 1))[0] = (byte)Math.Min(Math.Max(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 0), 255);

                Sx = ((dataPtrRead + nChan * (width - 2) + widthStep)[1] + (3 * (dataPtrRead + nChan * (width - 2))[1])) -
                     ((dataPtrRead + nChan * (width - 1) + widthStep)[1] + (3 * (dataPtrRead + nChan * (width - 1))[1]));

                Sy = ((dataPtrRead + nChan * (width - 2) + widthStep)[1] + (3 * (dataPtrRead + nChan * (width - 1) + widthStep)[1])) -
                     ((dataPtrRead + nChan * (width - 2))[1] + (3 * (dataPtrRead + nChan * (width - 1))[1]));

                (dataPtrWrite + nChan * (width - 1))[1] = (byte)Math.Min(Math.Max(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 0), 255);

                Sx = ((dataPtrRead + nChan * (width - 2) + widthStep)[2] + (3 * (dataPtrRead + nChan * (width - 2))[2])) -
                     ((dataPtrRead + nChan * (width - 1) + widthStep)[2] + (3 * (dataPtrRead + nChan * (width - 1))[2]));

                Sy = ((dataPtrRead + nChan * (width - 2) + widthStep)[2] + (3 * (dataPtrRead + nChan * (width - 1) + widthStep)[2])) -
                     ((dataPtrRead + nChan * (width - 2))[2] + (3 * (dataPtrRead + nChan * (width - 1))[2]));

                (dataPtrWrite + nChan * (width - 1))[2] = (byte)Math.Min(Math.Max(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 0), 255);

                // Bottom Left
                Sx = ((dataPtrRead + widthStep * (heigh - 2))[0] + (3 * (dataPtrRead + widthStep * (heigh - 1))[0])) -
                     ((dataPtrRead + nChan + widthStep * (heigh - 2))[0] + (3 * (dataPtrRead + nChan + widthStep * (heigh - 1))[0]));

                Sy = ((3 * (dataPtrRead + widthStep * (heigh - 1))[0]) + (dataPtrRead + nChan + widthStep * (heigh - 1))[0]) -
                     ((3 * (dataPtrRead + widthStep * (heigh - 2))[0]) + (dataPtrRead + nChan + widthStep * (heigh - 2))[0]);

                (dataPtrWrite + widthStep * (heigh - 1))[0] = (byte)Math.Min(Math.Max(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 0), 255);

                Sx = ((dataPtrRead + widthStep * (heigh - 2))[1] + (3 * (dataPtrRead + widthStep * (heigh - 1))[1])) -
                     ((dataPtrRead + nChan + widthStep * (heigh - 2))[1] + (3 * (dataPtrRead + nChan + widthStep * (heigh - 1))[1]));

                Sy = ((3 * (dataPtrRead + widthStep * (heigh - 1))[1]) + (dataPtrRead + nChan + widthStep * (heigh - 1))[1]) -
                     ((3 * (dataPtrRead + widthStep * (heigh - 2))[1]) + (dataPtrRead + nChan + widthStep * (heigh - 2))[1]);

                (dataPtrWrite + widthStep * (heigh - 1))[1] = (byte)Math.Min(Math.Max(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 0), 255);

                Sx = ((dataPtrRead + widthStep * (heigh - 2))[2] + (3 * (dataPtrRead + widthStep * (heigh - 1))[2])) -
                     ((dataPtrRead + nChan + widthStep * (heigh - 2))[2] + (3 * (dataPtrRead + nChan + widthStep * (heigh - 1))[2]));

                Sy = ((3 * (dataPtrRead + widthStep * (heigh - 1))[2]) + (dataPtrRead + nChan + widthStep * (heigh - 1))[2]) -
                     ((3 * (dataPtrRead + widthStep * (heigh - 2))[2]) + (dataPtrRead + nChan + widthStep * (heigh - 2))[2]);

                (dataPtrWrite + widthStep * (heigh - 1))[2] = (byte)Math.Min(Math.Max(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 0), 255);

                for (int x = 1; x < width - 1; x++)
                {
                    // Top
                    Sx = ((3 * (dataPtrRead + nChan * (x - 1))[0]) + (dataPtrRead + nChan * (x - 1) + widthStep)[0]) -
                         ((3 * (dataPtrRead + nChan * (x + 1))[0]) + (dataPtrRead + nChan * (x + 1) + widthStep)[0]);

                    Sy = ((dataPtrRead + nChan * (x - 1))[0] + (2 * (dataPtrRead + nChan * x)[0]) + (dataPtrRead + nChan * (x + 1))[0]) -
                         ((dataPtrRead + nChan * (x - 1) + widthStep)[0] + (2 * (dataPtrRead + nChan * x + widthStep)[0]) + (dataPtrRead + nChan * (x + 1) + widthStep)[0]);

                    (dataPtrWrite + nChan * x)[0] = (byte)Math.Max(Math.Min(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 255), 0);

                    Sx = ((3 * (dataPtrRead + nChan * (x - 1))[1]) + (dataPtrRead + nChan * (x - 1) + widthStep)[1]) -
                         ((3 * (dataPtrRead + nChan * (x + 1))[1]) + (dataPtrRead + nChan * (x + 1) + widthStep)[1]);

                    Sy = ((dataPtrRead + nChan * (x - 1))[1] + (2 * (dataPtrRead + nChan * x)[1]) + (dataPtrRead + nChan * (x + 1))[1]) -
                         ((dataPtrRead + nChan * (x - 1) + widthStep)[1] + (2 * (dataPtrRead + nChan * x + widthStep)[1]) + (dataPtrRead + nChan * (x + 1) + widthStep)[1]);

                    (dataPtrWrite + nChan * x)[1] = (byte)Math.Max(Math.Min(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 255), 0);

                    Sx = ((3 * (dataPtrRead + nChan * (x - 1))[2]) + (dataPtrRead + nChan * (x - 1) + widthStep)[2]) -
                         ((3 * (dataPtrRead + nChan * (x + 1))[2]) + (dataPtrRead + nChan * (x + 1) + widthStep)[2]);

                    Sy = ((dataPtrRead + nChan * (x - 1))[2] + (2 * (dataPtrRead + nChan * x)[2]) + (dataPtrRead + nChan * (x + 1))[2]) -
                         ((dataPtrRead + nChan * (x - 1) + widthStep)[2] + (2 * (dataPtrRead + nChan * x + widthStep)[2]) + (dataPtrRead + nChan * (x + 1) + widthStep)[2]);

                    (dataPtrWrite + nChan * x)[2] = (byte)Math.Max(Math.Min(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 255), 0);

                    // Bottom
                    Sx = ((3 * (dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 1))[0]) + (dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 2))[0]) -
                         ((3 * (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 1))[0]) + (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 2))[0]);

                    Sy = ((dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 1))[0] + (2 * (dataPtrRead + nChan * x + widthStep * (heigh - 1))[0]) + (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 1))[0]) -
                         ((dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 2))[0] + (2 * (dataPtrRead + nChan * x + widthStep * (heigh - 2))[0]) + (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 2))[0]);

                    (dataPtrWrite + nChan * x + widthStep * (heigh - 1))[0] = (byte)Math.Max(Math.Min(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 255), 0);

                    Sx = ((3 * (dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 1))[1]) + (dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 2))[1]) -
                         ((3 * (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 1))[1]) + (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 2))[1]);

                    Sy = ((dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 1))[1] + (2 * (dataPtrRead + nChan * x + widthStep * (heigh - 1))[1]) + (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 1))[1]) -
                         ((dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 2))[1] + (2 * (dataPtrRead + nChan * x + widthStep * (heigh - 2))[1]) + (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 2))[1]);

                    (dataPtrWrite + nChan * x + widthStep * (heigh - 1))[1] = (byte)Math.Max(Math.Min(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 255), 0);

                    Sx = ((3 * (dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 1))[2]) + (dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 2))[2]) -
                         ((3 * (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 1))[2]) + (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 2))[2]);

                    Sy = ((dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 1))[2] + (2 * (dataPtrRead + nChan * x + widthStep * (heigh - 1))[2]) + (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 1))[2]) -
                         ((dataPtrRead + nChan * (x - 1) + widthStep * (heigh - 2))[2] + (2 * (dataPtrRead + nChan * x + widthStep * (heigh - 2))[2]) + (dataPtrRead + nChan * (x + 1) + widthStep * (heigh - 2))[2]);

                    (dataPtrWrite + nChan * x + widthStep * (heigh - 1))[2] = (byte)Math.Max(Math.Min(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 255), 0);
                }
                for (int y = 1; y < heigh - 1; y++)
                {
                    // Left
                    Sx = ((dataPtrRead + widthStep * (y - 1))[0] + (2 * (dataPtrRead + widthStep * y)[0]) + (dataPtrRead + widthStep * (y + 1))[0]) -
                         ((dataPtrRead + nChan + widthStep * (y - 1))[0] + (2 * (dataPtrRead + nChan + widthStep * y)[0]) + (dataPtrRead + nChan + widthStep * (y + 1))[0]);

                    Sy = ((3 * (dataPtrRead + widthStep * (y - 1))[0]) + (dataPtrRead + nChan + widthStep * (y - 1))[0]) -
                         ((3 * (dataPtrRead + widthStep * (y + 1))[0]) + (dataPtrRead + nChan + widthStep * (y + 1))[0]);

                    (dataPtrWrite + widthStep * y)[0] = (byte)Math.Max(Math.Min(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 255), 0);

                    Sx = ((dataPtrRead + widthStep * (y - 1))[1] + (2 * (dataPtrRead + widthStep * y)[1]) + (dataPtrRead + widthStep * (y + 1))[1]) -
                         ((dataPtrRead + nChan + widthStep * (y - 1))[1] + (2 * (dataPtrRead + nChan + widthStep * y)[1]) + (dataPtrRead + nChan + widthStep * (y + 1))[1]);

                    Sy = ((3 * (dataPtrRead + widthStep * (y - 1))[1]) + (dataPtrRead + nChan + widthStep * (y - 1))[1]) -
                         ((3 * (dataPtrRead + widthStep * (y + 1))[1]) + (dataPtrRead + nChan + widthStep * (y + 1))[1]);

                    (dataPtrWrite + widthStep * y)[1] = (byte)Math.Max(Math.Min(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 255), 0);

                    Sx = ((dataPtrRead + widthStep * (y - 1))[2] + (2 * (dataPtrRead + widthStep * y)[2]) + (dataPtrRead + widthStep * (y + 1))[2]) -
                         ((dataPtrRead + nChan + widthStep * (y - 1))[2] + (2 * (dataPtrRead + nChan + widthStep * y)[2]) + (dataPtrRead + nChan + widthStep * (y + 1))[2]);

                    Sy = ((3 * (dataPtrRead + widthStep * (y - 1))[2]) + (dataPtrRead + nChan + widthStep * (y - 1))[2]) -
                         ((3 * (dataPtrRead + widthStep * (y + 1))[2]) + (dataPtrRead + nChan + widthStep * (y + 1))[2]);

                    (dataPtrWrite + widthStep * y)[2] = (byte)Math.Max(Math.Min(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 255), 0);

                    // Right
                    Sx = ((dataPtrRead + nChan * (width - 1) + widthStep * (y - 1))[0] + (2 * (dataPtrRead + nChan * (width - 1) + widthStep * y)[0]) + (dataPtrRead + nChan * (width - 1) + widthStep * (y + 1))[0]) -
                         ((dataPtrRead + nChan * (width - 2) + widthStep * (y - 1))[0] + (2 * (dataPtrRead + nChan * (width - 2) + widthStep * y)[0]) + (dataPtrRead + nChan * (width - 2) + widthStep * (y + 1))[0]);

                    Sy = ((3 * (dataPtrRead + nChan * (width - 1) + widthStep * (y - 1))[0]) + (dataPtrRead + nChan * (width - 2) + widthStep * (y - 1))[0]) -
                         ((3 * (dataPtrRead + nChan * (width - 1) + widthStep * (y + 1))[0]) + (dataPtrRead + nChan * (width - 2) + widthStep * (y + 1))[0]);

                    (dataPtrWrite + nChan * (width - 1) + widthStep * y)[0] = (byte)Math.Max(Math.Min(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 255), 0);

                    Sx = ((dataPtrRead + nChan * (width - 1) + widthStep * (y - 1))[1] + (2 * (dataPtrRead + nChan * (width - 1) + widthStep * y)[1]) + (dataPtrRead + nChan * (width - 1) + widthStep * (y + 1))[1]) -
                         ((dataPtrRead + nChan * (width - 2) + widthStep * (y - 1))[1] + (2 * (dataPtrRead + nChan * (width - 2) + widthStep * y)[1]) + (dataPtrRead + nChan * (width - 2) + widthStep * (y + 1))[1]);

                    Sy = ((3 * (dataPtrRead + nChan * (width - 1) + widthStep * (y - 1))[1]) + (dataPtrRead + nChan * (width - 2) + widthStep * (y - 1))[1]) -
                         ((3 * (dataPtrRead + nChan * (width - 1) + widthStep * (y + 1))[1]) + (dataPtrRead + nChan * (width - 2) + widthStep * (y + 1))[1]);

                    (dataPtrWrite + nChan * (width - 1) + widthStep * y)[1] = (byte)Math.Max(Math.Min(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 255), 0);

                    Sx = ((dataPtrRead + nChan * (width - 1) + widthStep * (y - 1))[2] + (2 * (dataPtrRead + nChan * (width - 1) + widthStep * y)[2]) + (dataPtrRead + nChan * (width - 1) + widthStep * (y + 1))[2]) -
                         ((dataPtrRead + nChan * (width - 2) + widthStep * (y - 1))[2] + (2 * (dataPtrRead + nChan * (width - 2) + widthStep * y)[2]) + (dataPtrRead + nChan * (width - 2) + widthStep * (y + 1))[2]);

                    Sy = ((3 * (dataPtrRead + nChan * (width - 1) + widthStep * (y - 1))[2]) + (dataPtrRead + nChan * (width - 2) + widthStep * (y - 1))[2]) -
                         ((3 * (dataPtrRead + nChan * (width - 1) + widthStep * (y + 1))[2]) + (dataPtrRead + nChan * (width - 2) + widthStep * (y + 1))[2]);

                    (dataPtrWrite + nChan * (width - 1) + widthStep * y)[2] = (byte)Math.Max(Math.Min(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 255), 0);
                }
                for (int y = 1; y < heigh - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        Sx = ((dataPtrRead + nChan * (x - 1) + widthStep * (y - 1))[0] + (2 * (dataPtrRead + nChan * (x - 1) + widthStep * y)[0]) + (dataPtrRead + nChan * (x - 1) + widthStep * (y + 1))[0]) -
                             ((dataPtrRead + nChan * (x + 1) + widthStep * (y - 1))[0] + (2 * (dataPtrRead + nChan * (x + 1) + widthStep * y)[0]) + (dataPtrRead + nChan * (x + 1) + widthStep * (y + 1))[0]);

                        Sy = ((dataPtrRead + nChan * (x - 1) + widthStep * (y - 1))[0] + (2 * (dataPtrRead + nChan * x + widthStep * (y - 1))[0]) + (dataPtrRead + nChan * (x + 1) + widthStep * (y - 1))[0]) -
                             ((dataPtrRead + nChan * (x - 1) + widthStep * (y + 1))[0] + (2 * (dataPtrRead + nChan * x + widthStep * (y + 1))[0]) + (dataPtrRead + nChan * (x + 1) + widthStep * (y + 1))[0]);

                        (dataPtrWrite + nChan * x + widthStep * y)[0] = (byte)Math.Max(Math.Min(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 255), 0);

                        Sx = ((dataPtrRead + nChan * (x - 1) + widthStep * (y - 1))[1] + (2 * (dataPtrRead + nChan * (x - 1) + widthStep * y)[1]) + (dataPtrRead + nChan * (x - 1) + widthStep * (y + 1))[1]) -
                             ((dataPtrRead + nChan * (x + 1) + widthStep * (y - 1))[1] + (2 * (dataPtrRead + nChan * (x + 1) + widthStep * y)[1]) + (dataPtrRead + nChan * (x + 1) + widthStep * (y + 1))[1]);

                        Sy = ((dataPtrRead + nChan * (x - 1) + widthStep * (y - 1))[1] + (2 * (dataPtrRead + nChan * x + widthStep * (y - 1))[1]) + (dataPtrRead + nChan * (x + 1) + widthStep * (y - 1))[1]) -
                             ((dataPtrRead + nChan * (x - 1) + widthStep * (y + 1))[1] + (2 * (dataPtrRead + nChan * x + widthStep * (y + 1))[1]) + (dataPtrRead + nChan * (x + 1) + widthStep * (y + 1))[1]);

                        (dataPtrWrite + nChan * x + widthStep * y)[1] = (byte)Math.Max(Math.Min(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 255), 0);

                        Sx = ((dataPtrRead + nChan * (x - 1) + widthStep * (y - 1))[2] + (2 * (dataPtrRead + nChan * (x - 1) + widthStep * y)[2]) + (dataPtrRead + nChan * (x - 1) + widthStep * (y + 1))[2]) -
                             ((dataPtrRead + nChan * (x + 1) + widthStep * (y - 1))[2] + (2 * (dataPtrRead + nChan * (x + 1) + widthStep * y)[2]) + (dataPtrRead + nChan * (x + 1) + widthStep * (y + 1))[2]);

                        Sy = ((dataPtrRead + nChan * (x - 1) + widthStep * (y - 1))[2] + (2 * (dataPtrRead + nChan * x + widthStep * (y - 1))[2]) + (dataPtrRead + nChan * (x + 1) + widthStep * (y - 1))[2]) -
                             ((dataPtrRead + nChan * (x - 1) + widthStep * (y + 1))[2] + (2 * (dataPtrRead + nChan * x + widthStep * (y + 1))[2]) + (dataPtrRead + nChan * (x + 1) + widthStep * (y + 1))[2]);

                        (dataPtrWrite + nChan * x + widthStep * y)[2] = (byte)Math.Max(Math.Min(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 255), 0);
                    }
                }
            }
        }

        /// <summary>
        /// Diferentiation, MISSING SIDES
        /// </summary>
        /// <param name="img"></param>
        /// <param name="imgCopy"></param>
        public static void Diferentiation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            MIplImage write = img.MIplImage;
            MIplImage read = imgCopy.MIplImage;

            int width = imgCopy.Width;
            int height = imgCopy.Height;
            int colorChn = write.nChannels;
            int widthstep = write.widthStep;

            unsafe
            {
                byte* dataPtrRead = (byte*)read.imageData.ToPointer();
                byte* dataPtrWrite = (byte*)write.imageData.ToPointer();

                for (int i = 0; i < colorChn; i++)
                {
                    // Compute the center
                    for (int y = 0; y < height - 1; y++)
                    {
                        for (int x = 0; x < width - 1; x++)
                        {
                            double xValue, yValue;
                            xValue =
                               (dataPtrRead + colorChn * (x) + widthstep * (y))[i] -
                               (dataPtrRead + colorChn * (x + 1) + widthstep * (y))[i];

                            yValue =
                                (dataPtrRead + colorChn * (x) + widthstep * (y))[i] -
                               (dataPtrRead + colorChn * (x) + widthstep * (y + 1))[i];

                            double dif = Math.Abs(xValue) + Math.Abs(yValue);
                            dif = Math.Round(dif);
                            (dataPtrWrite + colorChn * x + widthstep * y)[i] = (byte)Math.Min(Math.Max(dif, 0), 255);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Binarization of a given image
        /// </summary>
        /// <param name="img">image</param>
        /// <param name="treshold">treshold</param>
        public static void ConvertToBW(Image<Bgr, byte> img, int treshold)
        {
            MIplImage write = img.MIplImage;

            int width = img.Width;
            int height = img.Height;
            int colorChn = write.nChannels;
            int widthstep = write.widthStep;

            unsafe
            {
                byte* dataPtrWrite = (byte*)write.imageData.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte* p = dataPtrWrite + colorChn * (x) + widthstep * (y);

                        // Grey of the current pixel
                        float yValue = 16 + 0.257f * p[2] + 0.504f * p[1] + 0.098f * p[0];
                        byte color;

                        if (yValue < treshold)
                            color = 0;
                        else
                            color = 255;

                        p[0] = p[1] = p[2] = color;
                    }
                }
            }
        }

        /// <summary>
        /// Binarization of a given image using the Otsu method to find the treshold
        /// </summary>
        /// <param name="img"></param>
        public static void ConvertToBW_Otsu(Image<Bgr, byte> img)
        {
            ConvertToBW(img, Otsu(img));
        }

        public static int[][] Histogram_All(Image<Bgr, byte> img)
        {
            // G RGB
            int[][] final = new int[4][];
            for (int i = 0; i < final.Length; i++)
            {
                final[i] = new int[256];
            }

            MIplImage read = img.MIplImage;

            int width = img.Width;
            int height = img.Height;
            int colorChn = read.nChannels;
            int widthstep = read.widthStep;

            unsafe
            {
                byte* dataPtrRead = (byte*)read.imageData.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte* p = dataPtrRead + colorChn * (x) + widthstep * (y);
                        float yValue = 16 + 0.257f * p[2] + 0.504f * p[1] + 0.098f * p[0];
                        final[0][(byte)Math.Round(yValue)]++;
                        final[1][p[2]]++;
                        final[2][p[1]]++;
                        final[3][p[0]]++;
                    }
                }
            }
            return final;
        }

        public static int[] Histogram_Gray(Image<Bgr, byte> img)
        {
            // G RGB
            int[] final = new int[256];

            MIplImage read = img.MIplImage;

            int width = img.Width;
            int height = img.Height;
            int colorChn = read.nChannels;
            int widthstep = read.widthStep;

            unsafe
            {
                byte* dataPtrRead = (byte*)read.imageData.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte* p = dataPtrRead + colorChn * (x) + widthstep * (y);
                        float yValue = 16 + 0.257f * p[2] + 0.504f * p[1] + 0.098f * p[0];
                        final[(byte)Math.Round(yValue)]++;
                    }
                }
            }
            return final;
        }

        public static int[][] Histogram_RGB(Image<Bgr, byte> img)
        {
            // G RGB
            int[][] final = new int[3][];
            for (int i = 0; i < final.Length; i++)
            {
                final[i] = new int[256];
            }

            MIplImage read = img.MIplImage;

            int width = img.Width;
            int height = img.Height;
            int colorChn = read.nChannels;
            int widthstep = read.widthStep;

            unsafe
            {
                byte* dataPtrRead = (byte*)read.imageData.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte* p = dataPtrRead + colorChn * (x) + widthstep * (y);
                        final[0][p[2]]++;
                        final[1][p[1]]++;
                        final[2][p[0]]++;
                    }
                }
            }
            return final;
        }

        /// <summary>
        /// Otsu Treshold given an image
        /// </summary>
        /// <returns>Treshold to be used in binarization</returns>
        public static int Otsu(Image<Bgr, byte> img)
        {
            // Compute histogram part of the algorithm
            // Probabilities etc... 
            int[] histogram = ImageClass.Histogram_Gray(img);
            float[] probabilities = new float[histogram.Length];
            int pixelCount = img.Width * img.Height;

            for (int i = 0; i < histogram.Length; i++)
            {
                probabilities[i] = (float)histogram[i] / (float)pixelCount;
            }

            int treshold = 0;
            float max = 0;


            // Somatorio de probabilidades 0 a t
            float q1 = 0;
            // Somatorio de probabilidades de t a 255
            float q2 = 0;
            float q12;

            for (int t = 0; t < 256; t++)
            {
                q1 = sum(0, t, probabilities);
                q2 = sum(t + 1, 255, probabilities);
                q12 = q1 * q2;
                float u12 = (weightedSum(0, t, probabilities) / q1) - (weightedSum(t + 1, 255, probabilities) / q2);
                float ots = q12 * u12 * u12;

                if (ots >= max)
                {
                    treshold = t;
                    max = ots;
                }
            }

            return treshold;
        }

        /// <summary>
        /// Sums the probabilities from the array from start index to end index in a handy onliner
        /// </summary>
        /// <param name="start">Start index</param>
        /// <param name="end">End index</param>
        /// <param name="probabilities">array containing probabilities</param>
        /// <returns>Comulative probabilty</returns>
        public static float sum(int start, int end, float[] probabilities)
        {
            float sum = 0;
            int i;
            for (i = start; i <= end; i++)
            {
                sum += probabilities[i];
            }

            return sum;
        }

        /// <summary>
        /// Does the same as sum, but weighing in the index of the array, used in Otsu
        /// </summary>
        /// <param name="start">start index</param>
        /// <param name="end">end index</param>
        /// <param name="probabilities">probabilities to sum</param>
        /// <returns>Comulative weighted probability</returns>
        public static float weightedSum(int start, int end, float[] probabilities)
        {
            float sum = 0;
            int i;
            for (i = start; i <= end; i++)
            {
                sum += i * probabilities[i];
            }

            return sum;
        }


        /// <summary>
        /// Rotates a single puzzle piece on it's center
        /// </summary>
        /// <param name="dataPtrWrite"></param>
        /// <param name="dataPtrRead"></param>
        /// <param name="nChan"></param>
        /// <param name="widthStep"></param>
        /// <param name="piece"></param>
        /// <param name="angle"></param>
        public unsafe static void Rotation(byte* dataPtrWrite, byte* dataPtrRead, int nChan, int widthStep, PuzzlePiece piece, float angle)
        {
            unsafe
            {
                byte red, green, blue;
                int x0, y0;

                double W = piece.Top.x + (piece.Width / 2f);
                double H = piece.Top.y + (piece.Height / 2f);

                for (int y = piece.Top.y; y < piece.Bottom.y; y++)
                {
                    for (int x = piece.Top.x; x < piece.Bottom.x; x++)
                    {

                        double cos = Math.Cos(angle);
                        double sin = Math.Sin(angle);

                        x0 = (int)Math.Round((x - W) * cos - (H - y) * sin + W);
                        y0 = (int)Math.Round(H - (x - W) * sin - (H - y) * cos);

                        if (x0 >= piece.Top.x && x0 < piece.Bottom.x && y0 >= piece.Top.y && y0 < piece.Bottom.y)
                        {
                            red = (dataPtrRead + nChan * x0 + widthStep * y0)[2];
                            green = (dataPtrRead + nChan * x0 + widthStep * y0)[1];
                            blue = (dataPtrRead + nChan * x0 + widthStep * y0)[0];
                        }
                        else
                        {
                            red = 0;
                            green = 0;
                            blue = 0;
                        }

                        (dataPtrWrite + nChan * x + widthStep * y)[2] = red;
                        (dataPtrWrite + nChan * x + widthStep * y)[1] = green;
                        (dataPtrWrite + nChan * x + widthStep * y)[0] = blue;
                    }
                }
            }
        }
        /// <summary>
        /// Connected components algorithm, it takes the 0,0 pixel as the
        /// background color and finds and tags any images inside.
        /// 
        /// TODO: 
        /// - Optimize further. It takes about ~180 ms in my PC, making it
        /// quite slow.
        /// - Sort the tagged images into an image array or something
        /// </summary>
        /// <param name="img"></param>
        /// <param name="imgCopy"></param>
        public static void DetectIndependentObjects(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {

                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrRead = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrWrite = (byte*)m.imageData.ToPointer(); // It can be deleted after

                int width = imgCopy.Width;
                int heigh = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;

                // Stores the current tag as key and the equity tag 
                int[] linked = new int[width * heigh];

                // Temporarily stores the neighbors of the current pixel - Not sure about the performance cost tho.
                int[] neighbors = new int[2];

                // Contains the labels for each pixel
                int[,] labels = new int[width, heigh];

                // The next label to be given
                int nextLabel = 1;

                for (int y = 0; y < heigh; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Checks if the pixel is not exactly like the background
                        if ((dataPtrRead + nChan * x + widthStep * y)[0] != dataPtrRead[0] ||
                            (dataPtrRead + nChan * x + widthStep * y)[1] != dataPtrRead[1] ||
                            (dataPtrRead + nChan * x + widthStep * y)[2] != dataPtrRead[2])
                        {
                            int nNeigh = 0;

                            // Checks the left pixel
                            if (x - 1 >= 0 && x - 1 < width && labels[x - 1, y] != 0)
                            {
                                neighbors[nNeigh] = labels[x - 1, y];
                                nNeigh++;
                            }
                            // Checks the right pixel
                            if (y - 1 >= 0 && y - 1 < heigh && labels[x, y - 1] != 0)
                            {
                                neighbors[nNeigh] = labels[x, y - 1];
                                nNeigh++;
                            }

                            // If it has no neighbors creates a new link 
                            // (to itself) and increments the label
                            if (nNeigh == 0)
                            {
                                labels[x, y] = nextLabel;
                                //linked.Add(nextLabel, nextLabel);
                                linked[nextLabel] = nextLabel;
                                nextLabel++;
                            }
                            else
                            {
                                // If it contains 1 neighbor the lowest is that
                                // one, otherwise, finds the smallest
                                int lowest = nNeigh == 1 ?
                                    neighbors[0] : Math.Min(neighbors[0], neighbors[1]);


                                labels[x, y] = lowest;

                                // Cycles through the neighbors
                                for (int i = 0; i < nNeigh; i++)
                                {
                                    // If the lowest value is less than the
                                    // current, by the parent recursively
                                    if (lowest < linked[neighbors[i]])
                                    {
                                        int newTag = labels[x, y];
                                        int prevTag = 0;

                                        while (newTag != prevTag)
                                        {
                                            prevTag = newTag;
                                            newTag = linked[newTag];
                                        }
                                        linked[neighbors[i]] = newTag;
                                    }
                                }
                            }
                        }
                    }
                }
                Dictionary<int, int[]> images = new Dictionary<int, int[]>();

                for (int y = 0; y < heigh; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (labels[x, y] == 0) continue;

                        // This part can be only above, but it fucks up the
                        // top edges on diagonals, but removing on top for
                        // this one only slows it down ALOT
                        int newTag = labels[x, y];
                        int prevTag = 0;

                        while (newTag != prevTag)
                        {
                            prevTag = newTag;
                            newTag = linked[newTag];
                        }
                        labels[x, y] = newTag;

                        if (images.ContainsKey(newTag))
                        {
                            if (x < images[newTag][0])
                                images[newTag][0] = x;
                            if (y < images[newTag][1])
                                images[newTag][1] = y;

                            if (x > images[newTag][2])
                                images[newTag][2] = x;
                            if (y > images[newTag][3])
                                images[newTag][3] = y;
                        }
                        else
                        {
                            images.Add(newTag, new int[] { x, y, x, y });
                        }
                    }
                }

                // Basically to transform a dictionary with unknown keys to
                // an array where each unique image has +1 of the previous id:
                // image 1 = index 763
                // image 2 = index 129
                //
                // to
                //
                // image 1 = index 0
                // image 2 = index 1
                PuzzlePiece[] imgs = new PuzzlePiece[images.Keys.Count];
                int n = 0;

                foreach (int k in images.Keys)
                {
                    Vector2Int top = new Vector2Int(images[k][0], images[k][1]);
                    Vector2Int bot = new Vector2Int(images[k][2], images[k][3]);
                    imgs[n] = new PuzzlePiece(top, bot);

                    // Check if the image is rotated
                    if ((dataPtrRead + nChan * imgs[n].Top.x + widthStep * imgs[n].Top.y)[0] == dataPtrRead[0] ||
                        (dataPtrRead + nChan * imgs[n].Top.x + widthStep * imgs[n].Top.y)[1] == dataPtrRead[1] ||
                        (dataPtrRead + nChan * imgs[n].Top.x + widthStep * imgs[n].Top.y)[2] == dataPtrRead[2])
                    {
                        double delta = imgs[n].ImageAngle();
                        Rotation(dataPtrWrite, dataPtrRead, nChan, widthStep, imgs[n], -(float)delta);
                    }

                    // Draw a bounding box around each unique image
                    for (int x = imgs[n].Top.x; x < imgs[n].Bottom.x; x++)
                    {
                        (dataPtrWrite + nChan * x + widthStep * imgs[n].Top.y)[0] = 0;
                        (dataPtrWrite + nChan * x + widthStep * imgs[n].Top.y)[1] = 0;
                        (dataPtrWrite + nChan * x + widthStep * imgs[n].Top.y)[2] = 255;

                        (dataPtrWrite + nChan * x + widthStep * imgs[n].Bottom.y)[0] = 0;
                        (dataPtrWrite + nChan * x + widthStep * imgs[n].Bottom.y)[1] = 0;
                        (dataPtrWrite + nChan * x + widthStep * imgs[n].Bottom.y)[2] = 255;
                    }
                    for (int y = imgs[n].Top.y; y < imgs[n].Bottom.y + 1; y++)
                    {
                        (dataPtrWrite + nChan * imgs[n].Top.x + widthStep * y)[0] = 0;
                        (dataPtrWrite + nChan * imgs[n].Top.x + widthStep * y)[1] = 0;
                        (dataPtrWrite + nChan * imgs[n].Top.x + widthStep * y)[2] = 255;

                        (dataPtrWrite + nChan * imgs[n].Bottom.x + widthStep * y)[0] = 0;
                        (dataPtrWrite + nChan * imgs[n].Bottom.x + widthStep * y)[1] = 0;
                        (dataPtrWrite + nChan * imgs[n].Bottom.x + widthStep * y)[2] = 255;
                    }
                    n++;
                }
            }
        }

        /// <summary>
        /// Function that solves the puzzle
        /// </summary>
        /// <param name="img">Input/Output image</param>
        /// <param name="imgCopy">Image Copy</param>
        /// <param name="Pieces_positions">List of positions (Left-x,Top-y,Right-x,Bottom-y) of all detected pieces</param>
        /// <param name="Pieces_angle">List of detected pieces' angles</param>
        /// <param name="level">Level of image</param>
        public static void puzzle(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, out List<int[]> Pieces_positions, out List<int> Pieces_angle, int level)
        {
            Pieces_positions = new List<int[]>();
            int[] piece_vector = new int[4];
            Bgr backgroundColor = new Bgr(0, 0, 0);
            unsafe
            {
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrRead = (byte*)mUndo.imageData.ToPointer();
                backgroundColor.Red = dataPtrRead[0];
                backgroundColor.Green = dataPtrRead[1];
                backgroundColor.Blue = dataPtrRead[2];
            }

            piece_vector[0] = 65;   // x- Top-Left 
            piece_vector[1] = 385;  // y- Top-Left
            piece_vector[2] = 1089; // x- Bottom-Right
            piece_vector[3] = 1411; // y- Bottom-Right

            Pieces_positions.Add(piece_vector);

            Pieces_angle = new List<int>();
            Pieces_angle.Add(0); // angle
        }
    }
}