using System;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure.Configuration;
using Newtonsoft.Json.Linq;

namespace Aoto.PPS.Peripheral.Mock
{
    public class MockIDCardReader : IReader
    {
        private string dll;
        private int timeout;
        private bool enabled;
        private bool isBusy;
        private RunAsyncCaller readAsyncCaller;

        public bool Cancelled { get { return enabled; } set { enabled = value; } }
        public bool Enabled { get { return enabled; } }
        public bool IsBusy { get { return isBusy; } }
        public event RunCompletedEventHandler RunCompletedEvent;

        public MockIDCardReader()
        {
            this.enabled = Config.App.Peripheral["idCardReader"].Value<bool>("enabled");
            readAsyncCaller = new RunAsyncCaller(Read);
        }

        public void Initialize()
        {

        }

        public void ReadAsync(JObject jo)
        {
            readAsyncCaller.BeginInvoke(jo, new AsyncCallback(Callback), jo);
        }

        public void Read(JObject jo)
        {
            jo["certName"] = "王五";
            jo["gender"] = "男";
            jo["nationality"] = "汉族";
            jo["birthday"] = "19850512";
            jo["address"] = "安徽省来安县新安镇古楼东街８号２幢６６室";
            jo["certNo"] = "342623198511291625";
            jo["issuedBy"] = "来安县公安局";
            jo["expDate"] = "20081006-20181006";
            jo["certType"] = 1;
            jo["result"] = ErrorCode.Success;

            Thread.Sleep(1000);
        }

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