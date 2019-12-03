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

        void Businesshall2CallMachineBySelect(JObject jo);

        void Businesshall2CallMachineByAdd(JObject jo);

        void Businesshall2CallMachineByDelete(JObject jo);

        void Businesshall2CallMachineByUpdate(JObject jo);
        void Businesshall2CallMachineBySelectAsync(JObject jo);

        void Businesshall2CallMachineByAddAsync(JObject jo);

        void Businesshall2CallMachineByDeleteAsync(JObject jo);

        void Businesshall2CallMachineByUpdateAsync(JObject jo);

    }
}
