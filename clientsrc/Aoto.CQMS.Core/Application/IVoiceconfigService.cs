using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface IVoiceconfigService
    {
        void Voiceconfig2JS(JObject jo);

        void Voiceconfig2CallMachine(JObject jo);

        void Voiceconfig2CallMachineAsync(JObject jo);
    }
}
