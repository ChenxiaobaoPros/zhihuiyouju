using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoto.EMS.Infrastructure.ComponentModel
{
    public interface IScriptInvoker
    {
        /// <summary>
        /// H5调用
        /// </summary>
        /// <param name="jo"></param>
        void ScriptInvoke(JObject jo);
        /// <summary>
        /// 外设调用
        /// </summary>
        /// <param name="jo"></param>
        void PeripheralInvoke(JObject jo);
        /// <summary>
        /// 导航
        /// </summary>
        /// <param name="url"></param>
        void Navigate(string url);
        /// <summary>
        /// 关闭
        /// </summary>
        void Shut();
        /// <summary>
        /// 重置打印
        /// </summary>
        void ResetPrint();
        /// <summary>
        /// 读取外设数据
        /// </summary>
        /// <param name="jo"></param>
        void ReadRawDataInvoker(JObject jo);
    }
}
