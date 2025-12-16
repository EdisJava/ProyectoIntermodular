using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPlayerController : MonoBehaviour
{

    public float movementSpeed = 3;
    public float gravity = -9.8f;
    public Transform cameraTransform;
    public float sensitivity = 0.5f;
    public float minLimit= -80f;
    public float maxLimit= 80f;
    public bool canLook = true;

    private PlayerInputAction _inputAction;
    private CharacterController _characterController;
    

    private Vector2 _movement;
    private Vector2 _velocity;
    private Vector2 _look;

    private float _currentRotationY;

    private void Awake()
    {
        _inputAction = new PlayerInputAction();
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _inputAction.Player.Enable();

        _inputAction.Player.Move.performed += SetMovement;
        _inputAction.Player.Move.canceled += _ => _movement = Vector2.zero;

        _inputAction.Player.Look.performed += SetLook;
        _inputAction.Player.Look.canceled += _ => _look = Vector2.zero;
    }

    private void SetLook(InputAction.CallbackContext obj)
    {
        _look = obj.ReadValue<Vector2>();
    }

    private void SetMovement(InputAction.CallbackContext obj)
    {
        _movement= obj.ReadValue<Vector2>();
    }

    private void Update()
    {
        Movement();
        Look();
    }

    private void Look() 
    {
        if (!canLook) return;
        Vector2 mouseNormalized= _look * sensitivity;

        _currentRotationY = Mathf.Clamp(_currentRotationY - mouseNormalized.y, minLimit, maxLimit);

        cameraTransform.localRotation = Quaternion.Euler(_currentRotationY, 0, 0);
        transform.Rotate(Vector3.up * mouseNormalized.x);
    }

    private void Movement()
    {
        Vector3 move = transform.right * _movement.x + transform.forward * _movement.y;
        _characterController.Move(move * movementSpeed * Time.deltaTime);

        _velocity.y += gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }
}
