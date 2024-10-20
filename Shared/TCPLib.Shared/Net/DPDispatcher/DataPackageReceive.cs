using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPLib.Classes;

namespace TCPLib.Net.DPDispatcher
{
    public delegate Task DataPackageReceive(DataPackageSource package);
}
