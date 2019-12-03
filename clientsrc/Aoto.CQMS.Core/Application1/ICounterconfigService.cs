using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface ICounterconfigService
    {
        void Counterconfig2JS(JObject jo);

        void Counterconfig2CallMachineBySelect(JObject jo);

        void Counterconfig2CallMachineByUpdate(JObject jo);

        void Counterconfig2CallMachineByAdd(JObject jo);

        void Counterconfig2CallMachineByDelete(JObject jo);

        void Counterconfig2CallMachineBySelectAsync(JObject jo);

        void Counterconfig2CallMachineByUpdateAsync(JObject jo);

        void Counterconfig2CallMachineByAddAsync(JObject jo);

        void Counterconfig2CallMachineByDeleteAsync(JObject jo);
        void WindowQueueAdjust2CallMachineAsync(JObject jo);

        void WindowQueueAdjust2CallMachine(JObject jo);
    }
}
