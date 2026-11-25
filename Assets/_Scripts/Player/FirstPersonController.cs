using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cameraTransform;

    [Header("Movement")]
    [SerializeField] private float _walkSpeed = 4f;
    [SerializeField] private float _sprintSpeed = 6f;
    [SerializeField] private float _acceleration = 18f;
    [SerializeField] private float _deceleration = 22f;
    [SerializeField] private float _jumpHeight = 1.2f;
    [SerializeField] private float _gravity = -20f;

    [Header("Mouse Look")]
    [SerializeField] private float _mouseSensitivity = 0.15f;
    [SerializeField] private float _minPitch = -85f;
    [SerializeField] private float _maxPitch = 85f;

    [Header("Camera Movement")]
    [SerializeField] private float _walkBobFrequency = 1.8f;
    [SerializeField] private float _sprintBobFrequency = 2.5f;
    [SerializeField] private float _walkBobAmplitude = 0.025f;
    [SerializeField] private float _sprintBobAmplitude = 0.045f;
    [SerializeField] private float _bobSmoothness = 8f;

    private CharacterController _characterController;

    private Vector3 _horizontalVelocity;
    private Vector3 _verticalVelocity;
    private Vector3 _cameraInitialPosition;

    private float _cameraPitch;
    private float _bobTimer;

    private bool _isTabletInteractionMode;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _cameraInitialPosition = _cameraTransform.localPosition;

        SetTabletInteractionMode(false);
    }

    private void Update()
    {
        HandleTabletInput();

        if (_isTabletInteractionMode)
        {
            _horizontalVelocity = Vector3.zero;
            HandleCameraBob();
            return;
        }

        HandleMouseLook();
        HandleMovement();
        HandleCameraBob();
    }

    private void HandleTabletInput()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            SetTabletInteractionMode(!_isTabletInteractionMode);
        }
    }

    private void SetTabletInteractionMode(bool enabled)
    {
        _isTabletInteractionMode = enabled;

        Cursor.lockState = enabled
            ? CursorLockMode.None
            : CursorLockMode.Locked;

        Cursor.visible = enabled;

        _horizontalVelocity = Vector3.zero;
    }

    private void HandleMouseLook()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * _mouseSensitivity;
        float mouseY = mouseDelta.y * _mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        _cameraPitch -= mouseY;
        _cameraPitch = Mathf.Clamp(
            _cameraPitch,
            _minPitch,
            _maxPitch
        );

        _cameraTransform.localRotation =
            Quaternion.Euler(_cameraPitch, 0f, 0f);
    }

    private void HandleMovement()
    {
        if (Keyboard.current == null)
            return;

        Vector2 movementInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            movementInput.y += 1f;

        if (Keyboard.current.sKey.isPressed)
            movementInput.y -= 1f;

        if (Keyboard.current.dKey.isPressed)
            movementInput.x += 1f;

        if (Keyboard.current.aKey.isPressed)
            movementInput.x -= 1f;

        movementInput = Vector2.ClampMagnitude(
            movementInput, 1f
        );

        bool isSprinting =
            Keyboard.current.leftShiftKey.isPressed;

        float currentSpeed = isSprinting
            ? _sprintSpeed
            : _walkSpeed;

        Vector3 moveDirection =
            transform.right * movementInput.x +
            transform.forward * movementInput.y;

        Vector3 targetVelocity =
            moveDirection * currentSpeed;

        float accelerationRate =
            movementInput.sqrMagnitude > 0f
                ? _acceleration
                : _deceleration;

        _horizontalVelocity = Vector3.MoveTowards(
            _horizontalVelocity,
            targetVelocity,
            accelerationRate * Time.deltaTime
        );

        if (_characterController.isGrounded &&
            _verticalVelocity.y < 0f)
        {
            _verticalVelocity.y = -2f;
        }

        if (_characterController.isGrounded &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _verticalVelocity.y =
                Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }

        _verticalVelocity.y +=
            _gravity * Time.deltaTime;

        Vector3 finalMovement =
            _horizontalVelocity + _verticalVelocity;

        _characterController.Move(
            finalMovement * Time.deltaTime
        );
    }

    private void HandleCameraBob()
    {
        bool isMoving =
            !_isTabletInteractionMode &&
            _characterController.isGrounded &&
            _horizontalVelocity.magnitude > 0.1f;

        float targetBob = 0f;

        if (isMoving)
        {
            bool isSprinting =
                Keyboard.current != null &&
                Keyboard.current.leftShiftKey.isPressed;

            float frequency = isSprinting
                ? _sprintBobFrequency
                : _walkBobFrequency;

            float amplitude = isSprinting
                ? _sprintBobAmplitude
                : _walkBobAmplitude;

            _bobTimer += Time.deltaTime * frequency;

            targetBob =
                Mathf.Sin(_bobTimer * Mathf.PI * 2f)
                * amplitude;
        }
        else
        {
            _bobTimer = 0f;
        }

        Vector3 targetPosition =
            _cameraInitialPosition +
            Vector3.up * targetBob;

        _cameraTransform.localPosition = Vector3.Lerp(
            _cameraTransform.localPosition,
            targetPosition,
            Time.deltaTime * _bobSmoothness
        );
    }

    public void ResetMovement()
    {
        _horizontalVelocity = Vector3.zero;
        _verticalVelocity = Vector3.zero;

        _bobTimer = 0f;

        _cameraTransform.localPosition = _cameraInitialPosition;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}