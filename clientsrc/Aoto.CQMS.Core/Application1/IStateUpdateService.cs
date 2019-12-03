using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface IStateUpdateService
    {
        void StateUpdate2JS(JObject jo);

        void StateUpdate2CallMachine(JObject jo);

        void StateUpdate2CallMachineAsync(JObject jo);
    }
}
