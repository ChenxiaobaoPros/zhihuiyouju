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

        void Localcardbin2CallMachineBySelect(JObject jo);

        void Localcardbin2CallMachineByAdd(JObject jo);

        void Localcardbin2CallMachineByUpdate(JObject jo);

        void Localcardbin2CallMachineByDelete(JObject jo);
    }
}
