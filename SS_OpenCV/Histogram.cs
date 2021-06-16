using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ZedGraph;

namespace CG_OpenCV
{
    public partial class Histogram : Form
    {
        public Histogram(int[][] histogram)
        {
            InitializeComponent();

            DataPointCollection list1 = chart1.Series[0].Points;
            DataPointCollection list2 = chart1.Series[1].Points;
            DataPointCollection list3 = chart1.Series[2].Points;
            DataPointCollection list4 = chart1.Series[3].Points;

            for (int i = 0; i < histogram[0].Length; i++)
            {
                list1.AddXY(i, histogram[0][i]);
                list2.AddXY(i, histogram[1][i]);
                list3.AddXY(i, histogram[2][i]);
                list4.AddXY(i, histogram[3][i]);
            }

            chart1.Series[0].Name = "Greys";
            chart1.Series[0].Color = Color.Gray;

            chart1.Series[1].Name = "Red";
            chart1.Series[1].Color = Color.Red;

            chart1.Series[2].Name = "Green";
            chart1.Series[2].Color = Color.Green;

            chart1.Series[3].Name = "Blue";
            chart1.Series[3].Color = Color.Blue;

            chart1.ChartAreas[0].AxisX.Maximum = 255;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Title = "Intensity";
            chart1.ChartAreas[0].AxisY.Title = "Pixels Count";


            chart1.ResumeLayout();
        }

        /// <summary
    }
}
