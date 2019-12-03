using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface IReservationService
    {
        void Reservation2JS(JObject jo);

        void Reservation2CallMachine(JObject jo);
    }
}
