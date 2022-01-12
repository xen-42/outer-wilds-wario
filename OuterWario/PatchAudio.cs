using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OuterWario
{
    class PatchAudio
    {
        public static void Apply()
        {
            AudioType[] deathAudio =
            {
                AudioType.Death_BigBang,
                AudioType.Death_Crushed,
                AudioType.Death_Digestion,
                AudioType.Death_Energy,
                AudioType.Death_Instant,
                AudioType.Death_Lava,
                AudioType.Death_Self,
                AudioType.Death_TimeLoop,
            };

            var deathRattle = OuterWario.Instance.ModHelper.Assets.GetAudio("wario dies.wav");

            Util.Log("Loaded death rattle");

            var audioDictionary = Locator.GetAudioManager()._audioLibraryDict;

            Util.Log("Found the audio library dictionary");

            for(int i = 0; i < deathAudio.Length; i++)
            {
                try
                {
                    audioDictionary[(int)deathAudio[i]] = new AudioLibrary.AudioEntry(deathAudio[i], new AudioClip[] { deathRattle }, 0.5f);
                }
                catch
                {
                    audioDictionary.Add((int)deathAudio[i], new AudioLibrary.AudioEntry(deathAudio[i], new AudioClip[] { deathRattle }, 0.5f));
                }
            }

            Util.Log("Patched audio");
        }
    }
}
