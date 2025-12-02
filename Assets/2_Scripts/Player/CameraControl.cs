using System;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public bool IsLockedOn => isLockedOn;
    public LockOnTarget CurrentTarget => currentTarget;

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Transform player;

    [Header("chaserSetting")] [SerializeField]
    private float smoothSpeed = 10f;

    [SerializeField] private Vector3 offset;
    [SerializeField] private float lockOnHeightOffset = 1f;

    [Header("CamSetting")] [SerializeField]
    private Transform cameraPivot;

    [SerializeField] private float cameraSensitivity = 0.3f;
    [SerializeField] private float cameraMinPitch = -30f;
    [SerializeField] private float cameraMaxPitch = 30f;

    [Header("setting")] [SerializeField] private Transform cameraTransform;
    [SerializeField] private float searchRadius = 10f;
    [SerializeField] private LayerMask targetLayer;

    [Header("LockOn UI")] [SerializeField] private RectTransform lockOnIndicator;


    [Header("debug")] [SerializeField] private LockOnTarget currentTarget;
    [SerializeField] private bool isLockedOn = false;

    [Header("collisionSetting")] public LayerMask collisionLayer;
    public float collideOffset = 0.2f;
    public float cameraRadius = 0.3f;
    public float moveSpeed = 15f;

    private Vector3 _cameraVelocity;
    private float _defaultDistance;

    private float _yaw;
    private float _pitch;


    private void Start()
    {
        inputManager.OnMiddleMouseButtonInput += LockOn;
        inputManager.OnQorEInput += HandleTargetSwitching;
        _defaultDistance = Vector3.Distance(cameraPivot.position,
            cameraTransform.position);
        lockOnIndicator.gameObject.SetActive(false);
    }

    void Update()
    {
        HandleCamera();
    }

    private void LateUpdate()
    {
        FollowPlayer();
        LockOnCamControl();
        CameraCollision();
    }

    private void HandleTargetSwitching(bool isQForLeft)
    {
        if (isLockedOn == false) return;


        // Q 키 입력 확인 (왼쪽으로 전환)
        if (isQForLeft)
        {
            FindAndSwitchTarget(true); // false for left
        }
        // E 키 입력 확인 (오른쪽으로 전환)
        else if (isQForLeft == false)
        {
            FindAndSwitchTarget(false); // true for right
        }
    }

    private void FindAndSwitchTarget(bool isLeft)
    {
        // 1. 주변의 모든 잠재적 타겟을 찾습니다.
        Collider[] colliders = Physics.OverlapSphere(player.position, searchRadius, targetLayer);
        var potentialTargets = new System.Collections.Generic.List<LockOnTarget>();
        foreach (var coll in colliders)
        {
            var target = coll.GetComponentInChildren<LockOnTarget>();
            // 현재 타겟이 아니고, 활성화된 타겟만 리스트에 추가합니다.
            if (target != null && target != currentTarget && target.gameObject.activeInHierarchy)
            {
                potentialTargets.Add(target);
            }
        }

        if (potentialTargets.Count == 0) return; // 바꿀 타겟이 없음

        // 2. 가장 적합한 타겟을 찾습니다.
        LockOnTarget bestTarget = null;
        float bestScore = Mathf.Infinity;

        Vector3 currentScreenPos = Camera.main.WorldToScreenPoint(currentTarget.transform.position);

        foreach (var pTarget in potentialTargets)
        {
            Vector3 potentialScreenPos = Camera.main.WorldToScreenPoint(pTarget.transform.position);

            if (potentialScreenPos.z < 0) continue; // 카메라 뒤에 있는 타겟은 제외

            float horizontalDiff = potentialScreenPos.x - currentScreenPos.x;

            // 마우스 방향과 타겟의 상대적 위치가 일치하는지 확인
            bool isInDirection = (isLeft == false && horizontalDiff > 0) || (isLeft && horizontalDiff < 0);

            if (isInDirection)
            {
                // 화면상에서 수평 거리가 가장 가까운 타겟을 최고 점수로 선정
                float score = Mathf.Abs(horizontalDiff);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestTarget = pTarget;
                }
            }
        }

        // 3. 적합한 타겟을 찾았다면 교체합니다.
        if (bestTarget != null)
        {
            currentTarget = bestTarget;
        }
    }


    private void FollowPlayer()
    {
        Vector3 desiredPosition = player.position + offset;
        if (isLockedOn)
        {
            desiredPosition.y += lockOnHeightOffset;
        }

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

    private void CameraCollision()
    {
        float targetDistance = _defaultDistance;
        Vector3 direction = -cameraPivot.forward;

        if (Physics.SphereCast(cameraPivot.position, cameraRadius, direction,
                out RaycastHit hit, _defaultDistance, collisionLayer))
        {
            targetDistance = hit.distance - collideOffset;
        }

        Vector3 targetPosition = cameraPivot.position + direction *
            targetDistance;

        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, targetPosition,
            ref _cameraVelocity, 1f / moveSpeed);
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

        foreach (Collider coll in colliders)
        {
            LockOnTarget target = coll.GetComponentInChildren<LockOnTarget>();
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


            lockOnIndicator.gameObject.SetActive(true);
        }
    }

    private void UnlockOn()
    {
        isLockedOn = false;
        currentTarget = null;

        lockOnIndicator.gameObject.SetActive(false);
    }

    private void LockOnCamControl()
    {
        if (isLockedOn == false) return;

        if (currentTarget.gameObject.activeInHierarchy == false)
        {
            UnlockOn();
            return;
        }

        // 타겟의 월드 좌표를 스크린 좌표로 변환
        Vector3 screenPos = Camera.main.WorldToScreenPoint(currentTarget.transform.position);
        lockOnIndicator.position = screenPos;

        // 타겟이 화면 뒤(카메라 뒤)에 있으면 UI를 숨김
        lockOnIndicator.gameObject.SetActive(screenPos.z > 0);


        Vector3 dir = currentTarget.transform.position - cameraPivot.position;
        if (dir == Vector3.zero) return;

        // 부드럽게 회전 (Slerp)
        Quaternion targetRotation = Quaternion.LookRotation(dir);
        targetRotation.x = 0.0f;
        targetRotation.z = 0.0f;

        cameraPivot.rotation = targetRotation;
    }
}