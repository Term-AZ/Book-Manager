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
    public partial class ConfirmDelete : Form
    {
        public int returnValue;

        public ConfirmDelete(int i)
        {
            InitializeComponent();

            if (i == 0)
            {
                label1.Text = "Do you wish to delete everyrthing under this author as well?";
            }else if (i == 1)
            {
                label1.Text = "Do you wish to delete everything under this series as well?";
            }
            else
            {
                button2.Enabled = false;
                button2.Hide();
                button1.Location = new Point(112, 172);
                button1.Text = "Delete";
                button3.Location = new Point(258, 172);
                label1.Text = "Are you sure you want to delete this/these book(s)";

            }
        }



        private void ConfirmDelete_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            returnValue = 1;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            returnValue = 0;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            returnValue = -1;
            this.Close();
        }
    }
}
