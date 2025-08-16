using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using Framework;

namespace YouCantFireMeIQuit;

public class PluginInfo
{
    public const string PLUGIN_GUID = "com.gorzontrok.sittsm.youcantfiremeiquit";
    public const string PLUGIN_NAME = "You Can't Fire Me I Quit";
    public const string PLUGIN_VERSION = "1.0.0";
}

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        try
        {
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Logger.LogInfo($"Patched !");
        }
        catch(Exception ex)
        {
            Logger.LogError(ex);
        }
    }

    [HarmonyPatch("OnQuitPressed")]
    public class OnQuitPressed_Patch
    {
        [HarmonyPatch(typeof(PauseMenu))]
        [HarmonyPrefix]
        public static bool PauseMenuM(PauseMenu __instance)
        {
            Debug.Log("[PAUSE MENU] - OnQuitPressed");
            if (SingletonBehaviour<AreYouSurePanel>.HasInstance)
            {
                __instance.pauseMenuGroup.interactable = false;
                __instance.pauseMenuGroup.blocksRaycasts = false;
                SingletonBehaviour<AreYouSurePanel>.Instance.SetShowing(LocalisationManager.LocaliseKey("UI_Label_Pause_QuitQuestion", null), delegate { Application.Quit(); }, delegate
                {
                    __instance.pauseMenuGroup.interactable = true;
                    __instance.pauseMenuGroup.blocksRaycasts = true;
                }, null, null, true);
                return false;
            }
            Application.Quit();
            return false;
        }
        [HarmonyPatch(typeof(TitleMenu))]
        [HarmonyPrefix]
        public static bool TitleMenuM()
        {
            Debug.Log("[TITLE MENU] - OnQuitPressed");
            Application.Quit();
            return false;
        }
    }
}

