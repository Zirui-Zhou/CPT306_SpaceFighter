using System;
using System.Collections.Generic;

/// <copyright>
///     <see href="https://stackoverflow.com/a/1739058">“Initializing jagged arrays”</see>
///     by <see href="https://stackoverflow.com/users/76217/dtb">dtb</see>
///     is licensed under CC BY-SA 2.5.
/// </copyright>

public static class JaggedArray {
    public static T CreateJaggedArray<T>(params int[] lengths) {
        return (T)InitializeJaggedArray(typeof(T).GetElementType(), 0, lengths);
    }
    
    private static object InitializeJaggedArray(Type type, int index, IReadOnlyList<int> lengths) {
        var array = Array.CreateInstance(type, lengths[index]);
        var elementType = type.GetElementType();
    
        if (elementType == null) return array;
        for (var i = 0; i < lengths[index]; i++) {
            array.SetValue(
                InitializeJaggedArray(elementType, index + 1, lengths), i);
        }
    
        return array;
    }
}
