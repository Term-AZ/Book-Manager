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
            int m = -1;
            int l = 0; 
            int u = tree.Count -1;
            while (l <= u)
            {
                m = (l + u)/2;
                int res = s.CompareTo(tree[m].Text);

                if (res == 0)
                {
                    return -1;
                }
                if (res > 0)
                {
                    l = m + 1;
                }
                else
                {
                    u = m - 1;
                }               
            }
            return ++m;
        }
    }
}
