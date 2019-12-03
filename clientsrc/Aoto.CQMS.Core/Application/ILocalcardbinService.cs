using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface ILocalcardbinService
    {
        void Localcardbin2JS(JObject jo);

        void Localcardbin2CallMachine(JObject jo);

        void Localcardbin2CallMachineAsync(JObject jo);

        void Biomqmscardbin2JS(JObject jo);

        void Biomqmscardbin2CallMachine(JObject jo);

        void Biomqmscardbin2CallMachineAsync(JObject jo);
    }
}
