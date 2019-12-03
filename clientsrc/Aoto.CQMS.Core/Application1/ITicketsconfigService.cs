using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface ITicketsconfigService
    {
        void Ticketsconfig2JS(JObject jo);

        void Ticketsconfig2CallMachineBySelect(JObject jo);

        void Ticketsconfig2CallMachineByUpdate(JObject jo);
        void Ticketsconfig2CallMachineBySelectAsync(JObject jo);

        void Ticketsconfig2CallMachineByUpdateAsync(JObject jo);
    }
}
