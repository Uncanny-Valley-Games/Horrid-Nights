using UnityEngine;
using UnityEngine.InputSystem;

namespace DebugStuff
{
    public class DebugCamera : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        
        [SerializeField] private float lookSensitivity = 1f;
        [SerializeField] private float lookSmooth = 10f;
        [SerializeField] private bool invertY;
        [SerializeField] private float pitchMin = -70f;
        [SerializeField] private float pitchMax = 80f;
        
        [SerializeField] private float moveSpeed = 4f;
        
        private Transform cameraTransform;
        
        private InputAction _moveAction;
        private InputAction _sprintAction;
        private InputAction _crouchAction;
        private InputAction _jumpAction;
        private InputAction _lookAction;
        
        private Vector3 _velocity;
        
        private float _yaw;
        private float _pitch;
        
        private void Awake()
        {
            _moveAction = playerInput.actions["Move"];
            _sprintAction = playerInput.actions["Sprint"];
            _crouchAction = playerInput.actions["Crouch"];
            _jumpAction = playerInput.actions["Jump"];
            _lookAction = playerInput.actions["Look"];
            
            cameraTransform = GetComponent<Transform>();
        }

        private void Start()
        {
            _yaw = transform.eulerAngles.y;
            if (cameraTransform != null)
            {
                var initialPitch = cameraTransform.localEulerAngles.x;
                
                if (initialPitch > 180f) initialPitch -= 360f;
                _pitch = initialPitch;
            }

            ApplyCursorLock(true);
        }
        
        private void Update()
        {
            HandleLook();
            
            var rawDir = _moveAction?.ReadValue<Vector2>() ?? Vector2.zero;

            transform.position += transform.forward * (moveSpeed * rawDir.y * Time.deltaTime);
            transform.position += transform.right * (moveSpeed * rawDir.x * Time.deltaTime);
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

            var dx = lookDelta.x * lookSensitivity;
            var dy = lookDelta.y * lookSensitivity;
            if (invertY) dy = -dy;

            _yaw += dx;
            _pitch -= dy;
            _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);

            var rotTarget = transform;
            var targetYaw = Quaternion.Euler(0f, _yaw, 0f);
            rotTarget.rotation = Quaternion.Slerp(rotTarget.rotation, targetYaw, lookSmooth * Time.deltaTime);

            if (cameraTransform is not null)
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
    }
}
