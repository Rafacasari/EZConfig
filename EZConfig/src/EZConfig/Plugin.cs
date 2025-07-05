using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using EZConfig.Components;
using EZConfig.UI;
using PEAKLib.UI;
using PEAKLib.UI.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EZConfig;

[BepInAutoPlugin]
[BepInDependency(UIPlugin.Id)]
public partial class Plugin : BaseUnityPlugin
{
    public static Plugin instance = null!;
    internal static ManualLogSource Log { get; private set; } = null!;

    private void Awake()
    {
        instance = this;
        Log = Logger;
        Log.LogInfo($"Plugin {Name} is loaded!");

        PeakTemplates.Initialize();

        INFO("Load complete!");
    }

    private void Start()
    {
        LoadModSettings();

        void builderDelegate(Transform parent)
        {
            var modSettingsPage = MenuAPI.CreatePage("ModSettings").CreateBackground();

            var newText = MenuAPI.CreateText("Mod Settings", "Header")
                .SetFontSize(48)
                .ParentTo(modSettingsPage.transform)
                .SetPosition(new Vector2(100f, -60f));

            var backButton = MenuAPI.CreateMenuButton("Back")?
                .SetColor(new Color(1, 0.5f, 0.2f))
                .ParentTo(modSettingsPage.transform)
                .SetPosition(new Vector2(230, -160))
                .SetWidth(200)
                .OnClick(modSettingsPage.Close);

            var content = new GameObject("Content")
                .AddComponent<PeakElement>()
                .ParentTo(modSettingsPage.transform)
                .SetPivot(new Vector2(0, 1))
                .SetAnchorMin(new Vector2(0, 1))
                .SetAnchorMax(new Vector2(0, 1))
                .SetPosition(new Vector2(428, -70))
                .SetSize(new Vector2(1360, 980));

            var settingsMenu = content.gameObject.AddComponent<ModdedSettingsMenu>();

            var horizontalTabs = new GameObject("TABS")
                .ParentTo(content.transform)
                .AddComponent<PeakHorizontalTabs>();

            var moddedSettingsTABS = horizontalTabs.gameObject.AddComponent<ModdedSettingsTABS>();
            moddedSettingsTABS.SettingsMenu = settingsMenu;
            
            var tabContent = new GameObject("TabContent").ParentTo(content.transform).AddComponent<PeakTabContent>();

            settingsMenu.ContentParent = tabContent.Content;
            settingsMenu.Tabs = moddedSettingsTABS;

            foreach (var (modName, configEntryBases) in GetModConfigEntries())
            {
                var tabButton = horizontalTabs.AddTab(modName);
                var moddedButton = tabButton.AddComponent<ModdedTABSButton>();
                moddedButton.category = modName;
                moddedButton.text = tabButton.GetComponentInChildren<TextMeshProUGUI>();
                moddedButton.SelectedGraphic = tabButton.transform.Find("Selected").gameObject;
            }
           
            var isTitleScreen = SceneManager.GetActiveScene().name == "Title";
            
            var pauseOptionsMenu = FindAnyObjectByType<PauseOptionsMenu>();
            var modSettingsButton = MenuAPI.CreatePauseMenuButton("Mod Settings")?
                .SetColor(new Color(0.15f, 0.75f, 0.85f))
                .ParentTo(parent)
                .OnClick(() =>
                {
                    UIInputHandler.SetSelectedObject(null);
                    if (!isTitleScreen) pauseOptionsMenu?.Close();
                    modSettingsPage.Open();
                });

            if (modSettingsButton != null && isTitleScreen)
                modSettingsButton.SetPosition(new Vector2(-140, -210)).SetWidth(200);
            else if (modSettingsButton != null && !isTitleScreen)
                modSettingsButton.transform.SetSiblingIndex(4);
        }

        MenuAPI.AddToPauseMenu(builderDelegate);
        MenuAPI.AddToMainMenu(builderDelegate);
    }

    internal static void INFO(string message)
    {
        Log.LogDebug(message);
    }

    internal static void ERROR(string message)
    {
        Log.LogError(message);
    }

    internal static void WARNING(string message)
    {
        Log.LogWarning(message);
    }

    private static bool modSettingsLoaded = false;
    private static void LoadModSettings()
    {
        if (modSettingsLoaded) return;

        modSettingsLoaded = true;

        foreach (var (modName, configEntryBases) in GetModConfigEntries())
        {
            foreach (var configEntry in configEntryBases)
            {
                try
                {
                    if (configEntry.SettingType == typeof(bool))
                    {
                        var defaultValue = configEntry.DefaultValue is bool dValue && dValue;
                        var currentValue = configEntry.BoxedValue is bool bValue && bValue;

                        SettingsAPI.AddBoolToTab(configEntry.Definition.Key, defaultValue, modName, currentValue, newVal => configEntry.BoxedValue = newVal);
                    }
                    else if (configEntry.SettingType == typeof(float))
                    {
                        var defaultValue = configEntry.DefaultValue is float cValue ? cValue : 0f;
                        var currentValue = configEntry.BoxedValue is float bValue ? bValue : 0f;
 
                        float minValue = 0f;
                        float maxValue = 1000f;

                        if (configEntry.Description.AcceptableValues is AcceptableValueRange<float> range)
                        {
                            minValue = range.MinValue;
                            maxValue = range.MaxValue;
                        }

                        SettingsAPI.AddFloatToTab(configEntry.Definition.Key, defaultValue, modName, minValue, maxValue, currentValue, newVal => configEntry.BoxedValue = newVal);
                    }

                    else if (configEntry.SettingType == typeof(int))
                    {
                        var defaultValue = configEntry.DefaultValue is int cValue ? cValue : 0;
                        var currentValue = configEntry.BoxedValue is int bValue ? bValue : 0;
                        SettingsAPI.AddIntToTab(configEntry.Definition.Key, defaultValue, modName, currentValue, newVal => configEntry.BoxedValue = newVal);
                    }
                    else if (configEntry.SettingType == typeof(string))
                    {
                        var defaultValue = configEntry.DefaultValue is string cValue ? cValue : "";
                        var currentValue = configEntry.BoxedValue is string bValue ? bValue : "";
                        SettingsAPI.AddStringToTab(configEntry.Definition.Key, defaultValue, modName, currentValue, newVal => configEntry.BoxedValue = newVal);
                    }
                    else if (configEntry.SettingType == typeof(KeyCode))
                    {
                        var defaultValue = configEntry.DefaultValue is KeyCode cValue ? cValue : KeyCode.None;
                        var currentValue = configEntry.BoxedValue is KeyCode bValue ? bValue : KeyCode.None;

                        SettingsAPI.AddKeybindToTab(configEntry.Definition.Key, defaultValue, modName, currentValue, newVal => configEntry.BoxedValue = newVal);
                    }
                    else // Warn about missing SettingTypes
                        WARNING($"{modName} - {configEntry.Definition.Key} - {configEntry.SettingType}");
                }

                catch (Exception e)
                {
                    Log.LogError(e);
                }
            }

        }
    }

    // From https://github.com/IsThatTheRealNick/REPOConfig/blob/main/REPOConfig/ConfigMenu.cs#L453
    private static Dictionary<string, ConfigEntryBase[]> GetModConfigEntries()
    {
        var peakConfigs = new Dictionary<string, ConfigEntryBase[]>();

        foreach (var plugin in Chainloader.PluginInfos.Values.OrderBy(p => p.Metadata.Name))
        {
            var configEntries = new List<ConfigEntryBase>();

            foreach (var configEntryBase in plugin.Instance.Config.Select(configEntry => configEntry.Value))
            {
                var tags = configEntryBase.Description?.Tags;

                if (tags != null && tags.Contains("Hidden")) continue;

                configEntries.Add(configEntryBase);
            }

            if (configEntries.Count > 0)
                peakConfigs.TryAdd(FixNaming(plugin.Metadata.Name), [.. configEntries]);
        }

        return peakConfigs;
    }

    private static string FixNaming(string input)
    {
        input = Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
        input = Regex.Replace(input, "([A-Z])([A-Z][a-z])", "$1 $2");
        input = Regex.Replace(input, @"\s+", " ");
        input = Regex.Replace(input, @"([A-Z]\.)\s([A-Z]\.)", "$1$2");

        return input.Trim();
    }
}
