using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface IBasicconfigService
    {
        void GetBasicconfig2JS(JObject jo);

        void Basicconfig2CallMachine(JObject jo);

        void Basicconfig2CallMachineAsync(JObject jo);

        void GetQcmConfig2JS(JObject jo);

        void QcmConfig2CallMachine(JObject jo);

        void QcmConfig2CallMachineAsync(JObject jo);

      

    }
}
