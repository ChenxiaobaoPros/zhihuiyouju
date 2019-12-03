using System;
using Newtonsoft.Json.Linq;

namespace Aoto.PPS.Infrastructure.ComponentModel
{
    public delegate void RunAsyncCaller(JObject jo);
    public delegate void RunCompletedEventHandler(object sender, RunCompletedEventArgs e);

    public class RunCompletedEventArgs : EventArgs
    {
        private object result;

        public RunCompletedEventArgs(object result)
        {
            this.result = result;
        }

        public object Result { get { return result; } }
    }
    /// <summary>
    /// 异步
    /// </summary>
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
    /// <summary>
    /// 错误码
    /// </summary>
    public static class ErrorCode
    {
        public const int Success = 0;
        public const int Failure = 1;
        public const int Busy = 2;
        public const int Cancelled = 3;
        public const int Timeout = 4;
        public const int Offline = 5;
    }
    /// <summary>
    /// 状态码
    /// </summary>
    public static class StatusCode
    {
        public const int Disabled = 0;
        public const int Normal = 1;
        public const int Busy = 2;
        public const int Offline = 3;
        public const int NotSupport = 4;
        public const int Mock = 9;
    }
}
