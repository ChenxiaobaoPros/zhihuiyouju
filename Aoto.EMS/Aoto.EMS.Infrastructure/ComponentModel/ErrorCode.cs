using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoto.EMS.Infrastructure.ComponentModel
{
    public static class ErrorCode
    {
        public const int Success = 0;
        public const int Failure = 1;
        public const int Busy = 2;
        public const int Cancelled = 3;
        public const int Timeout = 4;
        public const int Offline = 5;
    }
}
