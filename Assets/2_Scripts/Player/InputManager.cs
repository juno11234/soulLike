using System;
using RPGCharacterAnims.Actions;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerInput _input;
    private PlayerInput.PlayerActions _player;

    public delegate void InputButtonHandler(bool isPressed);

    public event InputButtonHandler OnSpaceBarInput;
    public event InputButtonHandler OnLMBInput;
    public event InputButtonHandler OnShiftInput;
    public Vector2 MoveInput { get; private set; }
    public Vector2 CameraInput { get; private set; }

    private void Awake()
    {
        _input = new PlayerInput();
        _player = _input.Player;

        _player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        _player.Camera.performed += ctx => CameraInput = ctx.ReadValue<Vector2>();

        _player.Roll.performed += ctx => OnSpaceBarInput?.Invoke(true);
        _player.Roll.canceled += ctx => OnSpaceBarInput?.Invoke(false);

        _player.Attack.performed += ctx => OnLMBInput?.Invoke(true);
        _player.Attack.canceled += ctx => OnLMBInput?.Invoke(false);

        _player.Shift.performed += ctx => OnShiftInput?.Invoke(true);
        _player.Shift.canceled += ctx => OnShiftInput?.Invoke(false);
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }
}