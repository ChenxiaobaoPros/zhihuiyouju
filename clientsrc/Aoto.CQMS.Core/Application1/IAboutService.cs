using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface IAboutService
    {
        void About2JS(JObject jo);

        void About2CallMachine(JObject jo);
        void About2CallMachineAsync(JObject jo);

    }
}
