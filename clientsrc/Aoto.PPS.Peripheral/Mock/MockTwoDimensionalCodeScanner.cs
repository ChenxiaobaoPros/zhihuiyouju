using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;

using Newtonsoft.Json.Linq;

using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.Configuration;

namespace Aoto.PPS.Peripheral.Mock
{
    public class MockTwoDimensionalCodeScanner : IReader
    {
        private RunAsyncCaller readAsyncCaller;

        private string dll;
        private int timeout;
        private bool enabled;
        private bool cancelled;
        private bool isBusy;

        public bool Cancelled { get { return enabled; } set { enabled = value; } }
        public bool Enabled { get { return enabled; } }
        public bool IsBusy { get { return isBusy; } }
        public event RunCompletedEventHandler RunCompletedEvent;

        public MockTwoDimensionalCodeScanner()
        {
            readAsyncCaller = new RunAsyncCaller(Read);
        }

        public void Initialize()
        {
            
        }

        public void ReadAsync(JObject jo)
        {
            readAsyncCaller.BeginInvoke(jo, new AsyncCallback(Callback), jo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 0：禁用；1：正常；2：忙；3：离线；
        /// </returns>
        public int GetStatus()
        {
            if (enabled)
            {
                return StatusCode.Mock;
            }
            else
            {
                return StatusCode.Disabled;
            }
        }

        public void Cancel()
        {
            cancelled = true;
        }

        public void Dispose()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jo"></param>
        /// <returns>
        /// jc["result"] 0：成功；4：取消；5：超时；其他失败
        /// </returns>
        public void Read(JObject jo)
        {
            jo["info"] = "this is test data";
            jo["result"] = ErrorCode.Success;
            Thread.Sleep(1000);
        }

        private void Callback(IAsyncResult ar)
        {
            isBusy = false;
            JObject jo = (JObject)ar.AsyncState;
            ((RunAsyncCaller)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);
            RunCompletedEvent(this, new RunCompletedEventArgs(jo));
        }
    }
}
