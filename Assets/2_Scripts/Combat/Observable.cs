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
        listener?.Invoke(_value);
    }

    public void Unsubscribe(Action<T> listener)
    {
        OnValueChanged -= listener;
    }
}
