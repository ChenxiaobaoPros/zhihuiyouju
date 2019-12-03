using System;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Aoto.PPS.Infrastructure.ComponentModel;
using Aoto.PPS.Infrastructure.Configuration;
using Newtonsoft.Json.Linq;

namespace Aoto.PPS.Peripheral.Mock
{
    public class MockMagneticCardReaderWriter : IReader, IWriter
    {
        private string dll;
        private string priority;
        private int timeout;
        private bool enabled;
        private bool isBusy;
        private RunAsyncCaller readAsyncCaller;

        public bool Cancelled { get { return enabled; } set { enabled = value; } }
        public bool Enabled { get { return enabled; } }
        public bool IsBusy { get { return isBusy; } }
        public event RunCompletedEventHandler RunCompletedEvent;

        public MockMagneticCardReaderWriter()
        {
            this.enabled = Config.App.Peripheral["magneticCardReaderWriter"].Value<bool>("enabled");
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
            jo["cardNo"] = "9999888877776666";
            jo["result"] = ErrorCode.Success;

            Thread.Sleep(1000);
        }

        public void WriteAsync(JObject jo)
        {
            Write(jo);
        }

        public void Write(JObject jo)
        {
            jo["result"] = ErrorCode.Success;
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