using System;
using UnityEngine;

public enum MoveType
{
    None,
    Idle,
    Walk,
    Sprint
}

public class PlayerControll : MonoBehaviour
{
    [Header("이동")] [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float sprintSpeed = 15f;
    [SerializeField] private float rotationSpeed = 10f;


    [Header("카메라")] [SerializeField] private Transform cameraPivot;
    [SerializeField] private float cameraSensitivity = 0.3f;
    [SerializeField] private float cameraMinPitch = -30f;
    [SerializeField] private float cameraMaxPitch = 70f;

    private readonly int _vertical = Animator.StringToHash("Vertical");
    private readonly int _horizontal = Animator.StringToHash("Horizontal");
    private readonly int _roll = Animator.StringToHash("Rolling");

    //참조들
    private InputManager _inputManager;
    private CharacterController _controller;
    private Camera _mainCam;
    private Animator _animator;

    private float _yaw;
    private float _pitch;
    private float _currentSpeed;
    private MoveType _currentMoveType;
    private float _rollTimer;
    private bool _spaceBarPressed;

    private void Start()
    {
        _inputManager = GetComponent<InputManager>();
        _controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _mainCam = Camera.main;

        _inputManager.OnSpaceBarInput += HandleRoll;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraPivot != null)
        {
            _yaw = cameraPivot.eulerAngles.y;
            _pitch = cameraPivot.eulerAngles.x;
        }

        _currentMoveType = MoveType.Idle;
        _currentSpeed = walkSpeed;
    }

    private float _spacePressTimer;

    private void Update()
    {
        HandleMovement();
        HandleCamera();
        
        if (_spaceBarPressed)
        {
            _spacePressTimer += Time.deltaTime;
        }
    }

    #region 입력 컨트롤

    private void HandleMovement()
    {
        Vector2 input = _inputManager.MoveInput;

        Vector3 forward = _mainCam.transform.forward;
        Vector3 right = _mainCam.transform.right;
        forward.y = 0;
        right.y = 0;

        Vector3 desiredDir = forward.normalized * input.y + right.normalized * input.x;

        if (desiredDir.magnitude >= 0.1f)
        {
            if (_spaceBarPressed && _spacePressTimer > 0.25f)
            {
                _currentMoveType = MoveType.Sprint;
            }
            else
            {
                _currentMoveType = MoveType.Walk;
            }

            Vector3 moveVec = desiredDir.normalized * (_currentSpeed * Time.deltaTime);
            _controller.Move(moveVec);

            Quaternion targetRot = Quaternion.LookRotation(desiredDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
        else
        {
            _currentMoveType = MoveType.Idle;
        }

        HandleMove_Ani();
    }

    private void HandleCamera()
    {
        Vector2 input = _inputManager.CameraInput;

        _yaw += input.x * cameraSensitivity;
        _pitch -= input.y * cameraSensitivity;
        _pitch = Mathf.Clamp(_pitch, cameraMinPitch, cameraMaxPitch);

        cameraPivot.rotation = Quaternion.Euler(_pitch, _yaw, 0.0f);
    }

    private void HandleRoll(bool isPressed)
    {
        if (isPressed)
        {
            _spacePressTimer = 0;
            _spaceBarPressed = true;
        }

        if (isPressed == false)
        {
            _spaceBarPressed = false;

            if (_spacePressTimer < 0.25f)
            {
                HandleRoll_Ani();
            }
        }
    }

    #endregion

    #region 애니메이션 컨트롤

    private void HandleMove_Ani()
    {
        float v = 0f;

        switch (_currentMoveType)
        {
            case MoveType.None:
                v = 0f;
                break;
            case MoveType.Idle:
                v = 0f;
                break;
            case MoveType.Walk:
                v = 1f;
                _currentSpeed = walkSpeed;
                break;
            case MoveType.Sprint:
                v = 2f;
                _currentSpeed = sprintSpeed;
                break;
        }

        _animator.SetFloat(_vertical, v, 0.1f, Time.deltaTime);
        _animator.SetFloat(_horizontal, 0f);
    }

    private void HandleRoll_Ani()
    {
        PlayTargetAniClip(_roll);
    }

    private void PlayTargetAniClip(int hash)
    {
        _animator.CrossFade(hash, 0.2f);
    }

    #endregion
}