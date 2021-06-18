using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;
                byte red, green, blue;
                int x0, y0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        x0 = x - dx;
                        y0 = y - dy;

                        if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)
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
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;
                byte red, green, blue;
                int x0, y0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        x0 = (int)Math.Round(x / scaleFactor);
                        y0 = (int)Math.Round(y / scaleFactor);

                        if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)
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
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;
                byte red, green, blue;
                int x0, y0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        x0 = (int)Math.Round(((x - width / 2) / scaleFactor) + centerX);
                        y0 = (int)Math.Round(((y - height / 2) / scaleFactor) + centerY);

                        if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)
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
                int height = imgCopy.Height;
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

                (dataPtrWrite + nChan * (width - 1) + widthStep * (height - 1))[0] =
                   (byte)Math.Min(Math.Max(Math.Round((
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[0] * matrix[2, 2]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[0] * matrix[2, 1]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[0] * matrix[1, 2]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[0] * matrix[1, 1]) +

                        ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 2))[0] * matrix[1, 0]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 2))[0] * matrix[2, 0]) +

                        ((dataPtrRead + nChan * (width - 2) + widthStep * (height - 1))[0] * matrix[0, 1]) +
                        ((dataPtrRead + nChan * (width - 2) + widthStep * (height - 1))[0] * matrix[0, 2]) +

                        ((dataPtrRead + nChan * (width - 2) + widthStep * (height - 2))[0] * matrix[0, 0]))
                        / matrixWeight), 0), 255);

                (dataPtrWrite + nChan * (width - 1) + widthStep * (height - 1))[1] =
                   (byte)Math.Min(Math.Max(Math.Round((
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[1] * matrix[2, 2]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[1] * matrix[2, 1]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[1] * matrix[1, 2]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[1] * matrix[1, 1]) +

                        ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 2))[1] * matrix[1, 0]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 2))[1] * matrix[2, 0]) +

                        ((dataPtrRead + nChan * (width - 2) + widthStep * (height - 1))[1] * matrix[0, 1]) +
                        ((dataPtrRead + nChan * (width - 2) + widthStep * (height - 1))[1] * matrix[0, 2]) +

                        ((dataPtrRead + nChan * (width - 2) + widthStep * (height - 2))[1] * matrix[0, 0]))
                        / matrixWeight), 0), 255);

                (dataPtrWrite + nChan * (width - 1) + widthStep * (height - 1))[2] =
                    (byte)Math.Min(Math.Max(Math.Round((
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[2] * matrix[2, 2]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[2] * matrix[2, 1]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[2] * matrix[1, 2]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[2] * matrix[1, 1]) +

                        ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 2))[2] * matrix[1, 0]) +
                        ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 2))[2] * matrix[2, 0]) +

                        ((dataPtrRead + nChan * (width - 2) + widthStep * (height - 1))[2] * matrix[0, 1]) +
                        ((dataPtrRead + nChan * (width - 2) + widthStep * (height - 1))[2] * matrix[0, 2]) +

                        ((dataPtrRead + nChan * (width - 2) + widthStep * (height - 2))[2] * matrix[0, 0]))
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

                (dataPtrWrite + widthStep * (height - 1))[0] = (byte)Math.Min(Math.Max(Math.Round((
                    ((dataPtrRead + widthStep * (height - 1))[0] * matrix[0, 2]) +
                    ((dataPtrRead + widthStep * (height - 1))[0] * matrix[0, 1]) +
                    ((dataPtrRead + widthStep * (height - 1))[0] * matrix[1, 2]) +
                    ((dataPtrRead + widthStep * (height - 1))[0] * matrix[1, 1]) +

                    ((dataPtrRead + widthStep * (height - 2))[0] * matrix[0, 0]) +
                    ((dataPtrRead + widthStep * (height - 2))[0] * matrix[1, 0]) +

                    ((dataPtrRead + nChan + widthStep * (height - 1))[0] * matrix[2, 2]) +
                    ((dataPtrRead + nChan + widthStep * (height - 1))[0] * matrix[2, 1]) +

                    ((dataPtrRead + nChan + widthStep * (height - 2))[0] * matrix[2, 0]))
                    / matrixWeight), 0), 255);

                (dataPtrWrite + widthStep * (height - 1))[1] = (byte)Math.Min(Math.Max(Math.Round((
                    ((dataPtrRead + widthStep * (height - 1))[1] * matrix[0, 2]) +
                    ((dataPtrRead + widthStep * (height - 1))[1] * matrix[0, 1]) +
                    ((dataPtrRead + widthStep * (height - 1))[1] * matrix[1, 2]) +
                    ((dataPtrRead + widthStep * (height - 1))[1] * matrix[1, 1]) +

                    ((dataPtrRead + widthStep * (height - 2))[1] * matrix[0, 0]) +
                    ((dataPtrRead + widthStep * (height - 2))[1] * matrix[1, 0]) +

                    ((dataPtrRead + nChan + widthStep * (height - 1))[1] * matrix[2, 2]) +
                    ((dataPtrRead + nChan + widthStep * (height - 1))[1] * matrix[2, 1]) +

                    ((dataPtrRead + nChan + widthStep * (height - 2))[1] * matrix[2, 0]))
                    / matrixWeight), 0), 255);

                (dataPtrWrite + widthStep * (height - 1))[2] = (byte)Math.Min(Math.Max(Math.Round((
                    ((dataPtrRead + widthStep * (height - 1))[2] * matrix[0, 2]) +
                    ((dataPtrRead + widthStep * (height - 1))[2] * matrix[0, 1]) +
                    ((dataPtrRead + widthStep * (height - 1))[2] * matrix[1, 2]) +
                    ((dataPtrRead + widthStep * (height - 1))[2] * matrix[1, 1]) +

                    ((dataPtrRead + widthStep * (height - 2))[2] * matrix[0, 0]) +
                    ((dataPtrRead + widthStep * (height - 2))[2] * matrix[1, 0]) +

                    ((dataPtrRead + nChan + widthStep * (height - 1))[2] * matrix[2, 2]) +
                    ((dataPtrRead + nChan + widthStep * (height - 1))[2] * matrix[2, 1]) +

                    ((dataPtrRead + nChan + widthStep * (height - 2))[2] * matrix[2, 0]))
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

                    (dataPtrWrite + nChan * x + widthStep * (height - 1))[0] = (byte)Math.Min(Math.Max(Math.Round((((dataPtrRead + nChan * (x - 1) + widthStep * (height - 1))[0] * matrix[0, 2]) + ((dataPtrRead + nChan * x + widthStep * (height - 1))[0] * matrix[1, 2]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (height - 1))[0] * matrix[2, 2]) +
                                                                                     ((dataPtrRead + nChan * (x - 1) + widthStep * (height - 1))[0] * matrix[0, 1]) + ((dataPtrRead + nChan * x + widthStep * (height - 1))[0] * matrix[1, 1]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (height - 1))[0] * matrix[2, 1]) +
                                                                                     ((dataPtrRead + nChan * (x - 1) + widthStep * (height - 2))[0] * matrix[0, 0]) + ((dataPtrRead + nChan * x + widthStep * (height - 2))[0] * matrix[1, 0]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (height - 2))[0] * matrix[2, 0])) / matrixWeight), 0), 255);

                    (dataPtrWrite + nChan * x + widthStep * (height - 1))[1] = (byte)Math.Min(Math.Max(Math.Round((((dataPtrRead + nChan * (x - 1) + widthStep * (height - 1))[1] * matrix[0, 2]) + ((dataPtrRead + nChan * x + widthStep * (height - 1))[1] * matrix[1, 2]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (height - 1))[1] * matrix[2, 2]) +
                                                                                     ((dataPtrRead + nChan * (x - 1) + widthStep * (height - 1))[1] * matrix[0, 1]) + ((dataPtrRead + nChan * x + widthStep * (height - 1))[1] * matrix[1, 1]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (height - 1))[1] * matrix[2, 1]) +
                                                                                     ((dataPtrRead + nChan * (x - 1) + widthStep * (height - 2))[1] * matrix[0, 0]) + ((dataPtrRead + nChan * x + widthStep * (height - 2))[1] * matrix[1, 0]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (height - 2))[1] * matrix[2, 0])) / matrixWeight), 0), 255);

                    (dataPtrWrite + nChan * x + widthStep * (height - 1))[2] = (byte)Math.Min(Math.Max(Math.Round((((dataPtrRead + nChan * (x - 1) + widthStep * (height - 1))[2] * matrix[0, 2]) + ((dataPtrRead + nChan * x + widthStep * (height - 1))[2] * matrix[1, 2]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (height - 1))[2] * matrix[2, 2]) +
                                                                                     ((dataPtrRead + nChan * (x - 1) + widthStep * (height - 1))[2] * matrix[0, 1]) + ((dataPtrRead + nChan * x + widthStep * (height - 1))[2] * matrix[1, 1]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (height - 1))[2] * matrix[2, 1]) +
                                                                                     ((dataPtrRead + nChan * (x - 1) + widthStep * (height - 2))[2] * matrix[0, 0]) + ((dataPtrRead + nChan * x + widthStep * (height - 2))[2] * matrix[1, 0]) + ((dataPtrRead + nChan * (x + 1) + widthStep * (height - 2))[2] * matrix[2, 0])) / matrixWeight), 0), 255);
                }
                for (int y = 1; y < height - 1; y++)
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

                for (int y = 1; y < height - 1; y++)
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
                int height = imgCopy.Height;
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

                (dataPtrWrite + nChan * (width - 1) + widthStep * (height - 1))[0] =
                    (byte)Math.Round(
                        (4 * (dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[0] +
                        2 * (dataPtrRead + nChan * (width - 1) + widthStep * (height - 2))[0] +
                        2 * (dataPtrRead + nChan * (width - 2) + widthStep * (height - 1))[0] +
                        (dataPtrRead + nChan * (width - 2) + widthStep * (height - 2))[0]) / 9.0);

                (dataPtrWrite + nChan * (width - 1) + widthStep * (height - 1))[1] =
                    (byte)Math.Round(
                        (4 * (dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[1] +
                        2 * (dataPtrRead + nChan * (width - 1) + widthStep * (height - 2))[1] +
                        2 * (dataPtrRead + nChan * (width - 2) + widthStep * (height - 1))[1] +
                        (dataPtrRead + nChan * (width - 2) + widthStep * (height - 2))[1]) / 9.0);

                (dataPtrWrite + nChan * (width - 1) + widthStep * (height - 1))[2] =
                    (byte)Math.Round(
                        (4 * (dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[2] +
                        2 * (dataPtrRead + nChan * (width - 1) + widthStep * (height - 2))[2] +
                        2 * (dataPtrRead + nChan * (width - 2) + widthStep * (height - 1))[2] +
                        (dataPtrRead + nChan * (width - 2) + widthStep * (height - 2))[2]) / 9.0);

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

                (dataPtrWrite + widthStep * (height - 1))[0] = (byte)Math.Round((4 * (dataPtrRead + widthStep * (height - 1))[0] + 2 * (dataPtrRead + widthStep * (height - 2))[0] + 2 * (dataPtrRead + nChan + widthStep * (height - 1))[0] + (dataPtrRead + nChan + widthStep * (height - 2))[0]) / 9.0);
                (dataPtrWrite + widthStep * (height - 1))[1] = (byte)Math.Round((4 * (dataPtrRead + widthStep * (height - 1))[1] + 2 * (dataPtrRead + widthStep * (height - 2))[1] + 2 * (dataPtrRead + nChan + widthStep * (height - 1))[1] + (dataPtrRead + nChan + widthStep * (height - 2))[1]) / 9.0);
                (dataPtrWrite + widthStep * (height - 1))[2] = (byte)Math.Round((4 * (dataPtrRead + widthStep * (height - 1))[2] + 2 * (dataPtrRead + widthStep * (height - 2))[2] + 2 * (dataPtrRead + nChan + widthStep * (height - 1))[2] + (dataPtrRead + nChan + widthStep * (height - 2))[2]) / 9.0);

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

                    (dataPtrWrite + nChan * x + widthStep * (height - 1))[0] = (byte)Math.Round(((dataPtrRead + nChan * (x - 1) + widthStep * (height - 1))[0] + (dataPtrRead + nChan * x + widthStep * (height - 1))[0] + (dataPtrRead + nChan * (x + 1) + widthStep * (height - 1))[0] +
                                                                                 (dataPtrRead + nChan * (x - 1) + widthStep * (height - 1))[0] + (dataPtrRead + nChan * x + widthStep * (height - 1))[0] + (dataPtrRead + nChan * (x + 1) + widthStep * (height - 1))[0] +
                                                                                 (dataPtrRead + nChan * (x - 1) + widthStep * (height - 2))[0] + (dataPtrRead + nChan * x + widthStep * (height - 2))[0] + (dataPtrRead + nChan * (x + 1) + widthStep * (height - 2))[0]) / 9.0);

                    (dataPtrWrite + nChan * x + widthStep * (height - 1))[1] = (byte)Math.Round(((dataPtrRead + nChan * (x - 1) + widthStep * (height - 1))[1] + (dataPtrRead + nChan * x + widthStep * (height - 1))[1] + (dataPtrRead + nChan * (x + 1) + widthStep * (height - 1))[1] +
                                                                                 (dataPtrRead + nChan * (x - 1) + widthStep * (height - 1))[1] + (dataPtrRead + nChan * x + widthStep * (height - 1))[1] + (dataPtrRead + nChan * (x + 1) + widthStep * (height - 1))[1] +
                                                                                 (dataPtrRead + nChan * (x - 1) + widthStep * (height - 2))[1] + (dataPtrRead + nChan * x + widthStep * (height - 2))[1] + (dataPtrRead + nChan * (x + 1) + widthStep * (height - 2))[1]) / 9.0);

                    (dataPtrWrite + nChan * x + widthStep * (height - 1))[2] = (byte)Math.Round(((dataPtrRead + nChan * (x - 1) + widthStep * (height - 1))[2] + (dataPtrRead + nChan * x + widthStep * (height - 1))[2] + (dataPtrRead + nChan * (x + 1) + widthStep * (height - 1))[2] +
                                                                                 (dataPtrRead + nChan * (x - 1) + widthStep * (height - 1))[2] + (dataPtrRead + nChan * x + widthStep * (height - 1))[2] + (dataPtrRead + nChan * (x + 1) + widthStep * (height - 1))[2] +
                                                                                 (dataPtrRead + nChan * (x - 1) + widthStep * (height - 2))[2] + (dataPtrRead + nChan * x + widthStep * (height - 2))[2] + (dataPtrRead + nChan * (x + 1) + widthStep * (height - 2))[2]) / 9.0);
                }
                for (int y = 1; y < height - 1; y++)
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

                for (int y = 1; y < height - 1; y++)
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
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;
                byte red, green, blue;
                int x0, y0;

                double W = width / 2f;
                double H = height / 2f;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        double cos = Math.Cos(angle);
                        double sin = Math.Sin(angle);

                        x0 = (int)Math.Round((x - W) * cos - (H - y) * sin + W);
                        y0 = (int)Math.Round(H - (x - W) * sin - (H - y) * cos);

                        if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)
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
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;

                double[,] distsTotal = new double[3, 3];

                for (int y = 1; y < height - 1; y++)
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
                int height = imgCopy.Height;
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
                Sx = ((dataPtrRead + nChan * (width - 2) + widthStep * (height - 2))[0] + (3 * (dataPtrRead + nChan * (width - 2) + widthStep * (height - 1))[0])) -
                     ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 2))[0] + (3 * (dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[0]));

                Sy = (((3 * (dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[0]) + (dataPtrRead + nChan * (width - 2) + widthStep * (height - 1))[0])) -
                     (((3 * (dataPtrRead + nChan * (width - 1) + widthStep * (height - 2))[0]) + (dataPtrRead + nChan * (width - 2) + widthStep * (height - 2))[0]));

                (dataPtrWrite + nChan * (width - 1) + widthStep * (height - 1))[0] =
                   (byte)Math.Min(Math.Max(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 0), 255);

                Sx = ((dataPtrRead + nChan * (width - 2) + widthStep * (height - 2))[1] + (3 * (dataPtrRead + nChan * (width - 2) + widthStep * (height - 1))[1])) -
                     ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 2))[1] + (3 * (dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[1]));

                Sy = (((3 * (dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[1]) + (dataPtrRead + nChan * (width - 2) + widthStep * (height - 1))[1])) -
                     (((3 * (dataPtrRead + nChan * (width - 1) + widthStep * (height - 2))[1]) + (dataPtrRead + nChan * (width - 2) + widthStep * (height - 2))[1]));

                (dataPtrWrite + nChan * (width - 1) + widthStep * (height - 1))[1] =
                   (byte)Math.Min(Math.Max(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 0), 255);

                Sx = ((dataPtrRead + nChan * (width - 2) + widthStep * (height - 2))[2] + (3 * (dataPtrRead + nChan * (width - 2) + widthStep * (height - 1))[2])) -
                     ((dataPtrRead + nChan * (width - 1) + widthStep * (height - 2))[2] + (3 * (dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[2]));

                Sy = (((3 * (dataPtrRead + nChan * (width - 1) + widthStep * (height - 1))[2]) + (dataPtrRead + nChan * (width - 2) + widthStep * (height - 1))[2])) -
                     (((3 * (dataPtrRead + nChan * (width - 1) + widthStep * (height - 2))[2]) + (dataPtrRead + nChan * (width - 2) + widthStep * (height - 2))[2]));

                (dataPtrWrite + nChan * (width - 1) + widthStep * (height - 1))[2] =
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
                Sx = ((dataPtrRead + widthStep * (height - 2))[0] + (3 * (dataPtrRead + widthStep * (height - 1))[0])) -
                     ((dataPtrRead + nChan + widthStep * (height - 2))[0] + (3 * (dataPtrRead + nChan + widthStep * (height - 1))[0]));

                Sy = ((3 * (dataPtrRead + widthStep * (height - 1))[0]) + (dataPtrRead + nChan + widthStep * (height - 1))[0]) -
                     ((3 * (dataPtrRead + widthStep * (height - 2))[0]) + (dataPtrRead + nChan + widthStep * (height - 2))[0]);

                (dataPtrWrite + widthStep * (height - 1))[0] = (byte)Math.Min(Math.Max(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 0), 255);

                Sx = ((dataPtrRead + widthStep * (height - 2))[1] + (3 * (dataPtrRead + widthStep * (height - 1))[1])) -
                     ((dataPtrRead + nChan + widthStep * (height - 2))[1] + (3 * (dataPtrRead + nChan + widthStep * (height - 1))[1]));

                Sy = ((3 * (dataPtrRead + widthStep * (height - 1))[1]) + (dataPtrRead + nChan + widthStep * (height - 1))[1]) -
                     ((3 * (dataPtrRead + widthStep * (height - 2))[1]) + (dataPtrRead + nChan + widthStep * (height - 2))[1]);

                (dataPtrWrite + widthStep * (height - 1))[1] = (byte)Math.Min(Math.Max(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 0), 255);

                Sx = ((dataPtrRead + widthStep * (height - 2))[2] + (3 * (dataPtrRead + widthStep * (height - 1))[2])) -
                     ((dataPtrRead + nChan + widthStep * (height - 2))[2] + (3 * (dataPtrRead + nChan + widthStep * (height - 1))[2]));

                Sy = ((3 * (dataPtrRead + widthStep * (height - 1))[2]) + (dataPtrRead + nChan + widthStep * (height - 1))[2]) -
                     ((3 * (dataPtrRead + widthStep * (height - 2))[2]) + (dataPtrRead + nChan + widthStep * (height - 2))[2]);

                (dataPtrWrite + widthStep * (height - 1))[2] = (byte)Math.Min(Math.Max(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 0), 255);

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
                    Sx = ((3 * (dataPtrRead + nChan * (x - 1) + widthStep * (height - 1))[0]) + (dataPtrRead + nChan * (x - 1) + widthStep * (height - 2))[0]) -
                         ((3 * (dataPtrRead + nChan * (x + 1) + widthStep * (height - 1))[0]) + (dataPtrRead + nChan * (x + 1) + widthStep * (height - 2))[0]);

                    Sy = ((dataPtrRead + nChan * (x - 1) + widthStep * (height - 1))[0] + (2 * (dataPtrRead + nChan * x + widthStep * (height - 1))[0]) + (dataPtrRead + nChan * (x + 1) + widthStep * (height - 1))[0]) -
                         ((dataPtrRead + nChan * (x - 1) + widthStep * (height - 2))[0] + (2 * (dataPtrRead + nChan * x + widthStep * (height - 2))[0]) + (dataPtrRead + nChan * (x + 1) + widthStep * (height - 2))[0]);

                    (dataPtrWrite + nChan * x + widthStep * (height - 1))[0] = (byte)Math.Max(Math.Min(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 255), 0);

                    Sx = ((3 * (dataPtrRead + nChan * (x - 1) + widthStep * (height - 1))[1]) + (dataPtrRead + nChan * (x - 1) + widthStep * (height - 2))[1]) -
                         ((3 * (dataPtrRead + nChan * (x + 1) + widthStep * (height - 1))[1]) + (dataPtrRead + nChan * (x + 1) + widthStep * (height - 2))[1]);

                    Sy = ((dataPtrRead + nChan * (x - 1) + widthStep * (height - 1))[1] + (2 * (dataPtrRead + nChan * x + widthStep * (height - 1))[1]) + (dataPtrRead + nChan * (x + 1) + widthStep * (height - 1))[1]) -
                         ((dataPtrRead + nChan * (x - 1) + widthStep * (height - 2))[1] + (2 * (dataPtrRead + nChan * x + widthStep * (height - 2))[1]) + (dataPtrRead + nChan * (x + 1) + widthStep * (height - 2))[1]);

                    (dataPtrWrite + nChan * x + widthStep * (height - 1))[1] = (byte)Math.Max(Math.Min(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 255), 0);

                    Sx = ((3 * (dataPtrRead + nChan * (x - 1) + widthStep * (height - 1))[2]) + (dataPtrRead + nChan * (x - 1) + widthStep * (height - 2))[2]) -
                         ((3 * (dataPtrRead + nChan * (x + 1) + widthStep * (height - 1))[2]) + (dataPtrRead + nChan * (x + 1) + widthStep * (height - 2))[2]);

                    Sy = ((dataPtrRead + nChan * (x - 1) + widthStep * (height - 1))[2] + (2 * (dataPtrRead + nChan * x + widthStep * (height - 1))[2]) + (dataPtrRead + nChan * (x + 1) + widthStep * (height - 1))[2]) -
                         ((dataPtrRead + nChan * (x - 1) + widthStep * (height - 2))[2] + (2 * (dataPtrRead + nChan * x + widthStep * (height - 2))[2]) + (dataPtrRead + nChan * (x + 1) + widthStep * (height - 2))[2]);

                    (dataPtrWrite + nChan * x + widthStep * (height - 1))[2] = (byte)Math.Max(Math.Min(Math.Round(Math.Abs(Sx) + Math.Abs(Sy)), 255), 0);
                }
                for (int y = 1; y < height - 1; y++)
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
                for (int y = 1; y < height - 1; y++)
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
        public unsafe static void Rotation(byte* dataPtrWrite, byte* dataPtrRead, int nChan, int widthStep, PuzzlePiece piece, double angle)
        {
            byte red, green, blue;
            int x0, y0;

            // center position of the image
            double W = piece.Top.x + (piece.Width / 2f);
            double H = piece.Top.y + (piece.Height / 2f);

            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            for (int y = piece.Top.y; y <= piece.Bottom.y; y++)
            {
                for (int x = piece.Top.x; x <= piece.Bottom.x; x++)
                {
                    x0 = (int)Math.Round((x - W) * cos - (H - y) * sin + W);
                    y0 = (int)Math.Round(H - (x - W) * sin - (H - y) * cos);

                    if ((x0 >= piece.Top.x && x0 < piece.Bottom.x && y0 >= piece.Top.y && y0 < piece.Bottom.y))
                    {
                        red = (dataPtrRead + nChan * x0 + widthStep * y0)[2];
                        green = (dataPtrRead + nChan * x0 + widthStep * y0)[1];
                        blue = (dataPtrRead + nChan * x0 + widthStep * y0)[0];
                    }
                    else
                    {
                        red = dataPtrRead[2];
                        green = dataPtrRead[1];
                        blue = dataPtrRead[0];
                    }

                    (dataPtrWrite + nChan * x + widthStep * y)[2] = red;
                    (dataPtrWrite + nChan * x + widthStep * y)[1] = green;
                    (dataPtrWrite + nChan * x + widthStep * y)[0] = blue;
                }
            }
        }

        public static void Translation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, PuzzlePiece piece, int dx, int dy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrRead = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrWrite = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;
                byte red, green, blue;
                int x0, y0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        x0 = x - dx;
                        y0 = y - dy;

                        if (x0 >= piece.Top.x && x0 < piece.Bottom.x && y0 >= piece.Top.y && y0 < piece.Bottom.y)
                        {
                            red = (dataPtrRead + nChan * x0 + widthStep * y0)[2];
                            green = (dataPtrRead + nChan * x0 + widthStep * y0)[1];
                            blue = (dataPtrRead + nChan * x0 + widthStep * y0)[0];
                        }
                        else
                        {
                            red = (dataPtrRead + nChan * x + widthStep * y)[2];
                            green = (dataPtrRead + nChan * x + widthStep * y)[1];
                            blue = (dataPtrRead + nChan * x + widthStep * y)[0];
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
        public unsafe static PuzzlePiece[] DetectIndependentObjects(byte* dataPtrWrite, byte* dataPtrRead, int nChan, int widthStep, int width, int height)
        {
            // Stores the current tag as key and the equity tag
            int[] linked = new int[width * height];

            // Temporarily stores the neighbors of the current pixel - Not sure about the performance cost tho.
            int[] neighbors = new int[2];

            // Contains the labels for each pixel
            int[,] labels = new int[width, height];

            // The next label to be given
            int nextLabel = 1;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Checks if the pixel is not exactly like the background
                    if (!IsPixelBackGroundColor(x, y))
                    {
                        int nNeigh = 0;

                        // Checks the left pixel
                        if (x - 1 >= 0 && x - 1 < width && labels[x - 1, y] != 0)
                        {
                            neighbors[nNeigh] = labels[x - 1, y];
                            nNeigh++;
                        }
                        // Checks the right pixel
                        if (y - 1 >= 0 && y - 1 < height && labels[x, y - 1] != 0)
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

            // Travels the image and for each pixel loops through the tags
            // until it has the lowest tag possible
            for (int y = 0; y < height; y++)
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
            int[][] imgs2 = images.Values.ToArray();

            for (int i = 0; i < imgs2.Length; i++)
            {
                Vector2Int top = new Vector2Int(imgs2[i][0], imgs2[i][1]);
                Vector2Int bot = new Vector2Int(imgs2[i][2], imgs2[i][3]);
                imgs[i] = new PuzzlePiece(top, bot);
            }
            // Checks if the pixel being checked is equal to the background
            bool IsPixelBackGroundColor(int x, int y)
            {
                return (
                    (dataPtrRead + nChan * x + widthStep * y)[0] == dataPtrRead[0] &&
                    (dataPtrRead + nChan * x + widthStep * y)[1] == dataPtrRead[1] &&
                    (dataPtrRead + nChan * x + widthStep * y)[2] == dataPtrRead[2]);
            }
            return imgs;
        }

        /// <summary>
        /// Draws a red bounding box around the image
        /// </summary>
        /// <param name="dataPtrWrite"> Image to be modified </param>
        /// <param name="nChan"> Number of channels (3, RGB) </param>
        /// <param name="widthStep"> steps point should move on x </param>
        /// <param name="images"> array of the images found </param>
        private unsafe static void DrawBoundingBoxes(byte* dataPtrWrite, int nChan, int widthStep, PuzzlePiece[] images)
        {
            for (int n = 0; n < images.Length; n++)
            {
                // Draw a bounding box around each unique image
                for (int x = images[n].Top.x; x < images[n].Bottom.x; x++)
                {
                    (dataPtrWrite + nChan * x + widthStep * images[n].Top.y)[0] = 0;
                    (dataPtrWrite + nChan * x + widthStep * images[n].Top.y)[1] = 0;
                    (dataPtrWrite + nChan * x + widthStep * images[n].Top.y)[2] = 255;

                    (dataPtrWrite + nChan * x + widthStep * images[n].Bottom.y)[0] = 0;
                    (dataPtrWrite + nChan * x + widthStep * images[n].Bottom.y)[1] = 0;
                    (dataPtrWrite + nChan * x + widthStep * images[n].Bottom.y)[2] = 255;
                }
                for (int y = images[n].Top.y; y <= images[n].Bottom.y; y++)
                {
                    (dataPtrWrite + nChan * images[n].Top.x + widthStep * y)[0] = 0;
                    (dataPtrWrite + nChan * images[n].Top.x + widthStep * y)[1] = 0;
                    (dataPtrWrite + nChan * images[n].Top.x + widthStep * y)[2] = 255;

                    (dataPtrWrite + nChan * images[n].Bottom.x + widthStep * y)[0] = 0;
                    (dataPtrWrite + nChan * images[n].Bottom.x + widthStep * y)[1] = 0;
                    (dataPtrWrite + nChan * images[n].Bottom.x + widthStep * y)[2] = 255;
                }
            }
        }

        /// <summary>
        /// Find the bottom/left most point and the bottom/right most point
        /// then finds the rotation between the two and rotates the image
        /// on the original image
        ///        ___
        ///     ___   |
        ///  ___       |
        /// |         __BR
        ///  |    ___
        ///   BL__
        ///
        /// Then updates the bounding box after the rotation by "raycasting"
        /// from the top center and left center to the center of the image
        ///   _____________
        ///  |  ____|____  |
        ///  |_|         | |
        ///  | |_________| |
        ///  |_____________|
        /// </summary>
        /// <param name="dataPtrWrite"> Image to be modified </param>
        /// <param name="dataPtrRead"> Image to get data from </param>
        /// <param name="nChan"> Number of channels (3, RGB) </param>
        /// <param name="widthStep"> steps point should move on x </param>
        /// <param name="images"> array of the images found </param>
        private static unsafe void FindRotation(byte* dataPtrWrite, byte* dataPtrRead, int nChan, int widthStep, PuzzlePiece[] images)
        {
            for (int i = 0; i < images.Length; i++)
            {
                // Checks if the first pixel of the image is the background
                // color, if it is, assumes it is rotated
                if (IsPixelBackGroundColor(images[i].Top.x, images[i].Top.y))
                {
                    Vector2Int bottomLeft = new Vector2Int();
                    Vector2Int bottomRight = new Vector2Int();

                    // Note: This only works for images rotated counter clockwise
                    // Checks the bottom/left most pixel and the bottom/right
                    // pixels of the image
                    for (int x = images[i].Top.x; x <= images[i].Bottom.x; x++)
                    {
                        // If the pixel is not background means we found the
                        // lowest and most left pixel
                        if (!IsPixelBackGroundColor(x, images[i].Bottom.y))
                        {
                            bottomLeft = new Vector2Int(x, images[i].Bottom.y);
                            break;
                        }
                    }

                    for (int y = images[i].Top.y; y <= images[i].Bottom.y; y++)
                    {
                        // If the pixel is not background means we found one
                        // of the lowest most right pixel, but uncertain ig
                        // it is actually the lowest
                        if (!IsPixelBackGroundColor(images[i].Bottom.x, y))
                        {
                            bottomRight = new Vector2Int(images[i].Bottom.x, y);
                        }
                    }

                    // Finds the angle between the two points found
                    double delta = PuzzlePiece.ImageAngle(bottomRight, bottomLeft);

                    // Rotates a specific image in the original image, converting
                    // degrees to radians
                    Rotation(dataPtrWrite, dataPtrRead, nChan, widthStep, images[i], delta * Math.PI / 180.0);

                    // Updates the bounding box
                    // Note: this sucks. If it's not rotated properly
                    // (jagged edges problem) it will move the bounding box
                    // 1 pixel in more than it should, also not efficent
                    Vector2Int newBoundTop;
                    Vector2Int newBoundBottom;

                    int newXBound = 0;
                    int newYBound = 0;

                    for (int nY = images[i].Top.y; nY < images[i].Bottom.y; nY++)
                    {
                        if (!IsPixelBackGroundColor(images[i].Top.x + (int)(images[i].Width / 2f), nY))
                        {
                            newYBound = nY;
                            break;
                        }
                    }
                    for (int nX = images[i].Top.x; nX < images[i].Bottom.x; nX++)
                    {
                        if (!IsPixelBackGroundColor(nX, images[i].Top.y + (int)(images[i].Height / 2f)))
                        {
                            newXBound = nX;
                            break;
                        }
                    }
                    newBoundTop = new Vector2Int(newXBound, newYBound);

                    newBoundBottom = new Vector2Int(
                        images[i].Bottom.x - Math.Abs(images[i].Top.x - newXBound),
                        images[i].Bottom.y - Math.Abs(images[i].Top.y - newYBound));

                    // Updates that piece with the correct bounding box and angle
                    images[i] = new PuzzlePiece(newBoundTop, newBoundBottom, delta);
                }
            }
            bool IsPixelBackGroundColor(int x, int y)
            {
                return (
                    (dataPtrWrite + nChan * x + widthStep * y)[0] == dataPtrRead[0] &&
                    (dataPtrWrite + nChan * x + widthStep * y)[1] == dataPtrRead[1] &&
                    (dataPtrWrite + nChan * x + widthStep * y)[2] == dataPtrRead[2]);
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
        public static Image<Bgr, byte> puzzle(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, out List<int[]> Pieces_positions, out List<int> Pieces_angle, int level)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;
                Image<Bgr, byte> rotatedImage;

                byte* dataPtrRead = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrWrite = (byte*)m.imageData.ToPointer(); // It can be deleted after

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;

                PuzzlePiece[] puzzlePieces = DetectIndependentObjects(dataPtrWrite, dataPtrRead, nChan, widthStep, width, height);
                FindRotation(dataPtrWrite, dataPtrRead, nChan, widthStep, puzzlePieces);
                // DrawBoundingBoxes(dataPtrWrite, nChan, widthStep, puzzlePieces);

                // Update our read pointer with a copy of the rotated pieces image
                rotatedImage = img.Clone();
                dataPtrRead = (byte*)rotatedImage.MIplImage.imageData.ToPointer();



                // Creates the lists
                Pieces_positions = new List<int[]>();
                Pieces_angle = new List<int>();

                // Adds to the lists the angle and the position
                for (int i = 0; i < puzzlePieces.Length; i++)
                {
                    Pieces_angle.Add((int)Math.Round(puzzlePieces[i].Angle));
                    Pieces_positions.Add(new int[] { puzzlePieces[i].Top.x, puzzlePieces[i].Top.y, puzzlePieces[i].Bottom.x, puzzlePieces[i].Bottom.y });

                    puzzlePieces[i].CreateImage(rotatedImage);
                }


                List<PuzzlePiece> pieces = new List<PuzzlePiece>(puzzlePieces);
                Image<Bgr, byte> finalImage = RecursiveJoin(pieces).Img;
                return finalImage;
            }
        }

        public static PuzzlePiece RecursiveJoin(List<PuzzlePiece> pieces)
        {
            // while
            for (int i = 0; i < pieces.Count; i++)
            {
                Side connectionSide = Side.Top;
                double min = float.PositiveInfinity;
                int minIndex = -1;

                for (int side = 0; side < 4; side++)
                {
                    for (int j = 0; j < pieces.Count; j++)
                    {
                        // Protect from comparing with the same puzzle piece
                        if (i == j) continue;

                        double dist = 0;
                        // dist = puzzlePieces[i].CompareSide(puzzlePieces[j], (Side)side);

                        dist = PuzzlePiece.Compare(pieces[i], pieces[j], (Side)side);

                        if (dist < min)
                        {
                            min = dist;
                            minIndex = j;
                            connectionSide = (Side)side;
                        }
                    }
                }

                if (minIndex > -1) 
                {
                    // Combine
                    PuzzlePiece combined = pieces[i].Combine(pieces[minIndex], connectionSide);

                    pieces[i] = combined;
                    pieces.Remove(pieces[minIndex]);
                    i = 0;
                }
            }

            return pieces[0];
        }
    }
}