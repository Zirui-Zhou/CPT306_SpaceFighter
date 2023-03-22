using UnityEngine;

/// <copyright>
///     <see href="https://stackoverflow.com/a/28449039">“How to improvise singleton for inherited classes?”</see>
///     by <see href="https://stackoverflow.com/users/4523744/miron-alex">Miron Alex</see>
///     is licensed under CC BY-SA 3.0.
/// </copyright>

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {
    public static T Instance { get; private set; }

    protected void Awake() {
        if ( Instance == null) Instance = this as T;
        else if(Instance != this ) Destroy(this);
    }
}