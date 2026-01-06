using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Movement
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private CharacterController controller;
        [SerializeField] private Transform bodyTransform; //visual/model transform to rotate instead of the root

        [Header("Speeds")] 
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float sprintMultiplier = 1.8f;
        [SerializeField] private float crouchSpeedMultiplier = 0.6f;
        [SerializeField] private float acceleration = 20f;
        [SerializeField] private float deceleration = 25f;

        [Header("Jump / Gravity")] 
        [SerializeField] private float gravity = -24f;
        [SerializeField] private float jumpHeight = 1.6f;
        [SerializeField] private LayerMask groundLayers = ~0;

        [Header("Crouch")] 
        [SerializeField] private float standHeight = 2f;
        [SerializeField] private float crouchHeight = 1f;
        [SerializeField] private float crouchTransitionSpeed = 8f;
        [SerializeField] private bool holdToCrouch = true;

        [Header("Look")] 
        [SerializeField] private float lookSensitivity = 1f;
        [SerializeField] private float lookSmooth = 10f;
        [SerializeField] private bool invertY = false;
        [SerializeField] private float pitchMin = -70f;
        [SerializeField] private float pitchMax = 80f;

        private InputAction _moveAction;
        private InputAction _sprintAction;
        private InputAction _crouchAction;
        private InputAction _jumpAction;
        private InputAction _lookAction;

        private Vector3 _velocity;
        private Vector3 _currentHorizontalVelocity;
        private float _currentSpeed;
        private bool _isCrouched;
        private bool _crouchTogglePressed;

        private float _yaw;
        private float _pitch;

        private void Awake()
        {
            if (controller == null) controller = GetComponent<CharacterController>();
            if (playerInput == null) playerInput = GetComponent<PlayerInput>();

            if (playerInput != null)
            {
                _moveAction = playerInput.actions["Move"];
                _sprintAction = playerInput.actions["Sprint"];
                _crouchAction = playerInput.actions["Crouch"];
                _jumpAction = playerInput.actions["Jump"];
                _lookAction = playerInput.actions["Look"];
            }

            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;
        }

        private void Start()
        {
            if (controller != null)
            {
                controller.height = standHeight;
                controller.center = new Vector3(0f, controller.height / 2f, 0f);
            }

            _yaw = transform.eulerAngles.y;
            if (cameraTransform != null)
            {
                float initialPitch = cameraTransform.localEulerAngles.x;
                if (initialPitch > 180f) initialPitch -= 360f;
                _pitch = initialPitch;
            }

            ApplyCursorLock(true);
        }

        private void Update()
        {
            if (controller == null) return;

            if (Application.isEditor && Keyboard.current != null)
            {
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    ApplyCursorLock(false);
                }
            }

            HandleLook();

            Vector2 rawMove = _moveAction != null ? _moveAction.ReadValue<Vector2>() : Vector2.zero;
            bool sprinting = _sprintAction != null && _sprintAction.IsPressed();
            bool crouchPressed = _crouchAction != null && _crouchAction.WasPressedThisFrame();

            HandleCrouch(crouchPressed);
            HandleMovement(rawMove, sprinting);
            HandleGravityAndJump();
            ApplyControllerHeight();
        }

        private void ApplyCursorLock(bool locked)
        {
            if (locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void HandleLook()
        {
            Vector2 lookDelta = Vector2.zero;
            if (_lookAction != null)
            {
                lookDelta = _lookAction.ReadValue<Vector2>();
            }
            else if (Mouse.current != null)
            {
                lookDelta = Mouse.current.delta.ReadValue();
            }

            float dx = lookDelta.x * lookSensitivity;
            float dy = lookDelta.y * lookSensitivity;
            if (invertY) dy = -dy;

            _yaw += dx;
            _pitch -= dy;
            _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);

            Transform rotTarget = bodyTransform != null ? bodyTransform : transform;
            Quaternion targetYaw = Quaternion.Euler(0f, _yaw, 0f);
            rotTarget.rotation = Quaternion.Slerp(rotTarget.rotation, targetYaw, lookSmooth * Time.deltaTime);

            if (cameraTransform != null)
            {
                if (cameraTransform.parent == rotTarget)
                {
                    Quaternion targetLocalPitch = Quaternion.Euler(_pitch, 0f, 0f);
                    cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, targetLocalPitch,
                        lookSmooth * Time.deltaTime);
                }
                else
                {
                    Quaternion targetCamRot = Quaternion.Euler(_pitch, _yaw, 0f);
                    cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetCamRot,
                        lookSmooth * Time.deltaTime);
                }
            }
        }

        private void HandleCrouch(bool crouchPressedThisFrame)
        {
            if (holdToCrouch)
            {
                bool held = _crouchAction != null && _crouchAction.IsPressed();
                _isCrouched = held;
            }
            else
            {
                if (crouchPressedThisFrame)
                {
                    if (!_crouchTogglePressed)
                    {
                        _isCrouched = !_isCrouched;
                        _crouchTogglePressed = true;
                    }
                }
                else
                {
                    _crouchTogglePressed = false;
                }
            }

            if (!_isCrouched && !CanStand())
            {
                _isCrouched = true;
            }
        }

        private bool CanStand()
        {
            if (controller == null) return false;
            float checkDistance = (standHeight - crouchHeight);
            Vector3 origin = transform.position + Vector3.up * (crouchHeight + 0.01f);
            return !Physics.SphereCast(origin, controller.radius - 0.01f, Vector3.up, out _, checkDistance,
                groundLayers);
        }

        private void HandleMovement(Vector2 input, bool sprinting)
        {
            Vector3 camForward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
            Vector3 camRight = cameraTransform != null ? cameraTransform.right : Vector3.right;

            camForward.y = 0f;
            camRight.y = 0f;

            if (camForward.sqrMagnitude < 0.0001f) camForward = Vector3.forward;
            if (camRight.sqrMagnitude < 0.0001f) camRight = Vector3.right;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 targetDir = (camForward * input.y + camRight * input.x);
            float targetMagnitude = Mathf.Clamp01(targetDir.magnitude);
            if (targetDir.sqrMagnitude > 0.0001f) targetDir.Normalize();
            else targetDir = Vector3.zero;

            float baseSpeed = walkSpeed;
            if (sprinting && !_isCrouched) baseSpeed *= sprintMultiplier;
            if (_isCrouched) baseSpeed *= crouchSpeedMultiplier;

            float targetSpeed = baseSpeed * targetMagnitude;

            float accel = (targetSpeed > _currentSpeed) ? acceleration : deceleration;
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, accel * Time.deltaTime);

            Vector3 desiredHorizontal = targetDir * _currentSpeed;
            _currentHorizontalVelocity = Vector3.Lerp(_currentHorizontalVelocity, desiredHorizontal,
                Mathf.Clamp01(Time.deltaTime * 10f));

            Vector3 move = _currentHorizontalVelocity + new Vector3(0f, _velocity.y, 0f);
            controller.Move(move * Time.deltaTime);
        }

        private void HandleGravityAndJump()
        {
            bool grounded = controller.isGrounded;

            if (grounded && _velocity.y < 0f)
            {
                _velocity.y = -2f;
            }

            if (_jumpAction != null && _jumpAction.WasPressedThisFrame() && grounded && !_isCrouched)
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            _velocity.y += gravity * Time.deltaTime;
        }

        private void ApplyControllerHeight()
        {
            if (controller == null) return;
            float targetHeight = _isCrouched ? crouchHeight : standHeight;
            float newHeight = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
            controller.height = newHeight;
            controller.center = new Vector3(0f, newHeight / 2f, 0f);
        }
    }
}