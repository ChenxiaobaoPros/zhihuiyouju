using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
  public   interface ILogService
    {
        void SelectLog2JS(JObject jo);

        void SelectLog2CallMachine(JObject jo);

        void SelectLog2CallMachineAsync(JObject jo);
    }
}
