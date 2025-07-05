using HarmonyLib;
using UnityEngine;

namespace EZConfig.UI;
internal static class PeakTemplates
{
    internal static void Initialize()
    {
        Plugin.WARNING("Initializing patches");

        var harmony = new Harmony("ezconfig");
        harmony.PatchAll(typeof(PeakTemplates).Assembly);

        Plugin.WARNING("Patches applied");
    }

    public static GameObject? SettingsCellPrefab { get; internal set; }

    [HarmonyPatch(typeof(MenuWindow), nameof(MenuWindow.Start))]
    private class PauseMainMenu_Initialize
    {
        static bool Prefix(MenuWindow __instance)
        {
            if (__instance is not MainMenu menu || menu.settingsMenu is not PauseMainMenu settingsMenu) return true;

            var sharedSettings = settingsMenu.GetComponentInChildren<SharedSettingsMenu>();

            if (sharedSettings != null)
                SettingsCellPrefab = sharedSettings.m_settingsCellPrefab;

            return true;
        }
    }
}
