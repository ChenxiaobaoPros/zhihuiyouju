using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface IIcbcspecialuseService
    {
        void Icbcspecialuse2JS(JObject jo);

        void Icbcspecialuse2CallMachineBySelect(JObject jo);

        void Icbcspecialuse2CallMachineByUpdate(JObject jo);

        void Icbcspecialuse2CallMachineBySelectAsync(JObject jo);

        void Icbcspecialuse2CallMachineByUpdateAsync(JObject jo);
    }
}
