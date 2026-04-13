using System;
using System.IO;
using UnityEngine;

/*
* Script para guardar la configuracion del jugador.
* 
* Metodos:
*   - Load(): Metodo que carga la configuracion del jugador.
*   - Save(): Metodo que guarda la configuracion del jugador.
*
*   Variables:
*   - SettingsFileName: Nombre del archivo de configuracion.
*   - SettingsPath: Ruta del archivo de configuracion.
*
*   Funcionamiento:
*   - Guarda la configuracion del jugador.
*
*   Flujo:
*   1. El jugador cambia la configuracion.
*   2. Se guarda la configuracion.
*/

public static class PlayerSettingsSystem
{
    private const string SettingsFileName = "PlayerSettings.json";

    private static string SettingsPath => Path.Combine(Application.persistentDataPath, SettingsFileName);

    /*
    * Metodo que carga la configuracion del jugador.
    */
    public static PlayerSettingsData Load()
    {
        // Si no existe el archivo, devuelve una configuracion por defecto
        if (!File.Exists(SettingsPath))
        {
            return new PlayerSettingsData();
        }

        // Intenta leer el archivo y convertirlo a PlayerSettingsData
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

    /*
    * Metodo que guarda la configuracion del jugador.
    */
    public static void Save(PlayerSettingsData data)
    {
        // Intenta guardar la configuracion del jugador
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
