using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private CameraHandler _cameraHandler;
    private InputHandler _inputHandler;
    private PlayerLocomotion _playerLocomotion;
    private Animator _animator;
    public bool isInteracting;

    [Header("PlayerFlags")] public bool isSprinting;
    public bool isInAir;
    public bool isGrounded;

    private void Awake()
    {
        _cameraHandler = CameraHandler.Instance;
    }

    void Start()
    {
        _inputHandler = GetComponent<InputHandler>();
        _animator = GetComponentInChildren<Animator>();
        _playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;
        isInteracting = _animator.GetBool("isInteracting");
        _inputHandler.TickInput(delta);
        _playerLocomotion.HandleMovement(delta);
        _playerLocomotion.HandleRollingAndSprinting(delta);
        _playerLocomotion.HandleFalling(delta, _playerLocomotion.moveDirection);
    }

    private void FixedUpdate()
    {
        float delta = Time.deltaTime;

        if (_cameraHandler != null)
        {
            _cameraHandler.FollowTarget(delta);
            _cameraHandler.HandleCameraRotation(delta, _inputHandler.mouseX, _inputHandler.mouseY);
        }
    }

    private void LateUpdate()
    {
        _inputHandler.rollFlag = false;
        _inputHandler.sprintFlag = false;
        isSprinting = _inputHandler.b_Input;

        if (isInAir)
        {
            _playerLocomotion.inAirTimer = _playerLocomotion.inAirTimer + Time.deltaTime;
        }
    }
}