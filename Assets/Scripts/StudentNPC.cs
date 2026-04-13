using UnityEngine;
using UnityEngine.UI;

/*
* Script para manejar el comportamiento de los alumnos.
* 
* Metodos:
*   - Start(): Metodo que se ejecuta al iniciar el script.
*   - Interact(): Metodo que se ejecuta al interactuar con el alumno.
*   - MarkAsInterrogated(): Metodo que marca al alumno como interrogado.
*   - BuildProgressData(): Metodo que construye los datos de progreso.
*   - ApplyProgressData(): Metodo que aplica los datos de progreso.
*   - EnterFocus(): Metodo que entra en estado de enfoque.
*   - ExitFocus(): Metodo que sale del estado de enfoque.
*
*   Variables:
*   - studentName: Nombre del alumno.
*   - casualDialogue: Dialogo casual.
*   - investigationDialogue: Dialogo de investigacion.
*   - idleSprite: Sprite en estado de reposo.
*   - centerPoint: Punto central para el enfoque.
*   - myImage: Imagen del alumno.
*   - myRectTransform: Transformacion de la imagen.
*   - startAnchoredPos: Posicion anclada inicial.
*   - startScale: Escala inicial.
*   - alreadyInterrogatedDialogue: Dialogo de interrogatorio previo.
*   - victimStateDialogue: Dialogo de estado de victima.
*   - victimFound: Si se ha encontrado a la victima.
*   - alreadyInterrogated: Si se ha interrogado al alumno.
*   - isVictim: Si el alumno es la victima.
*   - casualRead: Si se ha leido el dialogo casual.
*
*   Funcionamiento:
*   - Controla el dialogo que se muestra segun la fase del dia y si se ha leido el dialogo casual.
*
*   Flujo:
*   1. El jugador interactua con el alumno.
*   2. Se determina la fase actual del dia.
*   3. Se llama al metodo correspondiente segun la fase.
*   4. Se muestra el dialogo del alumno.
*   5. El jugador puede interactuar con otro alumno.
*/

public class StudentNPC : MonoBehaviour
{
    public string studentName;
    public DialogueData casualDialogue;
    public DialogueData investigationDialogue;

    [Header("Sprites")]
    public Sprite idleSprite;
    public Transform centerPoint;

    // componentes y datos para el efecto de enfoque al hablar
    private Image myImage;
    private RectTransform myRectTransform;
    private Vector2 startAnchoredPos;
    private Vector3 startScale;

    // Dialogo que sale si ya leyo el casual y vuelve a hablar con el alumno durante investigacion (o si elige "no tengo mas preguntas")
    [Header("Diálogos Especiales")]
    public DialogueData alreadyInterrogatedDialogue;

    // Dialogo que sale si vuelve a hablar con la victima despues de haberla encontrado
    [Header("Diálogos de Víctima")]
    public DialogueData victimStateDialogue;

    private bool victimFound = false;
    private bool alreadyInterrogated = false; // Control de si ya solto la pista
    public bool isVictim;

    /*
    * Metodo que se ejecuta al iniciar el script.
    */
    void Start()
    {
        myImage = GetComponent<Image>();
        myRectTransform = GetComponent<RectTransform>();
        startAnchoredPos = myRectTransform.anchoredPosition;
        startScale = transform.localScale;
        if (idleSprite) myImage.sprite = idleSprite;
    }

    //controla si el dialogo casual ya fue leido
    private bool casualRead = false;

    /*
    * Metodo que se ejecuta al interactuar con el alumno.
    */
    public void Interact()
    {
        if (Time.timeScale == 0f) return;

        if (DialogueManager.Instance.IsDialogueActive) return;

        if (!GameManager.Instance.IsIn2D()) return;

        //logica de victima, tiene prioridad sobre la de alumno normal porque es un caso especial que no sigue las mismas reglas
        //(si es victima, siempre sale el mismo dialogo la primera vez, y luego otro dialogo distinto cada vez que hablas con ella,
        //sin importar la fase del dia ni si ya leiste el casual o no)
        if (isVictim)
        {
            if (!victimFound)
            {
                victimFound = true;
                DialogueManager.Instance.StartDialogue(casualDialogue, this);
                GameManager.Instance.FoundVictim();
            }
            else
            {
                // si ya encontro a la victima, cada vez que hable con ella sale un dialogo distinto (el victimStateDialogue, si es que se le asignao en el inspector)
                DialogueData nextD = victimStateDialogue != null ? victimStateDialogue : casualDialogue;
                DialogueManager.Instance.StartDialogue(nextD, this);
            }
            return;
        }

        //logica de alumnos

        // CASO A: aun no se ha leido su diálogo casual (maxima prioridad)
        // Saldra este dialogo tanto en fase Csual como en Investigation
        if (!casualRead)
        {
            casualRead = true; // La proxima vez ya pasara a la siguiente logica
            DialogueManager.Instance.StartDialogue(casualDialogue, this);
            return;
        }

        // CASO B: ya se leyo el casual, pero aun no estamos en fase de investigacion (sale el casual de nuevo)
        if (GameManager.Instance.currentDayPhase == DayPhase.CasualTalk)
        {
            DialogueManager.Instance.StartDialogue(casualDialogue, this);
            return;
        }

        // CASO C: ya se leyo el casual y estamos en fase de investigacion
        if (GameManager.Instance.currentDayPhase == DayPhase.Investigation ||
            GameManager.Instance.currentDayPhase == DayPhase.Decision)
        {
            // si ya se leyo el casual y estamos en investigacion,
            // pero ya se interrogo a este alumno antes, sale un
            // dialogo distinto (alreadyInterrogatedDialogue) o el casual de nuevo si no se asigno uno especial para esto
            if (alreadyInterrogated)
            {
                DialogueData nextD = alreadyInterrogatedDialogue != null ? alreadyInterrogatedDialogue : casualDialogue;
                DialogueManager.Instance.StartDialogue(nextD, this);
            }
            else
            {
                // si ya se leyo el casual y estamos en investigacion, y aun no se interrogo a este alumno, sale su dialogo de investigacion normal
                DialogueManager.Instance.StartDialogue(investigationDialogue, this);
            }
        }
    }

    // esta funcion la llama el DialogueManager cuando elige una opcion de interrogatorio sobre este npc,
    // para marcar que ya se interrogo a este npc y asi cambiar su dialogo en futuras interacciones
    public void MarkAsInterrogated()
    {
        alreadyInterrogated = true;
    }

    // funciones para guardar y cargar el progreso de este NPC
    public StudentDialogueProgressData BuildProgressData()
    {
        return new StudentDialogueProgressData
        {
            studentName = studentName,
            casualRead = casualRead,
            alreadyInterrogated = alreadyInterrogated,
            victimFound = victimFound
        };
    }

    // esta funcion se llama desde el GameManager al cargar el progreso, para aplicar los datos guardados a este npc
    public void ApplyProgressData(StudentDialogueProgressData data)
    {
        if (data == null || data.studentName != studentName)
        {
            return;
        }

        casualRead = data.casualRead;
        alreadyInterrogated = data.alreadyInterrogated;
        victimFound = data.victimFound;
    }

    // funciones para el efecto de enfoque al hablar, que se llama desde el DialogueManager al empezar a hablar con este npc, y al dejar de hablar
    public void EnterFocus()
    {
        // si el centerPoint es un objeto de la UI, usamos su posicion anclada
        RectTransform centerRect = centerPoint.GetComponent<RectTransform>();

        if (centerRect != null)
        {
            myRectTransform.anchoredPosition = centerRect.anchoredPosition;
        }
        else
        {
            // si por alguna razon no es UI, seguimos usando position pero es menos estable
            transform.position = centerPoint.position;
        }

        transform.localScale = startScale * 1.3f;
        // ocultar todo el objeto en lugar de solo la imagen previene que queden hijos o textos visibles flotando (como el bug de Aroy que se duplica visualmente)
        gameObject.SetActive(false);
    }
    public void ExitFocus()
    {
        // Volvemos a la posicion anclada original (que no cambia con la resolucion)
        myRectTransform.anchoredPosition = startAnchoredPos;

        transform.localScale = startScale;
        gameObject.SetActive(true);
    }

    // esta funcion se llama desde el GameManager al empezar un nuevo dia, para resetear la memoria de los npcs y que vuelvan a su estado inicial
    // (sin interrogar, sin encontrar la victima, sin haber leido el casualread)
    public void ResetMemory()
    {
        alreadyInterrogated = false;
        victimFound = false;
        casualRead = false;
    }

    // esta funcion se llama desde el GameManager al empezar un nuevo dia, para configurar el dialogo de cada npc segun el escenario del dia
    public void SetupCharacterForToday()
    {

        // obtenemos el escenario del dia actual para configurar el dialogo de este npc segun lo que diga ese escenario
        DayScenario today = GameManager.Instance.GetCurrentDayScenario();
        if (today == null) return;

        // verificamos si este npc es la victima del dia, para configurar su dialogo y su estado de victima
        isVictim = (studentName == today.victimName);
        //log para debug
        if (isVictim) Debug.Log(studentName + " es la víctima hoy.");

        // buscamos la configuracion de los npc en el escenario del dia, para configurar su dialogo casual y de investigacion segun lo que diga esa configuracion
        foreach (var config in today.characterConfigs)
        {
            if (config.characterName == studentName)
            {
                this.casualDialogue = config.casualDialogue;
                if (!isVictim)
                {
                    this.investigationDialogue = config.isLiarToday ? config.lieDialogue : config.truthDialogue;
                }
                break;
            }
        }
    }
}
