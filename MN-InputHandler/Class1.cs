using OWML.Common;
using OWML.ModHelper;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MN_InputHandler
{
    public class MainClass : ModBehaviour
    {
        public static string selectedMod;
        public static List<string> modList = new List<string>();
        public static int mod = 0;

        bool _enableGUI = false;

        void Start()
        {
            base.ModHelper.Console.WriteLine("[MN-INPUTHANDLER] :");

            SceneManager.sceneLoaded += this.OnSceneLoaded;

            foreach (var gameObj in GameObject.FindObjectsOfType(typeof(ModBehaviour)))
            {
                if (!gameObj.name.Contains("InputHandler"))
                {
                    foreach (var item in GameObject.Find(gameObj.name).GetComponents<ModBehaviour>())
                    {
                        if ((item.GetType().GetMethod("MNDeactivateInput") != null) && (item.GetType().GetMethod("MNActivateInput") != null))
                        {
                            modList.Add(gameObj.name);
                            base.ModHelper.Console.WriteLine(":     " + gameObj.name);
                        }
                    }
                }
            }

            base.ModHelper.Console.WriteLine(": Found " + modList.Count + " compatible mods.");

            if (modList.Count == 0)
            {
                base.ModHelper.Console.WriteLine(": No mods found!");
            }

            if (modList.Count == 1)
            {
                base.ModHelper.Console.WriteLine(": Only one mod found! Defaulting input to ACTIVE...");
                SelectMod(modList[0]);
            }
        }

        public override void Configure(IModConfig config)
        {
            this._enableGUI = config.GetSettingsValue<bool>("enableGUI");
            base.ModHelper.Console.WriteLine(_enableGUI);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            
            if (modList.Count > 1)
            {
                SelectMod(modList[0]);
            }
        }

        private void OnGUI()
        {
            if (_enableGUI)
            {
                if (modList.Count != 0)
                {
                    GUI.Label(new Rect(((float)Screen.width / 2) - 150f, 10f, 300f, 20f), ("INPUT ACTIVE FOR : " + modList[mod]));
                }
            }
            
        }

        void Update()
        {
            if (modList.Count > 1)
            {
                if (Keyboard.current[Key.Comma].wasPressedThisFrame)
                {
                    AudioSource.PlayClipAtPoint(Locator.GetAudioManager().GetAudioClipArray(AudioType.Menu_LeftRight)[0], Locator.GetActiveCamera().transform.position);
                    DeselectMod(modList[mod]);
                    mod -= 1;
                    if (mod < 0)
                        mod = modList.Count - 1;
                    SelectMod(modList[mod]);
                }

                if (Keyboard.current[Key.Period].wasPressedThisFrame)
                {
                    AudioSource.PlayClipAtPoint(Locator.GetAudioManager().GetAudioClipArray(AudioType.Menu_LeftRight)[0], Locator.GetActiveCamera().transform.position);
                    DeselectMod(modList[mod]);
                    mod += 1;
                    if (mod > modList.Count - 1)
                        mod = 0;
                    SelectMod(modList[mod]);
                }
            }
        }

        void SelectMod(string modName)
        {
            foreach (var item in GameObject.Find(modName).GetComponents<ModBehaviour>())
            {
                item.GetType().GetMethod("MNActivateInput").Invoke(item, null);
            }
                
        }
        void DeselectMod(string modName)
        {
            foreach (var item in GameObject.Find(modName).GetComponents<ModBehaviour>())
            {
                item.GetType().GetMethod("MNDeactivateInput").Invoke(item, null);
            }
        }
    }
}
