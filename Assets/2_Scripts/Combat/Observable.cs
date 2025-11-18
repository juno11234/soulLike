using System;

public class Observable<T>
{
    private T _value;
    public event Action<T> OnValueChanged;

    public T Value
    {
        get => _value;
        set
        {
            if (Equals(_value, value)) return;
            _value = value;
            OnValueChanged?.Invoke(_value);
        }
    }

    public Observable(T initialValue)
    {
        _value = initialValue;
    }

    public void Subscribe(Action<T> listener)
    {
        OnValueChanged += listener;
        // 구독 시 현재 값으로 즉시 한번 호출
        listener?.Invoke(_value);
    }

    public void Unsubscribe(Action<T> listener)
    {
        OnValueChanged -= listener;
    }
}
