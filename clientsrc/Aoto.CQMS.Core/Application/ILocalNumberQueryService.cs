using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface ILocalNumberQueryService
    {
        void LocalNumberQuery2JS(JObject jo);

        void LocalNumberQuery2CallMachine(JObject jo);
        void LocalNumberQuery2CallMachineAsync(JObject jo);
    }
}
