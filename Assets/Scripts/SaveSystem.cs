using System;
using System.IO;
using UnityEngine;

/*
* Script para guardar la partida.
* 
* Metodos:
*   - HasSave(): Metodo que comprueba si existe una partida guardada.
*   - SaveGame(): Metodo que guarda la partida.
*   - LoadGame(): Metodo que carga la partida.
*   - DeleteSave(): Metodo que elimina la partida guardada.
*
*   Variables:
*   - SaveFileName: Nombre del archivo de guardado.
*   - SavePath: Ruta del archivo de guardado.
*
*   Funcionamiento:
*   - Guarda la partida.
*
*   Flujo:
*   1. El jugador guarda la partida.
*   2. Se guarda la partida.
*/

public static class SaveSystem
{
    private const string SaveFileName = "savegame.json";
    // Ruta del archivo de guardado
    private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    /*
    * Metodo que comprueba si existe una partida guardada.
    */
    public static bool HasSave()
    {
        return File.Exists(SavePath);
    }

    /*
    * Metodo que guarda la partida.
    */
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

    /*
    * Metodo que carga la partida.
    */
    public static SaveData LoadGame()
    {
        if (!HasSave())
        {
            return null;
        }
        // Intenta leer el archivo y convertirlo a SaveData
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

    /*
    * Metodo que elimina la partida guardada.
    */
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
