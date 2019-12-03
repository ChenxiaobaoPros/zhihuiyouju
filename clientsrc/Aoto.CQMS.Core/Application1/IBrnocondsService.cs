using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface IBrnocondsService
    {
        void Brnoconds2JS(JObject jo);

        void Brnoconds2CallMachine(JObject jo);

        void Brnoconds2CallMachineAsync(JObject jo);
    }
}
