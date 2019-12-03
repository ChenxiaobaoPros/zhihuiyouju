using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using Aoto.PPS.Infrastructure;
using Aoto.PPS.Infrastructure.Configuration;
using log4net;

namespace Aoto.PPS.Peripheral.Default
{
    public class VoicePlayer : IVoicePlayer
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int VOIC_Initialize(string lpszConfigXml);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int VOIC_PlaySound(string lpszSoundXml);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int VOIC_GetStatus(out int lpStatus);

        private static readonly ILog log = LogManager.GetLogger("peripheral");
        private IntPtr ptr;
        private int logLevel;

        private VOIC_Initialize voicInitialize;
        private VOIC_PlaySound voicPlaySound;
        private VOIC_GetStatus voicGetStatus;

        public VoicePlayer()
        {
            string dll = Config.App.Peripheral["voicePlayer"].Value<string>("dll");
            this.logLevel = Config.App.Peripheral["voicePlayer"].Value<int>("logLevel");

            string dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, dll);

            if (!File.Exists(dllPath))
            {
                dllPath = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, "lib", dll);
            }

            ptr = Win32ApiInvoker.LoadLibrary(dllPath);
            log.InfoFormat("LoadLibrary: dllPath = {0}, ptr = {1}", dllPath, ptr);

            IntPtr api = Win32ApiInvoker.GetProcAddress(ptr, "VOIC_Initialize");
            voicInitialize = (VOIC_Initialize)Marshal.GetDelegateForFunctionPointer(api, typeof(VOIC_Initialize));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = VOIC_Initialize", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "VOIC_PlaySound");
            voicPlaySound = (VOIC_PlaySound)Marshal.GetDelegateForFunctionPointer(api, typeof(VOIC_PlaySound));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = VOIC_PlaySound", ptr);

            api = Win32ApiInvoker.GetProcAddress(ptr, "VOIC_GetStatus");
            voicGetStatus = (VOIC_GetStatus)Marshal.GetDelegateForFunctionPointer(api, typeof(VOIC_GetStatus));
            log.InfoFormat("GetProcAddress: ptr = {0}, entryPoint = VOIC_GetStatus", ptr);

            string xml = "<Device><DeviceId>VOIC001</DeviceId><LogLevel>" + logLevel + "</LogLevel></Device>";
            int code = voicInitialize(xml);
            log.InfoFormat("invoke {0} -> VOIC_Initialize, args: xml = {1}, return = {2}", dll, xml, code);
        }

        public void Play(string wav)
        {
            if (Exists(ref wav))
            {
                string xml = "<Sound><Mode>PATH</Mode><Language>MANDARIN</Language><Data>" + wav + "</Data></Sound>";
                voicPlaySound(xml);
                log.InfoFormat("play {0}", xml);
            }
        }

        public void PlayAsync(string xml)
        {
            voicPlaySound(xml);
            log.DebugFormat("PlayAsync {0}", xml);
        }

        public void PlayAsync(IList<string> list)
        {
            foreach (string s in list)
            {
                voicPlaySound(s);
                log.DebugFormat("PlayAsync {0}", s);
            }
        }

        private bool Exists(ref string wav)
        {
            if (Path.IsPathRooted(wav))
            {
                log.InfoFormat("wav = {0}", wav);
                return File.Exists(wav);
            }

            string dir = Path.GetDirectoryName(wav);
            string p = String.Empty;

            if (String.IsNullOrEmpty(dir))
            {
                p = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, "sound\\" + wav);
            }
            else
            {
                p = Path.Combine(Config.PeripheralAbsolutePath, PeripheralManager.Dir, wav);
            }

            wav = p;
            log.InfoFormat("wav = {0}", wav);
            return File.Exists(p);
        }

        public void Dispose()
        {
            log.Debug("begin");

            if (IntPtr.Zero != ptr)
            {
                Win32ApiInvoker.FreeLibrary(ptr);
                log.InfoFormat("FreeLibrary: ptr = {0}", ptr);
            }

            log.Debug("end");
        }
    }
}
