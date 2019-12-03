using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface IQueuequeryService
    {
        void Queuequery2JS(JObject jo);

        void Queuequery2CallMachine(JObject jo);
        void Queuequery2CallMachineAsync(JObject jo);

        void QueueAttributes2JS(JObject jo);

        void QueueAttributes2CallMachine(JObject jo);
        void QueueAttributes2CallMachineAsync(JObject jo);

    }
}
