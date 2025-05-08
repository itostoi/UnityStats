using System;
using UnityEngine;

public class Damage
{
    public Damage(GameObject source)
    { Source = source; }
    public readonly GameObject Source;
    public float Value { get; set; }
    public bool IsCrit = false;
}