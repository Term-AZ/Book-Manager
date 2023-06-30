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
    public partial class ResetConfirmer : Form
    {
        public bool returnValue = false;
        public ResetConfirmer()
        {
            InitializeComponent();
        }

        private void YesBtn_Click(object sender, EventArgs e)
        {
            returnValue = true;
            this.Close();
        }

        private void NoBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
