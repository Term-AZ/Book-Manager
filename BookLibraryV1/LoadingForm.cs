using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookLibraryV1
{
    public partial class LoadingForm : Form
    {
        public Form1 form;

        public LoadingForm(int max)
        {
            InitializeComponent();
            progressBar1.Minimum = 0;
            progressBar1.Maximum=max;

        }

        private void Form2_Load(object sender, EventArgs e)
        {
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

    }          
        
}
