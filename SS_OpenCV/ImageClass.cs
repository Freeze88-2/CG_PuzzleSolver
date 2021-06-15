using Emgu.CV;
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

                for (int y = 0; y < heigh; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        double W = width / 2f;
                        double H = heigh / 2f;

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
        /// Combine the 3 channels devide it by 3 that's the index (so 255 max histogram width)
        /// add one to that index
        /// </summary>
        /// <param name="img"></param>
        /// <param name="imgCopy"></param>
        /// <returns></returns>
        public static int[] GetImageHistogram(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
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

                int[] vs = new int[256];

                for (int y = 1; y < heigh - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        int index = ((dataPtrRead + nChan * x + widthStep * y)[0] + (dataPtrRead + nChan * x + widthStep * y)[1] + (dataPtrRead + nChan * x + widthStep * y)[2]) / 3;

                        vs[index] += 1;
                    }
                }
                return vs;
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
                                    if (lowest < linked[neighbors[i]] )
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
                        // top edges on diagonals
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
                // an array of with a position array inside:
                // image 1 = index 763
                // image 2 = index 129
                //
                // to
                //
                // first image = index 0
                // second image = index 1
                Rectangle[] imgs = new Rectangle[images.Keys.Count];
                int n = 0;

                foreach (int k in images.Keys)
                {
                    imgs[n] = new Rectangle(images[k][0], images[k][1], images[k][2] - images[k][0], images[k][3] - images[k][1]);

                    for (int x = imgs[n].Left; x < imgs[n].Right; x++)
                    {
                        (dataPtrWrite + nChan * x + widthStep * imgs[n].Top)[0] = 0;
                        (dataPtrWrite + nChan * x + widthStep * imgs[n].Top)[1] = 0;
                        (dataPtrWrite + nChan * x + widthStep * imgs[n].Top)[2] = 255;

                        (dataPtrWrite + nChan * x + widthStep * imgs[n].Bottom)[0] = 0;
                        (dataPtrWrite + nChan * x + widthStep * imgs[n].Bottom)[1] = 0;
                        (dataPtrWrite + nChan * x + widthStep * imgs[n].Bottom)[2] = 255;
                    }
                    for (int y = imgs[n].Top; y < imgs[n].Bottom +1; y++)
                    {
                        (dataPtrWrite + nChan * imgs[n].Left + widthStep * y)[0] = 0;
                        (dataPtrWrite + nChan * imgs[n].Left + widthStep * y)[1] = 0;
                        (dataPtrWrite + nChan * imgs[n].Left + widthStep * y)[2] = 255;

                        (dataPtrWrite + nChan * imgs[n].Right + widthStep * y)[0] = 0;
                        (dataPtrWrite + nChan * imgs[n].Right + widthStep * y)[1] = 0;
                        (dataPtrWrite + nChan * imgs[n].Right + widthStep * y)[2] = 255;
                    }
                    n++;
                }
            }
        }
    }
}