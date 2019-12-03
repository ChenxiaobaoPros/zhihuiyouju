using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface IQueueAttributesUpdateService
    {
        void Counterconfig2JS(JObject jo);

        void Counterconfig2CallMachineBySelect(JObject jo);

        void Counterconfig2CallMachineByUpdate(JObject jo);
    }
}
