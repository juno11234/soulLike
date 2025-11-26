using System;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Transform player;
    
    [Header("chaserSetting")] [SerializeField] private float smoothSpeed = 10f;
    [SerializeField] private Vector3 offset;

    [Header("CamSetting")] [SerializeField]
    private Transform cameraPivot;

    [SerializeField] private float cameraSensitivity = 0.3f;
    [SerializeField] private float cameraMinPitch = -30f;
    [SerializeField] private float cameraMaxPitch = 30f;

    [Header("setting")] [SerializeField] private Transform cameraTransform;
    [SerializeField] private float searchRadius = 10f;
    [SerializeField] private LayerMask targetLayer;

    [Header("debug")] [SerializeField] private LockOnTarget currentTarget;
    [SerializeField] private bool isLockedOn = false;
    
    [Header("collisionSetting")]
    public LayerMask collisionLayer; 
    public float collideOffset = 0.2f; 
    public float cameraRadius = 0.3f; 
    public float moveSpeed = 15f; 

    private float _yaw;
    private float _pitch;


    private void Start()
    {
        inputManager.OnMiddleMouseButtonInput += LockOn;
    }

    void Update()
    {
        HandleCamera();
        LockOnCamControl();
    }

    private void LateUpdate()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    private void HandleCamera()
    {
        if (isLockedOn) return;
        Vector2 input = inputManager.CameraInput;

        _yaw += input.x * cameraSensitivity;
        _pitch -= input.y * cameraSensitivity;
        _pitch = Mathf.Clamp(_pitch, cameraMinPitch, cameraMaxPitch);

        cameraPivot.rotation = Quaternion.Euler(_pitch, _yaw, 0.0f);
    }

    private void LockOn(bool isPressed)
    {
        if (isPressed == false) return;
        if (isLockedOn)
        {
            UnlockOn();
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius, targetLayer);

        LockOnTarget nearestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            LockOnTarget target = collider.GetComponentInChildren<LockOnTarget>();
            if (target != null)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestTarget = target;
                }
            }
        }

        if (nearestTarget != null)
        {
            currentTarget = nearestTarget;
            isLockedOn = true;
        }
    }

    private void UnlockOn()
    {
        isLockedOn = false;
        currentTarget = null;
    }

    private void LockOnCamControl()
    {
        if (isLockedOn == false || currentTarget == null) return;

        if (currentTarget.gameObject.activeInHierarchy == false)
        {
            UnlockOn();
            return;
        }

        Vector3 dir = currentTarget.transform.position - cameraPivot.position;
        if (dir == Vector3.zero) return;

        // 부드럽게 회전 (Slerp)
        Quaternion targetRotation = Quaternion.LookRotation(dir);
        targetRotation.x = 0.0f;
        targetRotation.z = 0.0f;
        cameraTransform.rotation = targetRotation;
    }
}