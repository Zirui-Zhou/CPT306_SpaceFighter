using System;
using System.Collections.Generic;
using UnityEngine;

/// <copyright>
///     <see href="https://stackoverflow.com/a/73599517">“Is there a way to easily bind data to UI in runtime”</see>
///     by <see href="https://stackoverflow.com/users/12806072/sisus">Sisus</see>
///     is licensed under CC BY-SA 4.0.
/// </copyright>

public interface IProperty<out T> : IProperty
{
    new event Action<T> ValueChanged;
    new T Value { get; }
}

public interface IProperty
{
    event Action<object> ValueChanged;
    object Value { get; }
}

[Serializable]
public class Property<T> : IProperty<T> {
    public event Action<T> ValueChanged;
    
    event Action<object> IProperty.ValueChanged {
        add => valueChanged += value;
        remove => valueChanged -= value;
    }
    
    [SerializeField]
    private T value;
    
    public T Value {
        get => value;
        
        set {
            if(EqualityComparer<T>.Default.Equals(this.value, value)) {
                return;
            }
            
            this.value = value;
            
            ValueChanged?.Invoke(value);
            valueChanged?.Invoke(value);
        }
    }
    
    object IProperty.Value => value;

    private Action<object> valueChanged;
    
    public Property(T value) => this.value = value;
    
    public static explicit operator Property<T>(T value) => new Property<T>(value);
    public static implicit operator T(Property<T> binding) => binding.value;
}