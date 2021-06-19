using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CG_OpenCV
{
    public partial class MainForm : Form
    {
        private Image<Bgr, byte> img = null; // working image
        private Image<Bgr, byte> imgUndo = null; // undo backup image - UNDO
        private readonly string title_bak = "";

        public MainForm()
        {
            InitializeComponent();
            title_bak = Text;
        }

        /// <summary>
        /// Opens a new image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                img = new Image<Bgr, byte>(openFileDialog1.FileName);
                Text = title_bak + " [" +
                        openFileDialog1.FileName.Substring(openFileDialog1.FileName.LastIndexOf("\\") + 1) +
                        "]";
                imgUndo = img.Copy();
                ImageViewer.Image = img.Bitmap;
                ImageViewer.Refresh();
            }
        }

        /// <summary>
        /// Saves an image with a new name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImageViewer.Image.Save(saveFileDialog1.FileName);
            }
        }

        /// <summary>
        /// Closes the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// restore last undo copy of the working image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (imgUndo == null) // verify if the image is already opened
            {
                return;
            }

            Cursor = Cursors.WaitCursor;
            img = imgUndo.Copy();

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor
        }

        /// <summary>
        /// Change visualization mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // zoom
            if (autoZoomToolStripMenuItem.Checked)
            {
                ImageViewer.SizeMode = PictureBoxSizeMode.Zoom;
                ImageViewer.Dock = DockStyle.Fill;
            }
            else // with scroll bars
            {
                ImageViewer.Dock = DockStyle.None;
                ImageViewer.SizeMode = PictureBoxSizeMode.AutoSize;
            }
        }

        /// <summary>
        /// Show authors form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AuthorsForm form = new AuthorsForm();
            form.ShowDialog();
        }

        /// <summary>
        /// Calculate the image negative
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void negativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
            {
                return;
            }

            Cursor = Cursors.WaitCursor; // clock cursor

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Negative(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor
        }

        /// <summary>
        /// Call automated image processing check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void evalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EvalForm eval = new EvalForm();
            eval.ShowDialog();
        }

        /// <summary>
        /// Call image convertion to gray scale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
            {
                return;
            }

            Cursor = Cursors.WaitCursor; // clock cursor

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.ConvertToGray(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor
        }

        private void brightnessContrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
            {
                return;
            }

            Cursor = Cursors.WaitCursor; // clock cursor

            //copy Undo Image
            imgUndo = img.Copy();

            InputBox form = new InputBox("brilho?");
            InputBox form2 = new InputBox("contraste?");

            form.ShowDialog();
            form2.ShowDialog();

            int brilho = Convert.ToInt32(form.ValueTextBox.Text);
            double contrast = Convert.ToDouble(form2.ValueTextBox.Text);

            ImageClass.BrightContrast(img, brilho, contrast);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor
        }

        private void rotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
            {
                return;
            }

            Cursor = Cursors.WaitCursor; // clock cursor

            //copy Undo Image
            imgUndo = img.Copy();

            InputBox form = new InputBox("Rotation?");

            form.ShowDialog();

            double rot = double.Parse(form.ValueTextBox.Text);

            ImageClass.Rotation(img, imgUndo, (float)(rot * Math.PI / 180));

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor
        }

        private void translationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
            {
                return;
            }

            Cursor = Cursors.WaitCursor; // clock cursor

            //copy Undo Image
            imgUndo = img.Copy();

            InputBox form = new InputBox("points? Tooltip: x y");

            form.ShowDialog();

            int a = Convert.ToInt32(form.ValueTextBox.Text[0]);
            int b = Convert.ToInt32(form.ValueTextBox.Text[1]);

            ImageClass.Translation(img, img, a, b);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor
        }

        private void binarizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
            {
                return;
            }
            Cursor = Cursors.WaitCursor; // clock cursor

            //copy Undo Image
            imgUndo = img.Copy();

            //implement here things

            Cursor = Cursors.Default;
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
            {
                return;
            }
            Cursor = Cursors.WaitCursor; // clock cursor

            //copy Undo Image
            imgUndo = img.Copy();

            int[][] histogram = ImageClass.Histogram_All(img);
            Histogram his = new Histogram(histogram);

            his.Show();
            Cursor = Cursors.Default;
        }

        private void PuzzleSolverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
            {
                return;
            }
            Cursor = Cursors.WaitCursor; // clock cursor

            //copy Undo Image
            imgUndo = img.Copy();

            Image<Bgr, byte> m = ImageClass.puzzle(img, imgUndo, out List<int[]> positions, out List<int> angles, 0);

            ImageViewer.Image = m.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default;
        }

        private void blackWhiteTresholdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
            {
                return;
            }

            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            InputBox tresholdBox = new InputBox("Binary treshold 0-255");
            tresholdBox.ShowDialog();
            if (string.IsNullOrEmpty(tresholdBox.ValueTextBox.Text))
                return;
            int t = Convert.ToInt32(tresholdBox.ValueTextBox.Text);

            ImageClass.ConvertToBW(img, t);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void blackWhiteOtsuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
            {
                return;
            }

            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            int t = 0;
            // Get the propper treshold from Otsu function
            t = ImageClass.Otsu(img);

            InputBox otsuValue = new InputBox("Calculated treshold");
            otsuValue.ValueTextBox.Text = t.ToString();
            otsuValue.ShowDialog();

            ImageClass.ConvertToBW(img, t);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        /// <summary>
        /// Change visualization mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zoomInImage(object sender, MouseEventArgs e)
        {
            int nSteps = e.Delta * SystemInformation.MouseWheelScrollLines / 60;

            ImageViewer.SizeMode = PictureBoxSizeMode.Zoom;
            ImageViewer.Size = new System.Drawing.Size(ImageViewer.Size.Width + nSteps, ImageViewer.Size.Height + nSteps);
            ImageViewer.Dock = DockStyle.None;
        }

        private void rotateIndividualPiecesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
            {
                return;
            }
            Cursor = Cursors.WaitCursor; // clock cursor

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.RotateIndividualPieces(img, imgUndo);
            ImageClass.DrawBounds(img, img);
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default;
        }

        private void drawBoundsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
            {
                return;
            }
            Cursor = Cursors.WaitCursor; // clock cursor

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.DrawBounds(img, imgUndo);
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default;
        }

        private void robertsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
            {
                return;
            }
            Cursor = Cursors.WaitCursor; // clock cursor

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Roberts(img, imgUndo);
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default;
        }

        private void diferentiationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
            {
                return;
            }
            Cursor = Cursors.WaitCursor; // clock cursor

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Diferentiation(img, imgUndo);
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default;
        }

        private void sobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
            {
                return;
            }
            Cursor = Cursors.WaitCursor; // clock cursor

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Sobel(img, imgUndo);
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default;
        }

        private void meanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
            {
                return;
            }
            Cursor = Cursors.WaitCursor; // clock cursor

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Mean(img, imgUndo);
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default;
        }

        private void medianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
            {
                return;
            }
            Cursor = Cursors.WaitCursor; // clock cursor

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Median(img, imgUndo);
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default;
        }

        private void nonUniformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
            {
                return;
            }

            //copy Undo Image
            imgUndo = img.Copy();
            float[,] matrix = new float[3,3];
            float weight = 1f;

            MatrixInput input = new MatrixInput("Non Uniform Matrix Input");
            input.ShowDialog();

            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    if (string.IsNullOrEmpty(input.InputMatrix[x, y].Text)) 
                    {
                        return;
                    }
                    matrix[x, y] = Convert.ToSingle(input.InputMatrix[x, y].Text);
                }
            }

            if (string.IsNullOrEmpty(input.WeightInput.Text))
            {
                return;
            }
            weight = Convert.ToSingle(input.WeightInput.Text);
            weight = Math.Min(1, Math.Max(weight, 0));

            Cursor = Cursors.WaitCursor; // clock cursor
            ImageClass.NonUniform(img, imgUndo, matrix, weight);
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default;
        }
    }
}