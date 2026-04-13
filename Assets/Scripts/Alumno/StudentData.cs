using UnityEngine;

/*
* Script para almacenar los datos de un alumno.
*  Campos:
*   - studentName: Nombre del alumno.
*   - casualDialogue: Diálogo casual del alumno.
*   - investigationDialogue: Diálogo de investigación del alumno.
*   - isVictim: Si el alumno es víctima.
*   - isBully: Si el alumno es acosador.
*   - lies: Si el alumno miente.
*   - appearsOnDay: Día en el que aparece el alumno.
*/

public class StudentData : MonoBehaviour
{
    public string studentName;

    [TextArea]
    public string casualDialogue;

    [TextArea]
    public string investigationDialogue;

    public bool isVictim;
    public bool isBully;
    public bool lies;

    public int appearsOnDay;
}