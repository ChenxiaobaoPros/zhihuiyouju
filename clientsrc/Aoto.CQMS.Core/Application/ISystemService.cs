using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Aoto.CQMS.Core.Application
{
    public interface ISystemService
    {
        void SetVolume(JObject jo);

        void SetMute(JObject jo);

        void SetVolume(double val);

        void SetMute(bool isMute);

        void Restart(JObject jo);

        void Shutdown(JObject jo);

        void AppRestart(JObject jo);

        long GetIdleTick();

        void SetLocalTime(DateTime current);

        void UpdateFromUsbAsync(JObject jo);

        void UpdateFromUsb(JObject jo);

        void BackupData();

        void SyncData();

        void DeleteLogs();
    }
}
