using System;

// Statuses receive information when deciding how to modify a stat.
public class StatEventArgs<T> : EventArgs where T : IEquatable<T>
{
    public T StatValue;
}

public class BuffableStatEventArgs : EventArgs
{
    public float StatValue;
    public float MultBuff;
    public float AddBuff;
}

// Statuses should not interfere with health changes directly.
public class HealthChangeEventArgs : EventArgs
{
    public HealthChangeEventArgs(float deltaHealth, float currentHealth, float maxHealth)
    {
        DeltaHealth = deltaHealth;
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
    }
    public readonly float DeltaHealth;
    public readonly float CurrentHealth;
    public readonly float MaxHealth;
}