using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface ILocalcardtypeService
    {
        void Localcardtype2JS(JObject jo);

        void Localcardtype2CallMachineBySelect(JObject jo);

        void Localcardtype2CallMachineByAdd(JObject jo);

        void Localcardtype2CallMachineByUpdate(JObject jo);

        void Localcardtype2CallMachineByDelete(JObject jo);
    }
}
