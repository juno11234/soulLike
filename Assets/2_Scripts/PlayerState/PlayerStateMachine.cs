using System;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private readonly int _vertical = Animator.StringToHash("Vertical");
    private readonly int _horizontal = Animator.StringToHash("Horizontal");


    [Header("MoveStat")] [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float sprintSpeed = 15f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float rollSpeed = 10f;
    [SerializeField] private float backstepSpeed = 2f;
    [SerializeField] private float groundCheckDistance = 2f;
    [SerializeField] private float groundCheckRadius = 1.3f;

    [Header(("StaminaAmount"))] [SerializeField]
    private float rollStamina;

    [SerializeField] private float backStepStamina;
    [SerializeField] private float attackStamina;
    [SerializeField] private float staminaRegenRate;

    [Header("CamSetting")] [SerializeField]
    private Transform cameraPivot;

    [SerializeField] private float cameraSensitivity = 0.3f;
    [SerializeField] private float cameraMinPitch = -30f;
    [SerializeField] private float cameraMaxPitch = 70f;

    //상태 객체용 변수
    public float WalkSpeed => walkSpeed;
    public float SprintSpeed => sprintSpeed;
    public float RollSpeed => rollSpeed;
    public float BackstepSpeed => backstepSpeed;
    public float MoveAmount => _moveAmount;
    public Animator Animator => _animator;

    public float RollStamina => rollStamina;
    public float BackStepStamina => backStepStamina;
    public float AttackStamina => attackStamina;

    public bool SpaceBarPressed => _spacePressed;
    public bool LmbPressed => _lmbPressed;
    public bool NoStamina => _noStamina;

    //참조들
    private InputManager _inputManager;
    private CharacterController _controller;
    private Camera _mainCam;
    private Animator _animator;
    private FighterView _fighterView;

    //입력용
    private bool _spacePressed;
    private bool _lmbPressed;
    private bool _noStamina;
    public event Action<bool> OnLMBAction;

    //로컬 변수들
    private float _currentSpeed;
    private IState _currentState;
    private float _yaw;
    private float _pitch;
    private float _moveAmount;
    private Vector3 _velocity;
    private float _gravity = -9.81f;
    private bool _sphereHit;
    private LayerMask _groundLayer;

    private void Awake()
    {
        _fighterView = GetComponent<FighterView>();
        _inputManager = GetComponent<InputManager>();
        _controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _mainCam = Camera.main;
        _groundLayer = (1 << 9);
    }

    private void Start()
    {
        _inputManager.OnSpaceBarInput += SpaceBarInput;
        _inputManager.OnLMBInput += LmbInput;

        _fighterView.OnDied += Die;
        _fighterView.OnTakeDamage += TakeDamage;
        _fighterView.OnStaminaZero += StaminaZero;

        ChangeState(new WalkState(this));
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        _currentState?.UpdateLogic();
        //Debug.Log(_currentState);
        HandleStaminaRegeneration();
        
        _velocity.y += _gravity * Time.deltaTime;
        if (_velocity.y < _gravity)
        {
            _velocity.y = _gravity;
        }

        _controller.Move(_velocity * Time.deltaTime);
        HandleCamera();
    }

    public void ChangeState(IState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }

    public void HandleCamera()
    {
        Vector2 input = _inputManager.CameraInput;

        _yaw += input.x * cameraSensitivity;
        _pitch -= input.y * cameraSensitivity;
        _pitch = Mathf.Clamp(_pitch, cameraMinPitch, cameraMaxPitch);

        cameraPivot.rotation = Quaternion.Euler(_pitch, _yaw, 0.0f);
    }

    public void Movement(float speed)
    {
        Vector2 input = _inputManager.MoveInput;
        _moveAmount = Mathf.Clamp01(Mathf.Abs(input.x) + Mathf.Abs(input.y));

        Vector3 forward = _mainCam.transform.forward;
        Vector3 right = _mainCam.transform.right;
        forward.y = 0;
        right.y = 0;

        Vector3 desiredDir = forward.normalized * input.y + right.normalized * input.x;

        if (desiredDir.magnitude >= 0.1f)
        {
            Vector3 moveVec = desiredDir.normalized * (speed * Time.deltaTime);
            _controller.Move(moveVec);

            Quaternion targetRot = Quaternion.LookRotation(desiredDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    public void HandleMove_Ani(float vericalVal, float horizVal, bool isSprinting)
    {
        float v;
        if (vericalVal > 0 && vericalVal < 0.55f)
        {
            v = 0.5f;
        }
        else if (vericalVal > 0.55f)
        {
            v = 1f;
        }
        else if (vericalVal < 0 && vericalVal > -0.55f)
        {
            v = -0.5f;
        }
        else if (vericalVal < 0.55f)
        {
            v = -1f;
        }
        else
        {
            v = 0f;
        }

        if (isSprinting && v > 0)
        {
            v = 2f;
        }

        _animator.SetFloat(_vertical, v, 0.1f, Time.deltaTime);
        _animator.SetFloat(_horizontal, horizVal);
    }

    public bool IsGrounded()
    {
        _sphereHit = Physics.SphereCast
        (transform.position, groundCheckRadius, Vector3.down,
            out _, groundCheckDistance, _groundLayer);

        return _sphereHit && _controller.isGrounded;
    }

    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position;
        Gizmos.color = _sphereHit ? Color.cyan : new Color(0, 1, 1, 0.3f);
        Gizmos.DrawWireSphere(origin + Vector3.down * groundCheckDistance, groundCheckRadius); // 끝점
    }

    public void ForwardMove(float speed)
    {
        float s = Mathf.Lerp(speed, 0, Time.deltaTime * 3f);
        Vector3 moveVec = transform.forward * (s * Time.deltaTime);
        _controller.Move(moveVec);
    }

    public void ActiveInvisible(bool active)
    {
        _fighterView.Invincible = active;
    }

    public void Backstep(float speed)
    {
        float s = Mathf.Lerp(speed, 0, Time.deltaTime * 3f);
        Vector3 moveVec = transform.forward * (s * Time.deltaTime);
        _controller.Move(-moveVec);
    }

    private void SpaceBarInput(bool isPressed)
    {
        _spacePressed = isPressed;
    }

    private void LmbInput(bool isPressed)
    {
        OnLMBAction?.Invoke(isPressed);
    }

    public void PlayTargetAniClip(int hash, float transition)
    {
        _animator.CrossFade(hash, transition);
    }

    private void Die()
    {
        ChangeState(new DieState(this));
    }

    private void HandleStaminaRegeneration()
    {
        if (_currentState is WalkState)
        {
            if (NoStamina == false)
            {
                StaminaChange((staminaRegenRate * Time.deltaTime));
            }
        }
    }

    public void StaminaChange(float stamina)
    {
        _fighterView.StaminaChange(stamina);
    }

    public void StaminaZero()
    {
        _noStamina = true;
    }

    public void StaminaZeroCancel()
    {
        _noStamina = false;
    }

    private void TakeDamage(int damage)
    {
        if (_currentState is HitState) return;

        ChangeState(new HitState(this));
    }
}