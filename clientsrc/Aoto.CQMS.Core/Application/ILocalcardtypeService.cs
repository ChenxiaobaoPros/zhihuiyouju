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

        void Localcardtype2CallMachine(JObject jo);


        void Localcardtype2CallMachineAsync(JObject jo);
    }
}
