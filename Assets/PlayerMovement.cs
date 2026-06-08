using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동")]
    public float speed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float rotationSmoothTime = 0.1f;

    [Header("마우스")]
    public float firstPersonSensitivity = 120f;

    [Header("카메라")]
    public Transform firstPersonCamera;
    public Transform cameraTarget;
    public Transform thirdPersonCamera;

    [Header("캐릭터 오브젝트")]
    public Animator characterAnimator;
    public GameObject playerBody;
    public GameObject helmetObject;

    [Header("지면 보정")]
    public float groundSnapForce = 10f;
    public float groundCheckDistance = 0.3f;                        // Force = 중력 , Check = 지면판정
    public LayerMask groundLayer;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float pitch;
    private float currentVelocity;

    private bool isFirstPerson = true;
    private bool isJumping;
    private bool isDefending;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;                    // 커서 중앙고정 , 1인칭시작
        SetView(true);
    }

    void Update()
    {
        HandleLook();
        HandleMovement();

        if (Keyboard.current.fKey.wasPressedThisFrame)               // 시점변환 키설정
        {
            isFirstPerson = !isFirstPerson;                         
            SetView(isFirstPerson);
        }

        if (isFirstPerson && firstPersonCamera && cameraTarget)      
        {
            firstPersonCamera.position = cameraTarget.position;      // 1인칭 카메라위치
            firstPersonCamera.rotation = cameraTarget.rotation;
        }
    }

    // ================= LOOK =================
    void HandleLook()
    {
        float mouseX = lookInput.x * firstPersonSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * firstPersonSensitivity * Time.deltaTime;

        if (isFirstPerson)
            transform.Rotate(Vector3.up * mouseX);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        if (cameraTarget)
            cameraTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    // ================= MOVE =================
    void HandleMovement()
    {
        if (!isDefending)
        {
            Transform cam = isFirstPerson ? firstPersonCamera : thirdPersonCamera;

            Vector3 forward = cam.forward;
            Vector3 right = cam.right;
            forward.y = 0f;
            right.y = 0f;

            Vector3 move = (forward.normalized * moveInput.y + right.normalized * moveInput.x);
            controller.Move(move * speed * Time.deltaTime);

            if (characterAnimator)
                characterAnimator.SetFloat("Speed", move.magnitude);

            if (!isFirstPerson && move.magnitude > 0.1f)
            {
                float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    targetAngle,
                    ref currentVelocity,
                    rotationSmoothTime
                );
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
        }
        else
        {
            if (characterAnimator)
                characterAnimator.SetFloat("Speed", 0f);
        }
        // 중력 처리
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
            isJumping = false;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        if (characterAnimator)
            characterAnimator.SetBool("IsGrounded", controller.isGrounded);
    }

    // ================= VIEW =================
    void SetView(bool firstPerson)
    {
        firstPersonCamera.gameObject.SetActive(firstPerson);
        thirdPersonCamera.gameObject.SetActive(!firstPerson);
        playerBody.SetActive(!firstPerson);
        helmetObject.SetActive(!firstPerson);
    }

    // ================= INPUT =================
    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnLook(InputValue value) => lookInput = value.Get<Vector2>();
    public void OnJump(InputValue value)
    {
        if (!value.isPressed || isDefending)
            return;

        if (controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;

            if (characterAnimator)
            {
                characterAnimator.ResetTrigger("Attack");
                characterAnimator.ResetTrigger("JumpAttack");
                characterAnimator.SetTrigger("Jump");
            }
        }
    }
    public void OnAttack(InputValue value)
    {
        if (!value.isPressed || characterAnimator == null)
            return;

        characterAnimator.ResetTrigger("Attack");
        characterAnimator.ResetTrigger("JumpAttack");

        // isGrounded+isJumping = 공중이면 JumpAttack
        if (!controller.isGrounded && isJumping)
            characterAnimator.SetTrigger("JumpAttack");
        else
            characterAnimator.SetTrigger("Attack");
    }
    public void OnDefense(InputValue value)
    {
        isDefending = value.isPressed;
        if (characterAnimator)
            characterAnimator.SetBool("IsDefending", isDefending);
    }
}
