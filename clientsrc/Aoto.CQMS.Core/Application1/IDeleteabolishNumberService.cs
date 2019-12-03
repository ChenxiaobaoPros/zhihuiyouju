using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface IDeleteabolishNumberService
    {
        void DeleteabolishNumber2JS(JObject jo);

        void DeleteabolishNumber2CallMachine(JObject jo);
        void DeleteabolishNumber2CallMachineAsync(JObject jo);

    }
}
