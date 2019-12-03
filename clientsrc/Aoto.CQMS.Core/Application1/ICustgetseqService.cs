using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface ICustgetseqService
    {
        void Custgetseq2JS(JObject jo);

        void Kazhejudge2JS(JObject jo);

        void Custgetseq2CallMachine(JObject jo);

        void Kazhejudge2CallMachine(JObject jo);

        void Custgetseq2CallMachineAsync(JObject jo);

        void Kazhejudge2CallMachineAsync(JObject jo);

    }
}
