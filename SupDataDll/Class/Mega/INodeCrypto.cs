using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupDataDll.Class.Mega
{
    public interface INodeCrypto
    {
        byte[] Key { get; }

        byte[] SharedKey { get; }

        byte[] Iv { get; }

        byte[] MetaMac { get; }

        byte[] FullKey { get; }
    }
}
