using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlacer
{
    class TreeNodeInherited : System.Windows.Forms.TreeNode
    {
        public TreeNodeInherited(String name) : base(name) { }

        public Boolean isPopulated { get; set; }
        public Boolean isDirectory { get; set; }
        public Boolean isImage { get; set; }
    }
}
