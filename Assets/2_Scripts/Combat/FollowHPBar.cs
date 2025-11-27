using UnityEngine;

/// <summary>
/// 월드 공간 UI(예: HP바)가 항상 메인 카메라를 바라보도록 만드는 빌보드 스크립트.
/// 이 스크립트를 HP바를 담고 있는 Canvas에 적용해야 합니다.
/// </summary>
public class FollowHPBar : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // 성능을 위해 메인 카메라를 캐싱합니다.
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        // 카메라의 회전값을 그대로 적용하여 UI가 항상 카메라를 정면으로 바라보게 합니다.
        // LookAt보다 안정적이며, UI가 뒤집히는 현상을 방지합니다.
        
            transform.rotation = mainCamera.transform.rotation;
        
    }
}