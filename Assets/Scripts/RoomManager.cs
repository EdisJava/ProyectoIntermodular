using UnityEngine;
using System.Collections.Generic;

/*
* Script para manejar la sala.
* 
* Metodos:
*   - RefreshRoom(): Metodo que actualiza la sala.
*
*   Variables:
*   - schedule: Horario de los alumnos.
*
*   Funcionamiento:
*   - Actualiza la sala segun el dia actual.
*
*   Flujo:
*   1. El jugador entra en la sala.
*   2. Se llama al metodo RefreshRoom().
*   3. Se actualiza la sala.
*/

public class RoomManager : MonoBehaviour
{
    [System.Serializable]
    public class StudentsByDay
    {
        public int day;
        public List<string> activeStudentNames; // Nombres de los alumnos que deben estar hoy
    }

    public List<StudentsByDay> schedule; // Configura esto en el Inspector
    /*
    * Metodo que actualiza la sala.
    */
    public void RefreshRoom()
    {
        DayScenario today = GameManager.Instance.GetCurrentDayScenario();
        if (today == null) return;

        // Lista de quién debe estar hoy
        List<string> activeStudents = new List<string>();
        activeStudents.Add(today.victimName);
        foreach (var config in today.characterConfigs) activeStudents.Add(config.characterName);

        foreach (Transform child in transform)
        {
            StudentNPC student = child.GetComponent<StudentNPC>();
            TeacherNPC teacher = child.GetComponent<TeacherNPC>();

            if (teacher != null)
            {
                // Solo activa al profesor si su nombre coincide con el del día
                bool isTodayTeacher = (teacher.teacherName == today.teacherName);
                child.gameObject.SetActive(isTodayTeacher);

                if (isTodayTeacher)
                {
                   
                    teacher.SetupTeacherForToday();
                }
            }
            else if (student != null)
            {
                bool shouldBeActive = activeStudents.Contains(student.studentName);
                child.gameObject.SetActive(shouldBeActive);

                if (shouldBeActive)
                {
                    //student.ResetMemory();
                    student.SetupCharacterForToday();
                }
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}

         