using System;
using System.Runtime.Remoting.Messaging;
using Aoto.PPS.Infrastructure.ComponentModel;
using log4net;
using Newtonsoft.Json.Linq;

namespace Aoto.PPS.Peripheral.Mock
{
    public class MockWirelessTransceiver : IWirelessTransceiver
    {
        private static readonly ILog log = LogManager.GetLogger("wir"); 

        private string addr;
        private int logLevel;
        private string dll;
        private int timeout;
        private bool enabled;
        private bool cancelled;
        private bool isBusy;
        private RunAsyncCaller readAsyncCaller;
        private RunAsyncCaller writeAsyncCaller;

        public bool Cancelled { get { return enabled; } set { enabled = value; } }
        public bool Enabled { get { return enabled; } }
        public bool IsBusy { get { return isBusy; } }
        public event RunCompletedEventHandler RunCompletedEvent;
        //private Thread thread; 

        public MockWirelessTransceiver(string addr, int logLevel, string dll, int timeout, bool enabled)
        {
            this.addr = addr;
            this.logLevel = logLevel;
            this.dll = dll;
            this.timeout = timeout;
            this.enabled = enabled;

            cancelled = false;
            isBusy = false;
            readAsyncCaller = new RunAsyncCaller(Read);
            writeAsyncCaller = new RunAsyncCaller(Write);
            //thread = new Thread(Run);
            //thread.Start();

            Initialize();
        }

        public void Start()
        {
   
        }

        private void Run()
        {
            //log.Debug("begin");

            //while (true)
            //{
            //    Thread.Sleep(200);

            //    log.Debug("Run loop");
            //}
            
        }

        public void Initialize()
        {
            log.Debug("begin");

            log.Debug("end");
        }

        public void ReadAsync(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            isBusy = true;
            cancelled = false;

            readAsyncCaller.BeginInvoke(jo, new AsyncCallback(Callback), jo);

            log.DebugFormat("end", jo);
        }

        public void Read(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);
            log.Debug("end");
        }

        public void WriteAsync(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);

            writeAsyncCaller.BeginInvoke(jo, new AsyncCallback(Callback), jo);

            log.DebugFormat("end");
        }

        public void Write(JObject jo)
        {
            log.DebugFormat("begin, args: jo = {0}", jo);
            log.Debug("end");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 0：禁用；1：正常；2：忙；3：离线；
        /// </returns>
        public int GetStatus()
        {
            log.Debug("begin");

            if (enabled)
            {
                return StatusCode.Mock;
            }
            else
            {
                return StatusCode.Disabled;
            }
        }

        public int GetStatus(int deviceType, int counterNo, int timeout)
        {
            log.DebugFormat("begin, args: deviceType = {0}, counterNo = {1}, timeout = {2}");

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
            log.Debug("begin");
            cancelled = true;
            log.Debug("end");
        }

        public void Dispose()
        {
            log.Debug("begin");
            log.Debug("end");
        }

        private void Callback(IAsyncResult ar)
        {
            JObject jo = (JObject)ar.AsyncState;

            try
            {
                ((RunAsyncCaller)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);
            }
            catch (Exception e)
            {
                jo["result"] = ErrorCode.Failure;
                log.Error("Error", e);
            }
        }
    }
}
