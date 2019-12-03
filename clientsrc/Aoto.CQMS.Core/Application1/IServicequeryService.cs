using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface IServicequeryService
    {
        void Servicequery2JS(JObject jo);

        void Servicequery2CallMachine(JObject jo);
        void Servicequery2CallMachineAsync(JObject jo);

    }
}
