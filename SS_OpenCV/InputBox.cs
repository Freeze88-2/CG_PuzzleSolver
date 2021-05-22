using System.Windows.Forms;

namespace CG_OpenCV
{
    public partial class InputBox : Form
    {
        public InputBox()
        {
            InitializeComponent();
        }

        public InputBox(string _title)
        {
            InitializeComponent();

            Text = _title;

        }

    }
}