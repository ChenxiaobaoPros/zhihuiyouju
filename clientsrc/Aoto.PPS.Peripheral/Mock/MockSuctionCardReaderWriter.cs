using System;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Aoto.PPS.Infrastructure.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Aoto.PPS.Peripheral.Mock
{
    public class MockSuctionCardReaderWriter : IReader, IWriter
    {
        private RunAsyncCaller readAsyncCaller;
        private RunAsyncCaller writeAsyncCaller;

        private string dll;
        private int timeout;
        private bool enabled;
        private bool isBusy;
        private bool cancelled;

        public bool Cancelled { get { return enabled; } set { enabled = value; } }
        public bool Enabled { get { return enabled; } }
        public bool IsBusy { get { return isBusy; } }
        public event RunCompletedEventHandler RunCompletedEvent;

        public MockSuctionCardReaderWriter()
        {
            readAsyncCaller = new RunAsyncCaller(Read);
            writeAsyncCaller = new RunAsyncCaller(Write);
        }

        public void Initialize()
        {
 
        }

        public void ReadAsync(JObject jo)
        {
            isBusy = true;
            cancelled = false;
            readAsyncCaller.BeginInvoke(jo, new AsyncCallback(Callback), jo);
        }

        public void WriteAsync(JObject jo)
        {
            isBusy = true;
            cancelled = false;
            writeAsyncCaller.BeginInvoke(jo, new AsyncCallback(Callback), jo);
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
            cancelled = true;
        }

        public void Dispose()
        {

        }

        public void Read(JObject jo)
        {
            jo["cardNo"] = "9999888877776666";
            jo["name"] = "王五";
            jo["certType"] = String.Empty;
            jo["certNo"] = "3446133198505124017";
            jo["serialNo"] = "MNBVCS";
            jo["result"] = ErrorCode.Success;

            Thread.Sleep(1000);
        }

        public void Write(JObject jo)
        {
            jo["result"] = ErrorCode.Success;
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