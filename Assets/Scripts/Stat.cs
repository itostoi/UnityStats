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
abstract public class BaseStat<T, E> where T : IEquatable<T> where E : EventArgs 
{
    public BaseStat(T baseValue) {
        this.baseValue = baseValue; 
        calcValue = baseValue;
    }

    public event EventHandler ValueChange;
    public event EventHandler<E> TriggerStatuses;

    // Statuses should (almost) never change this (since there's no infra to add/remove statuses correctly)
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

    protected virtual void OnTriggerStatuses(E statusArgs)
    {
        TriggerStatuses?.Invoke(this, statusArgs);
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

public class FloatStat : BaseStat<float, StatusArgs<float>>
{
    public FloatStat(float initialValue) : base(initialValue) { }
    public override void Refresh()
    {
        StatusArgs<float> statusArgs = new StatusArgs<float>{StatValue = baseValue};
        // UnityEngine.Debug.Log("Invoking {0} statuses, ", TriggerStatuses?.GetInvocationList().Length);
        OnTriggerStatuses(statusArgs);
        // Explicit calc using upstream stats
        float newCalcValue = Calc(statusArgs);

        if (!newCalcValue.Equals(calcValue))
        {
            calcValue = newCalcValue;
            // Let dependents know
            OnValueChange();
        }
    }
    protected override float Calc(StatusArgs<float> statusArgs)
    {
        return statusArgs.StatValue;
    }
}

// Statuses can modify a buffable stat more elegantly.
public class BuffableStat : BaseStat<float, BuffableArgs>
{
    public BuffableStat(float initialValue) : base(initialValue) {}

    private float multBuff = 1f;
    private float addBuff = 0f;
    public float MultBuff {get => multBuff;}
    public float AddBuff {get => addBuff;}

    public override void Refresh()
    {
        // glue additional stuff onto statusargs.
        BuffableArgs statusArgs = new BuffableArgs{StatValue = baseValue, MultBuff = 1f, AddBuff = 0f};
        OnTriggerStatuses(statusArgs);
        multBuff = statusArgs.MultBuff;
        addBuff = statusArgs.AddBuff;

        float newCalcValue = Calc(statusArgs);

        if (!newCalcValue.Equals(calcValue))
        {
            calcValue = newCalcValue;
            // Let dependents know
            OnValueChange();
        }
    }
    protected override float Calc(BuffableArgs statusArgs)
    {
        return (baseValue + addBuff) * multBuff;
    }
}