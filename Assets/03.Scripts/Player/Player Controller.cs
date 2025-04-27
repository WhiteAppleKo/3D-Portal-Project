using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerController : PortalTraveller
{
    private static readonly int MOVE_X = Animator.StringToHash("MoveX");
    private static readonly int MOVE_Z = Animator.StringToHash("MoveZ");
    private static readonly int JUMP = Animator.StringToHash("Jump");

    [Header("Player Movement")]
    private float walkSpeed = 4f;
    private float runSpeed = 8f;
    public float smoothMoveTime = 0.1f;
    public CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private readonly float gravity = 18;
    private int changeSpeed = 1;
    private int changeJump = 1;
    
    [Header("Camera Movement")]
    public float mouseSensitivity = 15f;
    public Vector2 pitchMinMax = new Vector2 (-40, 50);
    public Camera mainCamera;
    public float rotationSmoothTime = 0.1f;
    private float verticalLookRotation = 0f;
    private Quaternion initialCameraRotation;
    private Vector3 rotation;
    public Transform head;
    private float r;
    
    public bool lockCursor;
    
    public float yaw;
    public float pitch;
    float smoothYaw;
    float smoothPitch;
    
    [Header("Animation")]
    public Animator animator;

    public Camera rayCamera;
    private RenderTexture renderTexture;
    private Texture2D reusableTexture;
    public Transform cubeTransform;
    private GameObject cube;
    private Rigidbody rb;
    private void Start()
    {
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        yaw = transform.eulerAngles.y;
        pitch = mainCamera.transform.localEulerAngles.x;
        smoothYaw = yaw;
        smoothPitch = pitch;
        initialCameraRotation = mainCamera.transform.localRotation;
        
        renderTexture = new RenderTexture (Screen.width, Screen.height, 24);
        rayCamera.targetTexture = renderTexture;
    }

    private void Update()
    {
        UpdateMove();
        UpdateCameraAngle();
        if (Input.GetKeyDown(KeyCode.C)) // C 키를 눌러 색상 가져오기
        {
            Color color = GetPixelColorAtRayHit();
            Debug.Log($"발 아래 픽셀 색상: {color}");
        }
        
        if(Input.GetKeyDown(KeyCode.E))
        {
            // 큐브를 잡는 기능
            PickUpCube();
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Cursor.lockState = CursorLockMode.None; // 먼저 잠금 해제
            Cursor.visible = true; // 이후 커서 표시
        }
    }
    Vector3 smoothV;
    float verticalVelocity;
    bool jumping;
    float lastGroundedTime;
    public float jumpForce = 8;
    private void UpdateMove()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
        Vector3 inputDir = new Vector3(input.x, 0, input.y).normalized;
        Vector3 worldInputDir = transform.TransformDirection(inputDir);
        Vector3 targetVelocity = worldInputDir * (speed * changeSpeed);
        animator.SetFloat(MOVE_X, input.x * speed);
        animator.SetFloat(MOVE_Z, input.y * speed);

        
        
        velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref smoothV, smoothMoveTime);
        
        verticalVelocity -= gravity * Time.deltaTime;
        velocity = new Vector3(velocity.x, verticalVelocity, velocity.z);
        
        var flags = controller.Move(velocity * Time.deltaTime);
        if (flags == CollisionFlags.Below)
        {
            jumping = false;
            lastGroundedTime = Time.time;
            verticalVelocity = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            float timeSinceLastGroundedTime = Time.time - lastGroundedTime;
            if(controller.isGrounded || (!jumping && timeSinceLastGroundedTime < 0.15f))
            {
                jumping = true;
                animator.SetBool(JUMP, true);
                verticalVelocity = jumpForce * changeJump;
            }
        }
        /*Vector3 move;
        if (moveZ < 0)
        {
            move = (transform.right * moveX + transform.forward * -1) * walkSpeed;
        }
        else
        {
            move = (transform.right * moveX + transform.forward * moveZ) * speed;
        }
        // 마찰력 적용
        velocity.x = move.x;
        velocity.z = move.z;
        
        isGrounded = controller.isGrounded;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool(JUMP, true);
            UpdateJump(true);
        }
        else
        {
            UpdateJump(false);
        }
        
        controller.Move(velocity * Time.deltaTime);*/
    }

    private void UpdateJump(bool isJumping)
    {
        if (isGrounded == false)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = 0;
        }
        if(isJumping)
        {
            velocity.y = Mathf.Sqrt(-gravity * 3f * changeJump);
        }
    }

    private void UpdateCameraAngle()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        Quaternion yawRotation = Quaternion.AngleAxis(mouseX, Vector3.up);
        transform.rotation *= yawRotation;

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, pitchMinMax.x, pitchMinMax.y);

        Quaternion pitchRotation = Quaternion.AngleAxis(verticalLookRotation, Vector3.right);
        head.localRotation = initialCameraRotation * pitchRotation;
    }
    
    public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        transform.position = pos;
        Vector3 eulerRot = rot.eulerAngles;
        float delta = Mathf.DeltaAngle (smoothYaw, eulerRot.y);
        yaw += delta;
        smoothYaw += delta;
        transform.eulerAngles = Vector3.up * smoothYaw;
        velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (velocity));
        Physics.SyncTransforms ();
    }
    // public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
    //     transform.position = pos;
    //     Debug.Log(pos);
    //     Vector3 eulerRot = rot.eulerAngles;
    //     float delta = Mathf.DeltaAngle (smoothYaw, eulerRot.y);
    //     yaw += delta;
    //     smoothYaw += delta;
    //     transform.eulerAngles = Vector3.up * smoothYaw;
    //     // 운동량 변환
    //     velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (velocity));
    //     Physics.SyncTransforms ();
    // }

    public Color GetPixelColorAtRayHit()
    {
        /*
         * 240 ~ 255
         * 200 ~ 255
         * 0   ~ 50
         */
        // 발 아래로 레이 쏘기
        if (reusableTexture == null)
        {
            reusableTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        }
        Ray ray = new Ray(rayCamera.transform.position, -Vector3.up);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // RenderTexture를 Texture2D로 복사
            RenderTexture.active = renderTexture;
            reusableTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            reusableTexture.Apply();
            RenderTexture.active = null;
            
            // 충돌 지점을 스크린 좌표로 변환
            Vector3 screenPoint = rayCamera.WorldToScreenPoint(hit.point);
            int x = Mathf.Clamp((int)screenPoint.x, 0, reusableTexture.width - 1);
            int y = Mathf.Clamp((int)screenPoint.y, 0, reusableTexture.height - 1);
            Debug.Log($"{x}, {y}");

            // 해당 좌표의 색상 가져오기
            Color color = reusableTexture.GetPixel(x, y);
            
            if (color.b >= 0 / 255f && color.b <= 50 / 255f)
            {
                changeSpeed = 3;
            }
            else
            {
                changeSpeed = 1;
            }
            
            if (color.r >= 0 / 255f && color.r <= 50 / 255f)
            {
                changeJump = 2;
            }
            else
            {
                changeJump = 1;
            }
            return color;
        }

        Debug.Log("레이가 아무것도 맞추지 못했습니다.");
        return Color.clear;
    }

    private void PickUpCube()
    {
        if (cube == null)
        {
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.CompareTag("Cube"))
                {
                    cube = hit.collider.gameObject; // cube 변수에 할당
                    cube.transform.SetParent(cubeTransform); // cubeTransform의 자식으로 설정
                    cube.transform.localPosition = Vector3.zero;
                    rb = hit.collider.gameObject.GetComponent<Rigidbody>();
                    rb.useGravity = false;
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }
            }
        }
        else
        {
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            cube.transform.SetParent(null); // 부모-자식 관계 해제
            cube = null; // cube 변수를 null로 초기화
        }
    }
}