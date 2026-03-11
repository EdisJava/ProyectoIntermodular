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
                // Solo activamos al profesor si su nombre coincide con el del día
                bool isTodayTeacher = (teacher.teacherName == today.teacherName);
                child.gameObject.SetActive(isTodayTeacher);

                if (isTodayTeacher)
                {
                    teacher.ResetMemory();
                    teacher.SetupTeacherForToday();
                }
            }
            else if (student != null)
            {
                bool shouldBeActive = activeStudents.Contains(student.studentName);
                child.gameObject.SetActive(shouldBeActive);

                if (shouldBeActive)
                {
                    student.ResetMemory();
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

         