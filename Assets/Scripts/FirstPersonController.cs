using System;
using UnityEngine;
using UnityEngine.InputSystem;

/*
* Script para manejar el movimiento del jugador.
* 
* Metodos:
*   - SetMovement(): Metodo que se llama al presionar el boton de movimiento.
*   - SetLook(): Metodo que se llama al presionar el boton de mirar.
*   - Movement(): Metodo que mueve al jugador.
*   - Look(): Metodo que mira al jugador.
*   
*   Variables:
*   - movementSpeed: Velocidad de movimiento del jugador.
*   - gravity: Gravedad que afecta al jugador.
*   - cameraTransform: Transform de la camara.
*   - sensitivity: Sensibilidad del mouse.
*   - minLimit: Limite minimo de rotacion de la camara.
*   - maxLimit: Limite maximo de rotacion de la camara.
*   - canLook: Si el jugador puede mirar.
*   - _inputAction: Input action del jugador.
*   - _characterController: Character controller del jugador.
*   - _movement: Movimiento del jugador.
*   - _velocity: Velocidad del jugador.
*   - _look: Mirada del jugador.
*   - _currentRotationY: Rotacion actual de la camara.
*
*   Funcionamiento:
*   - Al presionar el boton de movimiento, se llama al metodo SetMovement().
*   - Al presionar el boton de mirar, se llama al metodo SetLook().
*   - El metodo Movement() mueve al jugador.
*   - El metodo Look() mira al jugador.
*
*   Flujo:
*   1. El jugador presiona el boton de movimiento.
*   2. Se llama al metodo SetMovement().
*   3. El jugador presiona el boton de mirar.
*   4. Se llama al metodo SetLook().
*   5. El jugador se mueve.
*   6. El jugador mira.
*/
public class FirstPlayerController : MonoBehaviour
{

    public float movementSpeed = 3;
    public float gravity = -9.8f;
    public Transform cameraTransform;
    public float sensitivity = 0.5f;
    public float minLimit= -80f;
    public float maxLimit= 80f;

    [Header("Footsteps")]
    public AudioSource footstepAudioSource;
    public AudioClip footstepClip;
    public float footstepInterval = 0.5f;
    private float _footstepTimer;

    public bool canLook = true;

    private PlayerInputAction _inputAction;
    private CharacterController _characterController;
    

    private Vector2 _movement;
    private Vector2 _velocity;
    private Vector2 _look;

    private float _currentRotationY;

    /*
    * Metodo que se llama al crear el objeto.
    */
    private void Awake()
    {
        _inputAction = new PlayerInputAction();
        _characterController = GetComponent<CharacterController>();
    }

    /*
    * Metodo que se llama al iniciar el juego.
    */
    private void Start()
    {
        _inputAction.Player.Enable();

        _inputAction.Player.Move.performed += SetMovement;
        _inputAction.Player.Move.canceled += _ => _movement = Vector2.zero;

        _inputAction.Player.Look.performed += SetLook;
        _inputAction.Player.Look.canceled += _ => _look = Vector2.zero;
    }

    /*
    * Metodo que se llama al presionar el boton de mirar.
    */
    private void SetLook(InputAction.CallbackContext obj)
    {
        _look = obj.ReadValue<Vector2>();
    }

    /*
    * Metodo que se llama al presionar el boton de movimiento.
    */
    private void SetMovement(InputAction.CallbackContext obj)
    {
        _movement= obj.ReadValue<Vector2>();
    }

    /*
    * Metodo que se llama cada frame.
    */
    private void Update()
    {
        Movement();
        Look();
    }

    /*
    * Metodo que mira al jugador.
    */
    private void Look() 
    {
        if (!canLook) return;
        Vector2 mouseNormalized= _look * sensitivity;

        _currentRotationY = Mathf.Clamp(_currentRotationY - mouseNormalized.y, minLimit, maxLimit);

        cameraTransform.localRotation = Quaternion.Euler(_currentRotationY, 0, 0);
        transform.Rotate(Vector3.up * mouseNormalized.x);
    }

    /*
    * Metodo que mueve al jugador.
    */
    private void Movement()
    {
        Vector3 move = transform.right * _movement.x + transform.forward * _movement.y;
        _characterController.Move(move * movementSpeed * Time.deltaTime);

        // Comprobamos solamente si hay intención de moverse.
        if (move.sqrMagnitude > 0.01f)
        {
            _footstepTimer -= Time.deltaTime;
            
            if (_footstepTimer <= 0f)
            {
                if (footstepAudioSource != null && footstepClip != null)
                {
                    footstepAudioSource.clip = footstepClip;
                    footstepAudioSource.Play();
                }
                
                // Usamos el intervalo configurado para repetirlo.
                _footstepTimer = footstepInterval; 
            }
        }
        else 
        {
            // Resetear el temporizador para que suene la primera pisada tan pronto como empiece a moverse de nuevo.
            _footstepTimer = 0f;
            
            // Si el jugador se detiene y el audio sigue sonando, lo paramos.
            if (footstepAudioSource != null && footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop();
            }
        }

        _velocity.y += gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }
}
