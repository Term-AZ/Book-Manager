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
    public partial class SelectAuthor : Form
    {
        List<String> authors = new List<String>();
        public String returnValue="";
        String authorName;
        public SelectAuthor(List<String> s, String name)
        {
            authors = s;
            authorName = name;
            InitializeComponent();

            foreach(String i in authors)
            {
                AuthorList.Items.Add(new ListViewItem(new String[] {i,name}));
            }
        }

        private void SelectAuthor_Load(object sender, EventArgs e)
        {

        }

        private void AuthorList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void selectAuthorBtn_Click(object sender, EventArgs e)
        {
            returnValue = AuthorList.SelectedItems[0].SubItems[0].Text;
            this.Close();
        }
    }
}
