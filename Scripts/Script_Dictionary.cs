using UnityEngine;
using System.Collections;
using System;

public class Script_Dictionary : IComparable<Script_Dictionary> {
    public string name;
    public int value;

    public Script_Dictionary(string newName, int newValue) {
        name = newName;
        value = newValue;
    }

    public int CompareTo(Script_Dictionary other) {
        if (other == null) {
            return 1;
        }

        return value - other.value;
    }
}