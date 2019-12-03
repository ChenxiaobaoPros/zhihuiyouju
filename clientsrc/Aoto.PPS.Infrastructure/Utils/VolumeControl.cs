using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreAudioApi;

namespace Aoto.PPS.Infrastructure.Utils
{
    /// <summary>
    /// 声音控件
    /// </summary>
    public class VolumeControl
    {
        private static VolumeControl _VolumeControl;
        private MMDevice device;
        public bool InitializeSucceed;

        public static VolumeControl Instance
        {
            get
            {
                if (_VolumeControl == null)
                    _VolumeControl = new VolumeControl();
                return _VolumeControl;
            }
        }

        private VolumeControl()
        {
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            try
            {
                device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                InitializeSucceed = true;
            }
            catch
            {
                InitializeSucceed = false;
            }
        }

        /// <summary>
        /// 控制声音
        /// </summary>
        public double MasterVolume
        {
            get { return InitializeSucceed ? device.AudioEndpointVolume.MasterVolumeLevelScalar * 100 : 0; }
            set
            {
                if (InitializeSucceed)
                {
                    device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(value / 100.0f);
                    if (this.IsMute)
                        this.IsMute = false;
                }
            }
        }

        /// <summary>
        /// 静音 
        /// </summary>
        public bool IsMute
        {
            get { return InitializeSucceed ? device.AudioEndpointVolume.Mute : true; }
            set { if (InitializeSucceed)device.AudioEndpointVolume.Mute = value; }
        }

    }

}
