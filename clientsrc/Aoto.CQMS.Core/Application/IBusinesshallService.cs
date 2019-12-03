using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface IBusinesshallService
    {
        void Businesshall2JS(JObject jo);

        void Businesshall2CallMachine(JObject jo);

        void Businesshall2CallMachineAsync(JObject jo);

        

    }
}
