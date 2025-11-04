using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;

    public bool b_Input;
    public bool rollFlag;
    public bool isInteracting;

    private PlayerControls _inputActions;
    private CameraHandler _cameraHandler;

    private Vector2 _movementInput;
    private Vector2 _cameraInput;

    private void Awake()
    {
        _cameraHandler = CameraHandler.Instance;
    }

    private void FixedUpdate()
    {
        float delta = Time.deltaTime;

        if (_cameraHandler != null)
        {
            _cameraHandler.FollowTarget(delta);
            _cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
        }
    }

    private void OnEnable()
    {
        if (_inputActions == null)
        {
            _inputActions = new PlayerControls();
            _inputActions.PlayerMovement.Movement.performed +=
                ctx => _movementInput = ctx.ReadValue<Vector2>();

            _inputActions.PlayerMovement.Camera.performed += i => _cameraInput = i.ReadValue<Vector2>();
        }

        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    public void TickInput(float delta)
    {
        MoveInput(delta);
        HandleRollInput(delta);
    }

    private void MoveInput(float delta)
    {
        horizontal = _movementInput.x;
        vertical = _movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = _cameraInput.x;
        mouseY = _cameraInput.y;
    }

    private void HandleRollInput(float delta)
    {
        b_Input = _inputActions.PlayerAction.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
        if (b_Input)
        {
            rollFlag = true;
        }
    }
}