using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerLocomotion : MonoBehaviour
{
    private Transform _cameraTransform;
    private InputHandler _inputHandler;
    private Vector3 _moveDirection;

    [HideInInspector] public Transform myTransform;
    [HideInInspector] public AnimatorHandler animatorHandler;
    public new Rigidbody rigidbody;
    public GameObject normalCamera;

    [Header("Stats")] [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        _inputHandler = GetComponent<InputHandler>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        _cameraTransform = Camera.main.transform;
        myTransform = transform;
        animatorHandler.Initialize();
    }

    public void Update()
    {
        float delta = Time.deltaTime;

        _inputHandler.TickInput(delta);
        HandleMovement(delta);
        HandleRollingAndSprinting(delta);
    }


    #region Movement

    private Vector3 _normalVector;
    private Vector3 _targetPosition;

    private void HandleRotation(float delta)
    {
        Vector3 targetDir;
        float moveOverride = _inputHandler.moveAmount;

        targetDir = _cameraTransform.forward * _inputHandler.vertical;
        targetDir += _cameraTransform.right * _inputHandler.horizontal;

        targetDir.Normalize();
        targetDir.y = 0;

        if (targetDir == Vector3.zero)
        {
            targetDir = myTransform.forward;
        }

        float rs = rotationSpeed;
        Quaternion tr = Quaternion.LookRotation(targetDir);
        Quaternion targetRot = Quaternion.Slerp(myTransform.rotation, tr, rs);
        myTransform.rotation = targetRot;
    }

    private void HandleMovement(float delta)
    {
        _moveDirection = _cameraTransform.forward * _inputHandler.vertical;
        _moveDirection += _cameraTransform.right * _inputHandler.horizontal;
        _moveDirection.Normalize();
        _moveDirection.y = 0f;

        float speed = moveSpeed;
        _moveDirection *= speed;

        Vector3 projectedVelocity = Vector3.ProjectOnPlane(_moveDirection, _normalVector);
        rigidbody.linearVelocity = projectedVelocity;

        animatorHandler.UpdateAnimatorValue(_inputHandler.moveAmount, 0);

        if (animatorHandler.canRotate)
        {
            HandleRotation(delta);
        }
    }

    public void HandleRollingAndSprinting(float delta)
    {
        if (animatorHandler.anim.GetBool("isInteracting")) return;

        if (_inputHandler.rollFlag)
        {
            _moveDirection = _cameraTransform.forward * _inputHandler.vertical;
            
            _moveDirection += _cameraTransform.right * _inputHandler.horizontal;

            if (_inputHandler.moveAmount > 0)
            {
                animatorHandler.PlayTargetAnimation("Rolling", true);
                _moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(_moveDirection);
                myTransform.rotation = rollRotation;
            }
            else
            {
                animatorHandler.PlayTargetAnimation("Backstep", true);
            }
        }
    }

    #endregion
}