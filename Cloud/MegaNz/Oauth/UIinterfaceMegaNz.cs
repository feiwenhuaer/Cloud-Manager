using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloud.MegaNz.Oauth
{
    public interface UIinterfaceMegaNz
    {
        void ShowDialog_();
        string Email { get;}
        string Pass { get; }
        bool Success { get; }
        void ShowError(string message);
    }
}
