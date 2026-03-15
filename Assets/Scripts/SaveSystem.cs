using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private const string SaveFileName = "savegame.json";

    private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    public static bool HasSave()
    {
        return File.Exists(SavePath);
    }

    public static void SaveGame(SaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"Partida guardada en: {SavePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error al guardar la partida: {ex.Message}");
        }
    }

    public static SaveData LoadGame()
    {
        if (!HasSave())
        {
            return null;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            if (data == null)
            {
                Debug.LogWarning("El archivo de guardado existe, pero no se pudo deserializar.");
            }

            return data;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error al cargar la partida: {ex.Message}");
            return null;
        }
    }

    public static void DeleteSave()
    {
        if (!HasSave())
        {
            return;
        }

        try
        {
            File.Delete(SavePath);
            Debug.Log("Partida guardada eliminada.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error al borrar la partida: {ex.Message}");
        }
    }
}
