using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupDataDll
{
    public class UnknowCloudNameException : Exception
    {
        public UnknowCloudNameException(string Message) : base(Message)
        {
        }
    }

    public class CancelExplorerException : Exception
    {
        public CancelExplorerException() : base()
        {
        }
    }
}
