using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoto.EMS.Infrastructure.ComponentModel
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
}
