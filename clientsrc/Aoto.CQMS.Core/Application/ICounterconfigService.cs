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

        void CounterconfigAsync(JObject jo);

        void Counterconfig2CallMachine(JObject jo);

        void WindowQueueAdjust2JS(JObject jo);

        void WindowQueueAdjustAsync(JObject jo);

        void WindowQueueAdjust2CallMachine(JObject jo);
    }
}
