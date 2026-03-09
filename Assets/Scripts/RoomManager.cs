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

        foreach (Transform child in transform)
        {
            StudentNPC student = child.GetComponent<StudentNPC>();
            TeacherNPC teacher = child.GetComponent<TeacherNPC>(); // Añadimos esto

            // --- SI ES EL PROFESOR ---
            if (teacher != null)
            {
                // El profesor siempre debe estar activo si el profesor tiene una configuración para hoy
                // O puedes añadir una lista de profesores en el schedule si quieres que cambien
                teacher.gameObject.SetActive(true);
                teacher.SetupTeacherForToday();
            }
            // --- SI ES UN ESTUDIANTE ---
            else if (student != null)
            {
                bool shouldBeActive = todayConfig != null && todayConfig.activeStudentNames.Contains(student.studentName);
                child.gameObject.SetActive(shouldBeActive);

                if (shouldBeActive)
                {
                    student.SetupCharacterForToday();
                }
            }
            // --- SI ES MUEBLE / FONDO ---
            else
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}