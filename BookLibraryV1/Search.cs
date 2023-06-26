using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookLibraryV1
{
    internal class Search
    {
        public Search()
        {

        }
        public int searchTree(TreeNodeCollection tree, String s)
        {
            int l = 0; 
            int r = tree.Count -1;
            while (l <= r)
            {
                int m = l + (r - 1) / 2;
                int res = s.CompareTo(tree[m].Text);

                if (res == 0)
                {
                    return m;
                }
                if (res > 0)
                {
                    l = m+1;
                }
                else
                {
                    r = m - 1;
                }               
            }
            return -1;
        }
    }
}
