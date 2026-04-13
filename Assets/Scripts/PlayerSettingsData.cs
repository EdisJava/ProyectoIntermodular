using System;

/*
* Script para guardar la configuracion del jugador.
* 
* Metodos:
*   - None
*
*   Variables:
*   - masterVolume: Volumen general.
*   - brightness: Brillo de la pantalla.
*   - fullscreen: Pantalla completa.
*   - resolutionIndex: Indice de la resolucion.
*
*   Funcionamiento:
*   - Guarda la configuracion del jugador.
*
*   Flujo:
*   1. El jugador cambia la configuracion.
*   2. Se guarda la configuracion.
*/

[Serializable]
public class PlayerSettingsData
{
    public float masterVolume = 1f;
    public float brightness = 1f;
    public bool fullscreen = true;
    public int resolutionIndex = 0;
}
