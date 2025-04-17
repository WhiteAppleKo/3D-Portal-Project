using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    private static readonly int MOVE_X = Animator.StringToHash("MoveX");
    private static readonly int MOVE_Z = Animator.StringToHash("MoveZ");
    private static readonly int JUMP = Animator.StringToHash("Jump");

    [Header("Player Movement")]
    public float walkSpeed = 1f;
    public float runSpeed = 2f;
    public float jumpForce = 2f;
    public CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private readonly float gravity = -9.81f;
    
    [Header("Camera Movement")]
    public float mouseSensitivity = 15f;
    public float minY = -40f;
    public float maxY = 50f;
    public Camera mainCamera;
    private float verticalLookRotation = 0f;
    private float horizontalLookRotation = 0f;
    private Quaternion initialCameraRotation;
    private Vector3 rotation;
    public Transform head;
    private float r;
    
    [Header("Animation")]
    public Animator animator;
    private void Start()
    {
        initialCameraRotation = mainCamera.transform.localRotation;
    }

    private void Update()
    {
        UpdateMove();
        UpdateCameraAngle();
    }

    private void UpdateMove()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        float moveX = Input.GetAxis("Horizontal") * speed;
        float moveZ = Input.GetAxis("Vertical") * speed;
        
        animator.SetFloat(MOVE_X, moveX);
        animator.SetFloat(MOVE_Z, moveZ);
        
        // Vector3 move = transform.right * moveX + transform.forward * moveZ;
        // controller.Move(move * (speed * Time.deltaTime));

        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        bool isGrounded = controller.isGrounded;
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            animator.SetBool(JUMP, true);
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    private void UpdateCameraAngle()
    {
        // float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        // float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        //
        // horizontalLookRotation += mouseX;
        // verticalLookRotation -= mouseY;
        // verticalLookRotation = Mathf.Clamp(verticalLookRotation, minY, maxY);
        //
        // Vector3 rotation = new Vector3(verticalLookRotation, horizontalLookRotation, 0f);
        // Quaternion targetRotation = Quaternion.Euler(rotation);
        //
        // // 부드러운 회전 적용 (Lerp 또는 Slerp 사용)
        // mainCamera.transform.localRotation = Quaternion.Slerp(initialCameraRotation, targetRotation, Time.deltaTime * rotationSpeed);
        //
        // // currentRotation 업데이트 (현재 회전 상태 저장)
        // initialCameraRotation = mainCamera.transform.localRotation;
        
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        Quaternion yawRotation = Quaternion.AngleAxis(mouseX, Vector3.up);
        transform.rotation *= yawRotation;

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, minY, maxY);
        float angleInRadians = Mathf.Deg2Rad * verticalLookRotation;

        Quaternion pitchRotation = Quaternion.AngleAxis(verticalLookRotation, Vector3.right);
        head.localRotation = initialCameraRotation * pitchRotation;
        // r * cos(a) 계산
        //float result = r * Mathf.Cos(angleInRadians);
        // Debug.Log($"result: {result}, r: {r}");
        //
        // Vector3 pos = mainCamera.transform.localPosition + Vector3.up * result;
        // mainCamera.transform.localPosition.y = Mathf.Clamp(verticalLookRotation, minY, maxY);
    }
}
