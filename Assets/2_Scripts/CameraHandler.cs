using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public static CameraHandler Instance;

    public Transform targetTransform;
    public Transform cameraTransform;
    public Transform cameraPivotTransform;

    private Transform _myTransform;
    private Vector3 _cameraPosition;
    private LayerMask _ignoreLayers;
    private Vector3 _cameraFollwVelocity = Vector3.zero;

    public float lookSpeed = 0.1f;
    public float followSpeed = 0.1f;
    public float pivotSpeed = 0.03f;

    private float _targetPosition;
    private float _defaultPosition;
    private float _lookAngle;
    private float _pivotAngle;
    public float minmumPivot = -35f;
    public float maxmumPivot = 35f;

    public float cameraSphereRadius;
    public float cameraCollisionOffset;
    public float minimumCollisionOffset;

    private void Awake()
    {
        Instance = this;
        _myTransform = transform;
        _defaultPosition = cameraTransform.localPosition.z;
        _ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
    }

    public void FollowTarget(float delta)
    {
        Vector3 targetPosition = Vector3.SmoothDamp
            (_myTransform.position, targetTransform.position, ref _cameraFollwVelocity, delta / followSpeed);
        _myTransform.position = targetPosition;

        HandleCameraCollisions(delta);
    }

    public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
    {
        _lookAngle += (mouseXInput * lookSpeed) / delta;
        _pivotAngle -= (mouseYInput * pivotSpeed) / delta;
        _pivotAngle = Mathf.Clamp(_pivotAngle, minmumPivot, maxmumPivot);

        Vector3 rotation = Vector3.zero;
        rotation.y = _lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        _myTransform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = _pivotAngle;

        targetRotation = Quaternion.Euler(rotation);
        cameraPivotTransform.localRotation = targetRotation;
    }

    private void HandleCameraCollisions(float delta)
    {
        _targetPosition = _defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit,
                Mathf.Abs(_targetPosition), _ignoreLayers))
        {
            float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
            _targetPosition = -(dis - cameraCollisionOffset);
        }

        if (Mathf.Abs(_targetPosition) < minimumCollisionOffset)
        {
            _targetPosition = -minimumCollisionOffset;
        }

        _cameraPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, _targetPosition, delta / 0.2f);
        cameraTransform.localPosition = _cameraPosition;
    }
}