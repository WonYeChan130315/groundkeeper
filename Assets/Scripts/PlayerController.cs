using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float jumpForce;
    private float currentSpeed;

    [Header("Look Settings")]
    public float mouseSensitivity;

    [Header("References")]
    public LayerMask groundLayer;
    public CinemachineCamera firstPersonCam;
    public CinemachineCamera thirdPersonCam;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isSprinting;
    private bool isGrounded;
    private bool isFirstPerson = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;

        currentSpeed = walkSpeed;
        UpdateCameraPriority();
        ApplySensitivity();
    }

    void OnValidate()
    {
        if (Application.isPlaying) ApplySensitivity();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    public void OnJump(InputValue value)
    {
        if (value.isPressed) Jump();
    }
    public void OnToggleView(InputValue value)
    {
        if (value.isPressed) ToggleCamera();
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
    }

    void FixedUpdate()
    {
        CheckGround();
        Move();
    }

    void CheckGround()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
    }

    void Move()
    {
        Transform camTransform = Camera.main.transform;
        Vector3 forward = camTransform.forward;
        Vector3 right = camTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (forward * moveInput.y + right * moveInput.x).normalized;

        rb.linearVelocity = new Vector3(moveDir.x * currentSpeed, rb.linearVelocity.y, moveDir.z * currentSpeed);

        Quaternion targetRotation = Quaternion.LookRotation(forward);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 0.5f));
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void ToggleCamera()
    {
        CinemachineCamera targetCam = isFirstPerson ? thirdPersonCam : firstPersonCam;
        SyncCameras(targetCam);

        isFirstPerson = !isFirstPerson;
        UpdateCameraPriority();
    }

    void SyncCameras(CinemachineCamera targetCam)
    {
        if (targetCam == null) return;

        // 현재 메인 카메라의 회전값 가져오기
        Vector3 currentAngles = Camera.main.transform.eulerAngles;

        float pitch = currentAngles.x;
        if (pitch > 180) pitch -= 360;

        // 1. 3인칭 카메라(Orbital Follow)일 때 동기화
        var orbitalFollow = targetCam.GetComponent<CinemachineOrbitalFollow>();
        if (orbitalFollow != null)
        {
            // Orbital Follow는 내부적으로 축 이름을 사용합니다.
            orbitalFollow.HorizontalAxis.Value = currentAngles.y; // Horizontal Axis
            orbitalFollow.VerticalAxis.Value = pitch;           // Vertical Axis
        }

        // 2. 1인칭 카메라(Pan Tilt)일 때 동기화
        var panTilt = targetCam.GetComponent<CinemachinePanTilt>();
        if (panTilt != null)
        {
            panTilt.PanAxis.Value = currentAngles.y;
            panTilt.TiltAxis.Value = pitch;
        }
    }

    void UpdateCameraPriority()
    {
        firstPersonCam.Priority = isFirstPerson ? 10 : 5;
        thirdPersonCam.Priority = isFirstPerson ? 5 : 10;
    }

    public void ApplySensitivity()
    {
        SetCamSensitivity(firstPersonCam);
        SetCamSensitivity(thirdPersonCam);
    }

    void SetCamSensitivity(CinemachineCamera cam)
    {
        if (cam == null) return;

        var inputController = cam.GetComponent<CinemachineInputAxisController>();
        if (inputController != null)
        {
            if (inputController.Controllers.Count > 0)
            {
                inputController.Controllers[0].Input.Gain = mouseSensitivity;
            }
            if (inputController.Controllers.Count > 1)
            {
                inputController.Controllers[1].Input.Gain = -mouseSensitivity;
            }
        }
    }
}