using System;
using UnityEngine;
using System.Threading; // CancellationToken 사용을 위해 필요
using Cysharp.Threading.Tasks;
using UnityEngine.UI; // UniTask 필수
public class FadeManager_UniTask : MonoBehaviour
{
    public static FadeManager_UniTask Instance;
    
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float waitTime = 0.5f;

    private void Awake()
    {
        Instance = this;
    }
    
    // 1. 외부에서 호출하는 진입점
    // Coroutine의 StartCoroutine과 대응됨. 
    // 결과를 기다릴 필요가 없다면 UniTaskVoid, 기다려야 한다면 UniTask를 반환
    public void StartFade(Action callback, Image image)
    {
        // Fire and Forget (실행하고 잊음)
        // 이 스크립트가 파괴되면(Destroy) 자동으로 취소되도록 토큰을 연결
        FadeSequence(callback, image, this.GetCancellationTokenOnDestroy()).Forget();
    }
    
    // 2. 전체 페이드 시퀀스 로직
    // IEnumerator -> async UniTask
    private async UniTaskVoid FadeSequence(Action callBack, Image image, CancellationToken token)
    {
      
        // Delay 중 오브젝트가 파괴되면 token이 작동해 여기서 즉시 멈춤 (안전!)
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);

        // 페이드 아웃 (투명 -> 불투명)
        await FadeAsync(true, image, token);

        await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);
        
        // 콜백 실행
        callBack?.Invoke();

        // 페이드 인 (불투명 -> 투명)
        await FadeAsync(false, image, token);

        await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration), cancellationToken: token);
    }
    
    // 3. 실제 페이드 애니메이션 로직
    private async UniTask FadeAsync(bool isOut, Image image, CancellationToken token)
    {
        float timer = 0f;
        Color color = image.color;
        
        float startAlpha = isOut ? 0f : 1f;
        float endAlpha = isOut ? 1f : 0f;

        while (timer < fadeDuration)
        {
            // 루프 도중에도 취소 요청(Destroy)이 오면 멈춰야 함
            if (token.IsCancellationRequested) return;

            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            // Mathf.Lerp 로직 그대로 유지
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            image.color = color;

            // yield return null -> await UniTask.Yield()
            // 다음 프레임까지 대기 (Update 타이밍)
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token);
        }

        // 최종 값 보정
        color.a = endAlpha;
        image.color = color;
    }
    
}
