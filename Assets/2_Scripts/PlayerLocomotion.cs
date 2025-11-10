using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerLocomotion : MonoBehaviour
{
    private PlayerManager _playerManager;
    private Transform _cameraTransform;
    private InputHandler _inputHandler;
    public Vector3 moveDirection;

    [HideInInspector] public Transform myTransform;
    [HideInInspector] public AnimatorHandler animatorHandler;
    public new Rigidbody rigidbody;
    public GameObject normalCamera;

    [Header("Movement Stats")] [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float fallingSpeed = 45;

    [SerializeField] private float groundDetectionRayStartPoint = 0.5f;
    [SerializeField] private float minimumDistanceNeedToBeginFall = 1f;
    [SerializeField] private float groundDetectionRayDistance = 0.2f;
    private LayerMask _ignoreLayerGroundCheck;
    public float inAirTimer;

    private void Start()
    {
        _playerManager = GetComponent<PlayerManager>();
        rigidbody = GetComponent<Rigidbody>();
        _inputHandler = GetComponent<InputHandler>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        _cameraTransform = Camera.main.transform;
        myTransform = transform;
        animatorHandler.Initialize();
        _playerManager.isGrounded = true;
        _ignoreLayerGroundCheck = ~(1 << 8 | 1 << 11);
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

    public void HandleMovement(float delta)
    {
        if (_inputHandler.rollFlag) return;
        if (_playerManager.isInteracting) return;
        moveDirection = _cameraTransform.forward * _inputHandler.vertical;
        moveDirection += _cameraTransform.right * _inputHandler.horizontal;
        moveDirection.Normalize();
        moveDirection.y = 0f;

        float speed = moveSpeed;

        if (_inputHandler.sprintFlag)
        {
            speed = sprintSpeed;
            _playerManager.isSprinting = true;
            moveDirection *= speed;
        }
        else
        {
            moveDirection *= speed;
        }

        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, _normalVector);
        rigidbody.linearVelocity = projectedVelocity;

        animatorHandler.UpdateAnimatorValue(_inputHandler.moveAmount, 0, _playerManager.isSprinting);

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
            moveDirection = _cameraTransform.forward * _inputHandler.vertical;

            moveDirection += _cameraTransform.right * _inputHandler.horizontal;

            if (_inputHandler.moveAmount > 0)
            {
                animatorHandler.PlayTargetAnimation("Rolling", true);
                moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                myTransform.rotation = rollRotation;
            }
            else
            {
                animatorHandler.PlayTargetAnimation("Backstep", true);
            }
        }
    }

    public void HandleFalling(float delta, Vector3 moveDirection)
    {
        _playerManager.isGrounded = false;
        RaycastHit hit;
        Vector3 origin = myTransform.position;
        origin.y += groundDetectionRayStartPoint;

        if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
        {
            moveDirection = Vector3.zero;
        }

        if (_playerManager.isInAir)
        {
            rigidbody.AddForce(-Vector3.up * fallingSpeed);
            rigidbody.AddForce(moveDirection * fallingSpeed / 5f);
        }

        Vector3 dir = moveDirection;
        dir.Normalize();
        origin = origin + dir * groundDetectionRayDistance;

        _targetPosition = myTransform.position;
        int layerMask = _ignoreLayerGroundCheck;
        Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeedToBeginFall, Color.red, 0.1f, false);
        if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeedToBeginFall, layerMask))
        {
            _normalVector = hit.normal;
            Vector3 tp = hit.point;
            _playerManager.isGrounded = true;
            _targetPosition.y = tp.y;

            if (_playerManager.isInAir)
            {
                if (inAirTimer > 0.5f)
                {
                    Debug.Log(inAirTimer);
                    animatorHandler.PlayTargetAnimation("Land", true);
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Empty", false);
                    inAirTimer = 0;
                }

                _playerManager.isInAir = false;
            }
        }
        else
        {
            if (_playerManager.isGrounded)
            {
                _playerManager.isGrounded = false;
            }

            if (_playerManager.isInAir == false)
            {
                if (_playerManager.isInteracting == false)
                {
                    animatorHandler.PlayTargetAnimation("Falling", true);
                }

                Vector3 vel = rigidbody.linearVelocity;
                vel.Normalize();
                rigidbody.linearVelocity = vel;
            }
        }
    }

    #endregion
}