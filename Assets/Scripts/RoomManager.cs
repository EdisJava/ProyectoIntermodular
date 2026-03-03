using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    [System.Serializable]
    public class StudentsByDay
    {
        public int day;
        public List<string> activeStudentNames; // Nombres de los alumnos que deben estar hoy
    }

    public List<StudentsByDay> schedule; // Configura esto en el Inspector

    public void RefreshRoom()
    {
        int currentDay = GameManager.Instance.currentDay;
        StudentsByDay todayConfig = schedule.Find(s => s.day == currentDay);

        // Recorremos todos los hijos del Empty (BathHombre, Cocina, etc.)
        foreach (Transform child in transform)
        {
            StudentNPC student = child.GetComponent<StudentNPC>();

            // SI NO ES UN ESTUDIANTE (es el fondo, un mueble, etc.)
            if (student == null)
            {
                child.gameObject.SetActive(true); // Siempre visible
            }
            // SI ES UN ESTUDIANTE
            else
            {
                bool shouldBeActive = todayConfig != null && todayConfig.activeStudentNames.Contains(student.studentName);
                child.gameObject.SetActive(shouldBeActive);

                if (shouldBeActive)
                {
                    student.SetupCharacterForToday();
                }
            }
        }
    }
}