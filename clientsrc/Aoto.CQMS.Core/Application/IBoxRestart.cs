using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    interface IBoxRestartService
    {
        void BoxRestart2JS(JObject jo);

        void BoxRestart2CallMachine(JObject jo);

        void BoxRestart2CallMachineAsync(JObject jo);
    }
}
