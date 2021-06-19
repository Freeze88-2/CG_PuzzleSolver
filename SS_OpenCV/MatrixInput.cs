using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CG_OpenCV
{
    public partial class MatrixInput : Form
    {
        public TextBox[,] InputMatrix { get; }
        public TextBox WeightInput { get; }

        public MatrixInput()
        {
            InitializeComponent();
        }

        public MatrixInput(string title) 
        {
            InitializeComponent();

            Text = title;

            InputMatrix = new TextBox[3, 3]
            {
                {matrix1, matrix2, matrix3},
                {matrix4, matrix5, matrix6},
                {matrix7, matrix8, matrix9}
            };

            WeightInput = weightInput;
            WeightInput.Text = "1";
        }

        private void MatrixInput_Load(object sender, EventArgs e)
        {
            
        }
    }
}
