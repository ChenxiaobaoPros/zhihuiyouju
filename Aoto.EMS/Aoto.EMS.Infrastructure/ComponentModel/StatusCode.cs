using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoto.EMS.Infrastructure.ComponentModel
{
    public static class StatusCode
    {
        public const int Disabled = 0;
        public const int Normal = 1;
        public const int Busy = 2;
        public const int Offline = 3;
        public const int NotSupport = 4;
        public const int Mock = 9;
    }
}
