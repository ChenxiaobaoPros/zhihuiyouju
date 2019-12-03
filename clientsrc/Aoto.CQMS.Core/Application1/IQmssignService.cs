using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface IQmssignService
    {
        void GetQmssign2JS(JObject jo);

        void Qmssign2CallMachine(JObject jo);

        void Qmssign2CallMachineAsync(JObject jo);
    }
}
