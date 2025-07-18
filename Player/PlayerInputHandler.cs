using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputHandler : MonoBehaviour, ICharacterInput
{

    public Camera playerCamera;
    public Transform cameraPivot;
    [SerializeField] private float minY = -90f;
    [SerializeField] private float maxY = 80f;

    private float yaw = 0f;
    private float pitch = 0f;

    private CharacterController controller;

    private InputSettings input => SettingsManager.Instance.inputSettings;

    [SerializeField] float movespeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float rotationSpeed = 10f;

    [SerializeField] private float mouseSensitivityX = 5f;
    [SerializeField] private float mouseSensitivityY = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    private float verticalVelocity;
    private bool isGrounded;




    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if(SettingsManager.Instance == null)
        {
            Debug.LogError("SettingsManager not found in the scene. Please add it to the scene.");
        }
        else if(SettingsManager.Instance.inputSettings == null)
        {
            Debug.LogError("InputSettings not found in SettingsManager. Please assign it in the inspector.");
        }

        mouseSensitivityX = input.mouseSensitivityX;
        mouseSensitivityY = input.mouseSensitivityY;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void Update()
    {
        MovePlayer();
        RotateCamera();
        //RotateTowardsMouse();
    }

    public Vector3 GetMovement()
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(input.moveForward)) move += Vector3.forward;
        if (Input.GetKey(input.moveBackward)) move += Vector3.back;
        if (Input.GetKey(input.moveLeft)) move += Vector3.left;
        if (Input.GetKey(input.moveRight)) move += Vector3.right;

        move = move.normalized;

        Vector3 cameraForward = cameraPivot.forward;
        Vector3 cameraRight = cameraPivot.right;

        cameraForward.y = 0f; // Ignore vertical movement
        cameraRight.y = 0f; // Ignore vertical movement

        Vector3 moveDirection = cameraForward.normalized * move.z + cameraRight.normalized * move.x;
        return moveDirection.normalized;
    }
    private void MovePlayer()
    {
        Vector3 move = GetMovement();
        float speed = movespeed * (GetRun() ? sprintMultiplier : 1f); // Sprinting logic
        //controller.SimpleMove(move * speed);
        isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Stick to ground
        }

        if (GetJump() && isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * speed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);


        /*        if(move != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(move);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
        */
        if (move != Vector3.zero)
        {
            // Rotate only based on move direction (which is already camera-relative)
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    private void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY;
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minY, maxY);
        cameraPivot.localRotation = Quaternion.Euler(pitch, yaw, 0f);

/*        Vector3 move = GetMovement();
        if (move.sqrMagnitude > 0.01f)
        {
            Vector3 direction = Quaternion.Euler(0, yaw, 0) * move;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }*/
    }

    public Vector3 GetLookDirection() => playerCamera.transform.forward;

    public bool GetPrimary() => Input.GetKeyDown(input.primaryFire);
    public bool GetPrimaryHeld() => Input.GetKey(input.primaryFire);
    public bool GetSecondary() => Input.GetKeyDown(input.secondaryFire);
    public bool GetSecondaryHeld() => Input.GetKey(input.secondaryFire);
    public bool GetSkill(int index)
    {
        return index switch
        {
            0 => Input.GetKeyDown(input.ability1),
            1 => Input.GetKeyDown(input.ability2),
            2 => Input.GetKeyDown(input.ability3),
            3 => Input.GetKeyDown(input.ability4),
            _ => false,
        };
    }
    public bool GetJump() => Input.GetKeyDown(input.jump);
    public bool GetRun() => Input.GetKey(input.run);



}