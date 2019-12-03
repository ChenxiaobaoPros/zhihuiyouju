using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface IBiomqmscardbinService
    {
        void Biomqmscardbin2JS(JObject jo);

        void Biomqmscardbin2CallMachine(JObject jo);
    }
}
