using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface IQcmConfigService
    {
        void GetQcmConfig2JS(JObject jo);

        void QcmConfig2CallMachine(JObject jo);

        void QcmConfig2CallMachineAsync(JObject jo);
    }
}
