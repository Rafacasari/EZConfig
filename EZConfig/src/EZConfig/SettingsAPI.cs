using EZConfig.SettingOptions;
using System;
using UnityEngine;

namespace EZConfig;

public static class SettingsAPI
{
    public static void AddTextToTab(string displayName, string tabName)
    {
        if (SettingsHandler.Instance == null)
        {
            Plugin.Log.LogError("You're registering options too early! Use the Start() function to create new options!");
            return;
        }

        SettingsHandler.Instance.AddSetting(new BepInExHeader(displayName, tabName));
    }

    public static void AddBoolToTab(string displayName, bool defaultValue, string tabName, bool currentValue = false, Action<bool>? saveCallback = null)
    {
        if (SettingsHandler.Instance == null)
        {
            Plugin.Log.LogError("You're registering options too early! Use the Start() function to create new options!");
            return;
        }

        SettingsHandler.Instance.AddSetting(new BepInExOffOn(displayName, defaultValue, tabName, currentValue, saveCallback));
    }

    public static void AddFloatToTab(string displayName, float defaultValue,
        string tabName, float minValue = 0f, float maxValue = 1f, float currentValue = 0f, Action<float>? applyCallback = null)
    {
        if (SettingsHandler.Instance == null)
        {
            Plugin.Log.LogError("You're registering options too early! Use the Start() function to create new options!");
            return;
        }

        SettingsHandler.Instance.AddSetting(new BepInExFloat(displayName, defaultValue, tabName, minValue, maxValue, currentValue, applyCallback));
    }

    public static void AddIntToTab(string displayName, int defaultValue, string tabName, int currentValue = 0, Action<int>? saveCallback = null)
    {
        if (SettingsHandler.Instance == null)
        {
            Plugin.Log.LogError("You're registering options too early! Use the Start() function to create new options!");
            return;
        }

        SettingsHandler.Instance.AddSetting(new BepInExInt(displayName, tabName, defaultValue, currentValue, saveCallback));
    }

    public static void AddStringToTab(string displayName, string defaultValue, string tabName, string currentValue = "", Action<string>? saveCallback = null)
    {
        if (SettingsHandler.Instance == null)
        {
            Plugin.Log.LogError("You're registering options too early! Use the Start() function to create new options!");
            return;
        }

        SettingsHandler.Instance.AddSetting(new BepInExString(displayName, tabName, defaultValue, currentValue, saveCallback));
    }


    public static void AddKeybindToTab(string displayName, KeyCode defaultValue, string tabName, KeyCode currentValue, Action<KeyCode>? saveCallback)
    {
        if (SettingsHandler.Instance == null)
        {
            Plugin.Log.LogError("You're registering options too early! Use the Start() function to create new options!");
            return;
        }

        SettingsHandler.Instance.AddSetting(new BepInExKeyCode(displayName, tabName, defaultValue, currentValue, saveCallback));
    }
}