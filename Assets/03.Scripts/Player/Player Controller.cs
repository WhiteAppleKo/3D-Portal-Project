using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 12f;
    public CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private readonly float gravity = -9.81f;
    
    [Header("Camera Movement")]
    public float mouseSensitivity = 15f;
    public float minY = -60f;
    public float maxY = 60f;
    public Transform target;
    private float verticalLookRotation = 0f;
    private Quaternion initialTargetPosition;

    public float latRad = 0;
    public float lonRad = 0;
    private void Start()
    {
        initialTargetPosition = target.transform.localRotation;
    }

    private void Update()
    {
        UpdateMove();
        UpdateCameraAngle();
    }

    private void UpdateMove()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * (speed * Time.deltaTime));

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        bool isGrounded = controller.isGrounded;
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    private void UpdateCameraAngle()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        Quaternion yawRotation = Quaternion.AngleAxis(mouseX, Vector3.up);
        //target.transform.rotation *= yawRotation;
        

        //Quaternion pitchRotation = Quaternion.AngleAxis(verticalLookRotation, Vector3.right);
        //target.transform.localRotation = initialTargetPosition * pitchRotation;

        latRad += Mathf.Deg2Rad * mouseY;
        lonRad += Mathf.Deg2Rad * mouseX;
        latRad = Mathf.Clamp(latRad, minY, maxY);

        // 구체 표면에서의 x, y, z 좌표 계산
        float x = 10 * Mathf.Cos(latRad) * Mathf.Cos(lonRad);
        float y = 10 * Mathf.Sin(latRad);
        float z = 10 * Mathf.Cos(latRad) * Mathf.Sin(lonRad);
        
        target.transform.position = new Vector3(-x, y, z);
    }
}
