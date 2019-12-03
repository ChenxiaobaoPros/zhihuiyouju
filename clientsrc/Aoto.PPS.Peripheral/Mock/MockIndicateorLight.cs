using System.Collections;
using Aoto.PPS.Infrastructure.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Aoto.PPS.Peripheral.Mock
{
    public class MockIndicateorLight : IIndicateorLight
    {
        private string dll;
        private int timeout;
        private bool enabled;
        private bool isBusy;

        public bool Cancelled { get { return enabled; } set { enabled = value; } }
        public bool Enabled { get { return enabled; } }
        public bool IsBusy { get { return isBusy; } }
        public event RunCompletedEventHandler RunCompletedEvent;

        public MockIndicateorLight(string dll, int timeout, bool enabled)
        {
            this.enabled = enabled;
        }

        public void Initialize()
        {

        }

        public void ControlLight(int lightNo, int lightType)
        {

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
    }
}