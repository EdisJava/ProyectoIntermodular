using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NPCInteractionState
{
    public string npcId;
    public int remaining;
    public int dayIndex; // día al que corresponde este contador
}

[Serializable]
public class SaveData
{
    public int currentDayIndex = 0;
    public int badDecisionsCount = 0;
    public List<NPCInteractionState> interactionsRemaining = new List<NPCInteractionState>();
    public string saveVersion = "v1";
}

public static class SaveManager
{
    const string SaveKey = "GameSave_v1";

    public static void Save(SaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
            Debug.Log("SaveManager: guardado ok.");
        }
        catch (Exception e)
        {
            Debug.LogError("SaveManager: error guardando: " + e);
        }
    }

    public static SaveData Load()
    {
        if (!HasSave()) return new SaveData();
        string json = PlayerPrefs.GetString(SaveKey);
        try
        {
            var data = JsonUtility.FromJson<SaveData>(json);
            if (data == null) return new SaveData();
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError("SaveManager: error cargando: " + e);
            return new SaveData();
        }
    }

    public static bool HasSave() => PlayerPrefs.HasKey(SaveKey);

    public static void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }
}