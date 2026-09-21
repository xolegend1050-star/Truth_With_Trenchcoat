using UnityEngine;

namespace TruthWithTrenchcoat.FPS
{
    /// <summary>
    /// First-person player controller for non-VR play.
    /// WASD movement, mouse look, sprint, jump, gravity.
    /// Attach to the player capsule. Assign playerCamera in Inspector.
    /// </summary>
    public class FPSPlayer : MonoBehaviour
    {
        [Header("Movement")]
        public float walkSpeed = 4f;
        public float sprintSpeed = 7f;
        public float jumpHeight = 1.2f;
        public float gravity = -15f;

        [Header("Look")]
        public float mouseSensitivity = 2f;
        public float maxLookAngle = 80f;
        public Camera playerCamera;

        [Header("Stamina")]
        public float maxStamina = 3f;
        public float staminaDrain = 1.5f;
        public float staminaRegen = 2f;

        private CharacterController controller;
        private float xRotation = 0f;
        private Vector3 velocity;
        private bool isGrounded;
        private float currentStamina;
        private bool isSprinting;

        void Start()
        {
            controller = GetComponent<CharacterController>();
            if (controller == null)
            {
                controller = gameObject.AddComponent<CharacterController>();
                controller.height = 1.8f;
                controller.radius = 0.3f;
                controller.center = new Vector3(0, 0.9f, 0);
            }

            if (playerCamera == null)
            {
                var camObj = new GameObject("PlayerCamera");
                camObj.transform.SetParent(transform);
                camObj.transform.localPosition = new Vector3(0, 1.6f, 0);
                playerCamera = camObj.AddComponent<Camera>();
                playerCamera.nearClipPlane = 0.1f;
                playerCamera.fieldOfView = 70f;
                camObj.AddComponent<AudioListener>();
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            currentStamina = maxStamina;
        }

        void Update()
        {
            HandleGroundCheck();
            HandleMovement();
            HandleMouseLook();
            HandleJump();
            HandleStamina();
            ApplyGravity();
        }

        void HandleGroundCheck()
        {
            isGrounded = controller.isGrounded;
            if (isGrounded && velocity.y < 0)
                velocity.y = -2f;
        }

        void HandleMovement()
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && z > 0;
            float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * currentSpeed * Time.deltaTime);
        }

        void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

            if (playerCamera != null)
                playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * mouseX);
        }

        void HandleJump()
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        void HandleStamina()
        {
            if (isSprinting)
                currentStamina -= staminaDrain * Time.deltaTime;
            else
                currentStamina += staminaRegen * Time.deltaTime;

            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }

        void ApplyGravity()
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        /// <summary>
        /// Call this to unlock cursor (for menus, puzzles, etc.)
        /// </summary>
        public void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        /// <summary>
        /// Call this to re-lock cursor after menu/puzzle closes.
        /// </summary>
        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
