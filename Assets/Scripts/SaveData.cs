using System;
using System.Collections.Generic;

/*
* Script para guardar los datos del juego.
*   
*  Es serializable para poder guardarlo en un archivo JSON.
*  Seriarizable se pone porque si no no se puede guardar en un archivo JSON.
*
*   Variables:
*   - sceneName: Nombre de la escena actual.
*   - currentDay: Dia actual.
*   - currentDayPhase: Fase actual del dia.
*   - remainingQuestions: Preguntas restantes.
*   - goodDecisions: Decisiones buenas.
*   - badDecisions: Decisiones malas.
*   - hasAccusedThisDay: Si se ha acusado este dia.
*   - hasReadPrologueLetter: Si se ha leido la carta del prologo.
*   - isEndingPhase: Si se esta en la fase de final.
*   - isGoodEnding: Si es el final bueno.
*   - studentProgress: Progreso de los dialogos de los alumnos.
*   - teacherProgress: Progreso de los dialogos del profesor.
*   - hasPlayerPosition: Si tiene posicion de jugador.
*   - playerPosX: Posicion X del jugador.
*   - playerPosY: Posicion Y del jugador.
*   - playerPosZ: Posicion Z del jugador.
*   - hasPlayerRotation: Si tiene rotacion de jugador.
*   - playerRotX: Rotacion X del jugador.
*   - playerRotY: Rotacion Y del jugador.
*   - playerRotZ: Rotacion Z del jugador.
*   - playerRotW: Rotacion W del jugador.
*
*   Funcionamiento:
*   - Guarda los datos del juego.
*
*   Flujo:
*   1. El jugador guarda el juego.
*   2. Se guardan los datos del juego.
*/

[Serializable]
public class SaveData
{
    public string sceneName;
    public int currentDay;
    public int currentDayPhase;
    public int remainingQuestions;
    public int goodDecisions;
    public int badDecisions;
    public bool hasAccusedThisDay;
    public bool hasReadPrologueLetter;
    public bool isEndingPhase;
    public bool isGoodEnding;
    public List<StudentDialogueProgressData> studentProgress = new List<StudentDialogueProgressData>();
    public TeacherDialogueProgressData teacherProgress;
    public bool hasPlayerPosition;
    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;
    public bool hasPlayerRotation;
    public float playerRotX;
    public float playerRotY;
    public float playerRotZ;
    public float playerRotW;
}

[Serializable]
public class StudentDialogueProgressData
{
    public string studentName;
    public bool casualRead;
    public bool alreadyInterrogated;
    public bool victimFound;
}

[Serializable]
public class TeacherDialogueProgressData
{
    public string teacherName;
    public bool casualRead;
    public bool hasAccusedToday;
}
