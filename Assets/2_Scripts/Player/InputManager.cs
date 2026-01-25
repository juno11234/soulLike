using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput _input;
    private PlayerInput.PlayerActions _player;

    public delegate void InputButtonHandler(bool isPressed);

    public delegate void InputQEHandler(bool left);

    public event InputButtonHandler OnSpaceBarInput;
    public event InputButtonHandler OnLMBInput;
    public event InputButtonHandler OnShiftInput;
    public event InputButtonHandler OnMiddleMouseButtonInput;
    public event InputButtonHandler OnRInput;
    public event InputQEHandler OnQorEInput;
    public Vector2 MoveInput { get; private set; }
    public Vector2 CameraInput { get; private set; }

    private void Awake()
    {
        _input = new PlayerInput();
        _player = _input.Player;
    }

    #region 인풋 함수

    private void MovePerformed(InputAction.CallbackContext context)
        => MoveInput = context.ReadValue<Vector2>();

    private void CameraPerformed(InputAction.CallbackContext context)
        => CameraInput = context.ReadValue<Vector2>();

    private void SpacePerformed(InputAction.CallbackContext context)
        => OnSpaceBarInput?.Invoke(true);

    private void SpaceCanceled(InputAction.CallbackContext context)
        => OnSpaceBarInput?.Invoke(false);

    private void LmbPerformed(InputAction.CallbackContext context)
        => OnLMBInput?.Invoke(true);

    private void LmbCanceled(InputAction.CallbackContext context)
        => OnLMBInput?.Invoke(false);

    private void ShiftPerformed(InputAction.CallbackContext context)
        => OnShiftInput?.Invoke(true);

    private void ShiftCanceled(InputAction.CallbackContext context)
        => OnShiftInput?.Invoke(false);

    private void LockOnPerformed(InputAction.CallbackContext context)
        => OnMiddleMouseButtonInput?.Invoke(true);

    private void RPerformed(InputAction.CallbackContext context)
        => OnRInput?.Invoke(true);

    private void QPerformed(InputAction.CallbackContext context)
        => OnQorEInput?.Invoke(true);

    private void EKeyPerformed(InputAction.CallbackContext context)
        => OnQorEInput?.Invoke(false);

    #endregion


    private void OnEnable()
    {
        _input.Enable();

        _player.Move.performed += MovePerformed;
        _player.Camera.performed += CameraPerformed;

        _player.Roll.performed += SpacePerformed;
        _player.Roll.canceled += SpaceCanceled;

        _player.Attack.performed += LmbPerformed;
        _player.Attack.canceled += LmbCanceled;

        _player.Shift.performed += ShiftPerformed;
        _player.Shift.canceled += ShiftCanceled;

        _player.LockOn.performed += LockOnPerformed;

        _player.R.performed += RPerformed;

        _player.Q.performed += QPerformed;
        _player.E.performed += EKeyPerformed;
    }

    private void OnDisable()
    {
        _input.Disable();

        _player.Move.performed -= MovePerformed;
        _player.Camera.performed -= CameraPerformed;

        _player.Roll.performed -= SpacePerformed;
        _player.Roll.canceled -= SpaceCanceled;

        _player.Attack.performed -= LmbPerformed;
        _player.Attack.canceled -= LmbCanceled;

        _player.Shift.performed -= ShiftPerformed;
        _player.Shift.canceled -= ShiftCanceled;

        _player.LockOn.performed -= LockOnPerformed;

        _player.R.performed -= RPerformed;

        _player.Q.performed -= QPerformed;
        _player.E.performed -= EKeyPerformed;
    }
}