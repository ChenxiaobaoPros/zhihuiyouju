using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoto.PPS.Infrastructure.Configuration
{
    /// <summary>
    /// 缓存信息
    /// </summary>
    public class AppCache
    {
        public static readonly IDictionary<string, string> dicPageCache = new Dictionary<string, string>();
    }
}
