using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface IIcbcspecialuseService
    {
        void Icbcspecialuse2JS(JObject jo);
        void IcbcspecialusePing2JS(JObject jo);

        void Icbcspecialuse2CallMachine(JObject jo);
        void IcbcspecialusePing2CallMachine(JObject jo);
        void IcbcspecialusePing2CallMachineAsync(JObject jo);
        void Icbcspecialuse2CallMachineAsync(JObject jo);

    
    }
}
