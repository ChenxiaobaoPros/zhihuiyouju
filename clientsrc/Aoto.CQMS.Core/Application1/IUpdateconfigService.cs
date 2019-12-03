using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface IUpdateconfigService
    {
        void Updateconfig2JS(JObject jo);

        void Updateconfig2CallMachineBySelect(JObject jo);

        void Updateconfig2CallMachineByUpdate(JObject jo);
        void Updateconfig2CallMachineBySelectAsync(JObject jo);

        void Updateconfig2CallMachineByUpdateAsync(JObject jo);
    }
}
