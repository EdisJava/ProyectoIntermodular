using System;
using System.IO;
using UnityEngine;

public static class PlayerSettingsSystem
{
    private const string SettingsFileName = "PlayerSettings.json";

    private static string SettingsPath => Path.Combine(Application.persistentDataPath, SettingsFileName);

    public static PlayerSettingsData Load()
    {
        if (!File.Exists(SettingsPath))
        {
            return new PlayerSettingsData();
        }

        try
        {
            string json = File.ReadAllText(SettingsPath);
            PlayerSettingsData data = JsonUtility.FromJson<PlayerSettingsData>(json);
            return data ?? new PlayerSettingsData();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error al cargar ajustes: {ex.Message}");
            return new PlayerSettingsData();
        }
    }

    public static void Save(PlayerSettingsData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SettingsPath, json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error al guardar ajustes: {ex.Message}");
        }
    }
}
