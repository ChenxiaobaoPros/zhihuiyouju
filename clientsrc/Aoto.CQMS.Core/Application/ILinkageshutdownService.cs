using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface ILinkageshutdownService
    {
        void Linkageshutdown2JS(JObject jo);

        void Linkageshutdown2CallMachineBy(JObject jo);

        void Linkageshutdown2CallMachineByAsync(JObject jo);

    }
}
