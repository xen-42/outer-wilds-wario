using OWML.ModHelper;
using OWML.Common;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;
using System;

namespace OuterWario
{
    public class OuterWario : ModBehaviour
    {

        public static OuterWario Instance { get; private set; }

        private void Start()
        {
            ModHelper.Console.WriteLine($"CursedTitle loaded", MessageType.Info);

            Instance = this;

            SceneManager.sceneLoaded += OnSceneLoaded;
            //LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;

            //TitleScreen is already open
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "SolarSystem") return;

            PatchMesh.ReplacePlayer();
            ModHelper.Events.Unity.FireOnNextUpdate(PatchAudio.Apply);
        }

        void OnCompleteSceneLoad(OWScene originalScene, OWScene loadScene)
        {
            PatchAudio.Apply();
        }
    }
}
