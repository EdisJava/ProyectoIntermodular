using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/*
* Script para manejar la interfaz de usuario de preguntas.
* 
* Metodos:
*   - Update(): Metodo que se llama cada frame.
*   - UpdateUI(): Metodo que actualiza la visualizacion de preguntas.
*
*   Variables:
*   - iconPrefab: Prefab del icono de pregunta.
*   - activeIcons: Lista de iconos de pregunta activos.
*
*   Funcionamiento:
*   - Al actualizar, verifica si el numero de preguntas restantes ha cambiado.
*   - Si ha cambiado, actualiza la visualizacion de preguntas.
*
*   Flujo:
*   1. El jugador interactua con el alumno.
*   2. Se llama al metodo Interact().
*   3. Se determina la fase actual del dia.
*   4. Se llama al metodo correspondiente segun la fase.
*   5. Se muestra el dialogo del alumno.
*   6. El jugador puede interactuar con otro alumno.
*/

public class QuestionUI : MonoBehaviour
{
    [Header("Configuracion")]
    public GameObject iconPrefab;

    private List<GameObject> activeIcons = new List<GameObject>();

    /*
    * Metodo que se llama al inicio.
    */
    void Start()
    {
        UpdateUI();
    }

    /*
    * Metodo que se llama cada frame.
    */
    void Update()
    {
        // Para que sea automatico, compara el conteo
        int remaining = GameManager.Instance.GetRemainingQuestions();
        if (remaining != activeIcons.Count)
        {
            UpdateUI();
        }
    }

    /*
    * Metodo que actualiza la visualizacion de preguntas.
    */
    public void UpdateUI()
    {
        // Limpiar iconos viejos
        foreach (GameObject icon in activeIcons)
        {
            Destroy(icon);
        }
        activeIcons.Clear();

        // Crear iconos nuevos segun las preguntas restantes
        int remaining = GameManager.Instance.GetRemainingQuestions();

        // Solo muestra iconos si esta en fase de investigacion o decision
        if (GameManager.Instance.currentDayPhase != DayPhase.CasualTalk)
        {
            for (int i = 0; i < remaining; i++)
            {
                GameObject newIcon = Instantiate(iconPrefab, transform);
                activeIcons.Add(newIcon);
            }
        }
    }
}