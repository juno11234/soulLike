using System;
using UnityEngine;
using UnityEngine.UI;


public class FighterView : MonoBehaviour
{
    [SerializeField] private FighterStats stats;
    [SerializeField] private Slider hpBar; // 예시: HP 바
    [SerializeField] private Slider staminaBar; // 예시: HP 바
    [SerializeField] private Collider modelCollider;
    [SerializeField] private float widthScaleFactor = 1f; // 너비 조절을 위한 스케일 팩터
    private FighterViewModel ViewModel { get; set; }

    public bool Invincible { get; set; }

    public FighterStats Stats => stats;

    // 이벤트 들
    public event Action<int> OnTakeDamage;
    public event Action OnSecondPhase;
    public event Action OnStaminaZero;
    public event Action OnDied;

    // IFighter 구현
    public Collider mainModelCollider => modelCollider;
    public GameObject GameObject => gameObject;

    private void Awake()
    {
        // 1. Model을 기반으로 ViewModel을 생성합니다.
        ViewModel = new FighterViewModel(stats);

        if (stats.name == "Player")
        {
            RectTransform hpBarRect = hpBar.GetComponent<RectTransform>();
            if (hpBarRect != null)
            {
                float newWidth = stats.MaxHealth * widthScaleFactor;
                hpBarRect.sizeDelta = new Vector2(newWidth, hpBarRect.sizeDelta.y);
            }

            RectTransform staminaBarRect = staminaBar.GetComponent<RectTransform>();
            if (staminaBarRect != null)
            {
                float newWidth = stats.MaxStamina * widthScaleFactor;
                staminaBarRect.sizeDelta = new Vector2(newWidth, staminaBarRect.sizeDelta.y);
            }
        }
        // 최대 체력/스태미나에 비례하여 HP 바와 스태미나 바의 너비를 설정합니다.


        // 2. ViewModel의 데이터 변경을 View에 반영하도록 구독(Subscribe)합니다.
        ViewModel.HealthRatio.Subscribe(UpdateHpBar);
        ViewModel.OnTakeDamage += OnTakeDamageInvoke;
        ViewModel.OnSecondPhase += OnSecondPhaseInvoke;
        ViewModel.OnDied += OnDiedInvoke;

        if (staminaBar != null)
        {
            ViewModel.CurrentStamina.Subscribe(UpdateStaminaBar);
            ViewModel.OnStaminaZero += OnStaminaZeroInvoke;
        }
    }

    private void OnEnable()
    {
        hpBar.gameObject.SetActive(true);
        if (staminaBar != null)
        {
            staminaBar.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (hpBar != null)
        {
            hpBar.gameObject.SetActive(false);
        }

        if (staminaBar != null)
        {
            staminaBar.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        CombatSystem.Instance.RegisterFighter(this);
    }

    private void UpdateHpBar(float newHealthRatio)
    {
        hpBar.value = newHealthRatio;
    }

    private void UpdateStaminaBar(float newStamina)
    {
        staminaBar.value = newStamina / stats.MaxStamina;
    }

    public void Respawn()
    {
        ViewModel.Respawn();
        modelCollider.enabled = true;
        hpBar.gameObject.SetActive(true);
        if (staminaBar != null)
        {
            staminaBar.gameObject.SetActive(true);
        }
    }

    #region 이벤트 발송

    private void OnTakeDamageInvoke(int damage)
    {
        OnTakeDamage?.Invoke(damage);
    }

    private void OnSecondPhaseInvoke()
    {
        OnSecondPhase?.Invoke();
    }

    private void OnStaminaZeroInvoke()
    {
        OnStaminaZero?.Invoke();
    }

    private void OnDiedInvoke()
    {
        OnDied?.Invoke();
        modelCollider.enabled = false;
        hpBar.gameObject.SetActive(false);
        if (staminaBar != null)
        {
            staminaBar.gameObject.SetActive(false);
        }
    }

    #endregion


    // 뷰 모델 함수 실행
    public void TakeDamage(CombatEvent combatEvent)
    {
        if (Invincible) return;

        ViewModel.TakeDamage(combatEvent.Damage);
    }

    public void StaminaChange(float stamina)
    {
        ViewModel.StaminaChange(stamina);
    }

    public void TakeHeal(HealthEvent healEvent)
    {
        ViewModel.TakeHeal(healEvent.HealAmount);
    }
}