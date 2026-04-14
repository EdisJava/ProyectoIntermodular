using UnityEngine;
using System.Collections.Generic;

public class FinalBuenoScript : MonoBehaviour
{
    [Header("Secuencia de Diálogos")]
    public List<DialogueData> dialogosSecuencia; // Arrastra aquí los diálogos en orden
    
    [Header("Personaje que habla (opcional)")]
    public StudentNPC locutorNPC; 
    public TeacherNPC locutorTeacher; 

    private int indiceActual = 0;
    private bool secuenciaActiva = false;

    private void Start()
    {
        // Evitamos que inicie enseguida si no hay nada
        if (dialogosSecuencia != null && dialogosSecuencia.Count > 0)
        {
            EmpezarSecuencia();
        }
    }

    public void EmpezarSecuencia()
    {
        indiceActual = 0;
        secuenciaActiva = true;
        MostrarSiguienteDialogo();
    }

    private void MostrarSiguienteDialogo()
    {
        if (indiceActual < dialogosSecuencia.Count)
        {
            DialogueData dData = dialogosSecuencia[indiceActual];
            
            // Preferimos profesor si está asignado, si no, el alumno, y en caso de que no haya le pasamos null.
            // Nota: DialogueManager espera un StudentNPC o TeacherNPC asignado, o uno falso para funcionar.
            if (locutorTeacher != null)
            {
                DialogueManager.Instance.StartDialogue(dData, locutorTeacher);
            }
            else
            {
                DialogueManager.Instance.StartDialogue(dData, locutorNPC);
            }
        }
        else
        {
            FinalizarSecuencia();
        }
    }

    private void Update()
    {
        // Detectamos cuando termina un diálogo para mostrar el siguiente
        if (secuenciaActiva)
        {
            // Si el DialogueManager ya no tiene el panel activo, significa que el diálogo actual terminó
            if (!DialogueManager.Instance.IsDialogueActive)
            {
                indiceActual++;
                MostrarSiguienteDialogo();
            }
        }
    }

    private void FinalizarSecuencia()
    {
        secuenciaActiva = false;
        Debug.Log("La secuencia de diálogos finales ha terminado.");
        // Aquí podrías mostrar créditos, volver al menú, etc.
    }
}
