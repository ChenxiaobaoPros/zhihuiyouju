using System;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure.Configuration;
using Newtonsoft.Json.Linq;

namespace Aoto.PPS.Peripheral.Mock
{
    public class MockNeedlePrinter : IPrinter
    {
        private string name;
        private string dll;
        private int timeout;
        private bool enabled;
        private bool cancelled;
        private bool isBusy;
        private RunAsyncCaller printAsyncCaller;

        public bool Cancelled { get { return enabled; } set { enabled = value; } }
        public bool Enabled { get { return enabled; } }
        public bool IsBusy { get { return isBusy; } }
        public event RunCompletedEventHandler RunCompletedEvent;

        public MockNeedlePrinter()
        {
            this.enabled = Config.App.Peripheral["needlePrinter"].Value<bool>("enabled");
            printAsyncCaller = new RunAsyncCaller(Print);
        }

        public void Initialize()
        {

        }

        public void Execute(int command)
        {

        }

        public void Print(JObject jo)
        {
            Thread.Sleep(2000);
            jo["result"] = ErrorCode.Success;
        }

        public void PrintAsync(JObject jo)
        {
            isBusy = true;
            cancelled = false;
            printAsyncCaller.BeginInvoke(jo, new AsyncCallback(Callback), jo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 0：禁用；1：正常；2：忙；3：离线；
        /// 10：空闲未装载纸；11：空闲已装载纸，准备就绪
        /// 99: 模拟
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

        private void Callback(IAsyncResult ar)
        {
            isBusy = false;
            JObject jo = (JObject)ar.AsyncState;
            ((RunAsyncCaller)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);
            RunCompletedEvent(this, new RunCompletedEventArgs(jo));
        }
    }
}