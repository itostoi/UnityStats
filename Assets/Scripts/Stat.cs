using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


// RMK: At the moment, all statuses subscribing to the same event must commute
// For example, for a buffable stat, multiple adds and muliplies are ok, but we cannot
// do something like set x to y.

// We can specify execution order with a custom event implementation, but this
// is a substantial amount of overhead, since we'll need a data structure +
// infra for sorting statuses.

// For example, if we want a status for invulnerability, we can: 
// 1) add a stat for isinvulnerable and make damage calc dependent on it
// 2) add a second event which fires after normal damage calc and sets damage to 0
// 3) implement a sorting order for events which process damage.
// If we need to hardcode more than three ordered triggers, we'll implement some kind of sorted event
abstract public class BaseStat<T, E> where E : EventArgs 
{
    public BaseStat(T baseValue) {
        this.baseValue = baseValue; 
        calcValue = baseValue;
    }

    public event EventHandler ValueChange;
    public event EventHandler<E> TriggerEffects;

    // Effects should (almost) never change this (since there's no infra to add/remove statuses correctly)
    protected T baseValue;
    public T BaseValue {
        get => baseValue;
        set { 
            Refresh();
            baseValue = value;
        }
    }

    protected virtual void OnValueChange()
    {
        ValueChange?.Invoke(this, new EventArgs());
    }

    protected virtual void OnTriggerEffects(E effectEventArgs)
    {
        TriggerEffects?.Invoke(this, effectEventArgs);
    }

    // "caches" the calcs which determine the public stat
    protected T calcValue;
    public T Value { get => calcValue; }

    // Refresh recalculates calcValue (Value) when upstream stats change
    public void Refresh(System.Object o, EventArgs e)
    { 
        Refresh();
    }
    public abstract void Refresh();
    protected abstract T Calc(E statValue);
}

public class FloatStat : BaseStat<float, StatEventArgs<float>>
{
    public FloatStat(float initialValue) : base(initialValue) { }
    public override void Refresh()
    {
        StatEventArgs<float> StatEventArgs = new StatEventArgs<float>{StatValue = baseValue};
        // UnityEngine.Debug.Log("Invoking {0} statuses, ", TriggerStatuses?.GetInvocationList().Length);
        OnTriggerEffects(StatEventArgs);
        // Explicit calc using upstream stats
        float newCalcValue = Calc(StatEventArgs);

        if (!newCalcValue.Equals(calcValue))
        {
            calcValue = newCalcValue;
            // Let dependents know
            OnValueChange();
        }
    }
    protected override float Calc(StatEventArgs<float> StatEventArgs)
    {
        return StatEventArgs.StatValue;
    }
}

// Statuses can modify a buffable stat more elegantly.
public class BuffableStat : BaseStat<float, BuffableStatEventArgs>
{
    public BuffableStat(float initialValue) : base(initialValue) {}

    private float multBuff = 1f;
    private float addBuff = 0f;
    public float MultBuff {get => multBuff;}
    public float AddBuff {get => addBuff;}

    public override void Refresh()
    {
        // glue additional stuff onto StatEventArgs.
        BuffableStatEventArgs StatEventArgs = new BuffableStatEventArgs{StatValue = baseValue, MultBuff = 1f, AddBuff = 0f};
        OnTriggerEffects(StatEventArgs);
        multBuff = StatEventArgs.MultBuff;
        addBuff = StatEventArgs.AddBuff;

        float newCalcValue = Calc(StatEventArgs);

        if (!newCalcValue.Equals(calcValue))
        {
            calcValue = newCalcValue;
            // Let dependents know
            OnValueChange();
        }
    }
    protected override float Calc(BuffableStatEventArgs StatEventArgs)
    {
        return (baseValue + addBuff) * multBuff;
    }
}

// A lil awkward. Basically, effects cannot directly modify how health changes.
// Any effects which modify how much to change health should be implemented in an "upstream" layer.
public class HealthStat : BaseStat<float, HealthChangeEventArgs>
{
    private BuffableStat maxHealth;

    public HealthStat (BuffableStat maxHealth) : base(maxHealth.BaseValue)
    {
        this.maxHealth = maxHealth; 
        maxHealth.ValueChange += Refresh;
    }
    
    public void ChangeHealth(float amt)
    {
        baseValue = Math.Clamp(baseValue + amt, 0, maxHealth.Value);
        Refresh();
    }

    public override void Refresh()
    {
        HealthChangeEventArgs statArgs = new HealthChangeEventArgs(baseValue-calcValue, calcValue, maxHealth.Value);
        // UnityEngine.Debug.Log("Invoking {0} statuses, ", TriggerStatuses?.GetInvocationList().Length);
        OnTriggerEffects(statArgs);

        // Calc needs to clamp since it's called when the maxHealth changes.
        float newCalcValue = Calc(statArgs);

        if (!newCalcValue.Equals(calcValue))
        {
            calcValue = newCalcValue;
            baseValue = newCalcValue;
            // Let dependents know
            OnValueChange();
        }
    }

    protected override float Calc(HealthChangeEventArgs args)
    {
        return Math.Clamp(baseValue, 0, maxHealth.Value);
    }
}