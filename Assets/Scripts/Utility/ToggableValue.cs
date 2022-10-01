using UnityEngine;

[System.Serializable]
public struct ToggableValue<T>
{
    [SerializeField] bool enabled;
    [SerializeField] T value;

    public bool Enabled => enabled;
    public T Value => value;

    public ToggableValue(T value)
    {
        this.value = value;
        enabled = false;
    }

    public ToggableValue(bool enabled, T value)
    {
        this.value = value;
        this.enabled = enabled;
    }

    public static implicit operator T(ToggableValue<T> v) => v.Value;
    public static explicit operator ToggableValue<T>(T t) => new ToggableValue<T>(t);
}