using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudManagerGeneralLib.Class
{
    public class CommandNodes
    {
        List<ExplorerNode> from_ = new List<ExplorerNode>();
        public List<ExplorerNode> From { get { return from_; } set { from_ = value; } }
        public ExplorerNode To { get; set; }
        public Commands Command { get; set; }
    }

    public enum Commands
    {
        List,//Get list chid node in parent
        FileInfo,
        Cut,Copy,
        Delete, PermanentDelete,
        /// <summary>
        /// Cloud to Cloud, add a file shared from B to A cloud (not create new file(id))
        /// </summary>
        Import,
    }
}
