using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Aoto.PPS.Infrastructure.Configuration;
using log4net;

namespace Aoto.PPS.Peripheral.Mock
{
    public class MockVoicePlayer : IVoicePlayer
    {
        private static readonly ILog log = LogManager.GetLogger("peripheral");
        private SoundPlayer soundPlayer;

        public MockVoicePlayer()
        {
            soundPlayer = new SoundPlayer();
        }

        public void Play(string wav)
        {

        }

        public void PlayAsync(string wav)
        {

        }

        public void PlayAsync(string[] wavs)
        {
            string p = String.Empty;

            foreach (string w in wavs)
            {
                p = w;

                if (Exists(ref p))
                {
                    soundPlayer.SoundLocation = p;
                    soundPlayer.PlaySync();
                }
            }
        }

        private bool Exists(ref string wav)
        {
            string dir = Path.GetDirectoryName(wav);
            string p = String.Empty;

            if (String.IsNullOrEmpty(dir))
            {
                p = Path.Combine(Config.AppRoot, "sounds\\pps\\" + wav);
            }
            else
            {
                p = Path.Combine(Config.AppRoot, wav);
            }

            wav = p;
            return File.Exists(p);
        }
    }
}
