using UnityEngine;
using System;
using UnityEngine.InputSystem.Interactions;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public abstract class Effect {
    public abstract bool Attach(GameObject target);
    public abstract void Detach();
}

// we might want a factory for statuses, for example.
public abstract class StatChangeEffect : Effect 
{
    protected StatManager stats;
    protected string targetStatName;
    protected BuffableStat targetStat = null;

    public abstract void DoEffect(System.Object o, BuffableStatEventArgs args);

    public override bool Attach(GameObject o) 
    {
        if (o.TryGetComponent(out stats)) {
            targetStat = stats.BuffableStats[targetStatName];
            if (targetStat != null) {
                targetStat.TriggerEffects += DoEffect;
                targetStat.Refresh();
                return true;
            }
            else
            {
                Debug.LogWarning("Could not get stat on StatManager: " + targetStatName);
            }
        }
        Debug.LogWarning("Failed to Attach an Effect: " + GetType().Name);
        return false;
    }

    public override void Detach()
    {
        if (targetStat != null)
        {
            targetStat.TriggerEffects -= DoEffect;
            targetStat.Refresh();
        }
        targetStat = null;
    }
}
public class AdditiveBuff : StatChangeEffect {
    public AdditiveBuff(string targetStatName, float buffValue) {
        this.targetStatName = targetStatName;
        this.buffValue = buffValue;
    }
    protected float buffValue = 0f;
    public override void DoEffect(System.Object o, BuffableStatEventArgs args)
    {
        args.AddBuff += buffValue;
    }
}

public class CrossStatBuff : StatChangeEffect {
    string sourceStatName;
    BuffableStat sourceStat;
    float sourceValue;
    float multiplier;

    private void updateSource(System.Object o, EventArgs args) {
        targetStat.Refresh();
    }

    public override bool Attach(GameObject o)
    {
        if (o.TryGetComponent(out stats)) {
            targetStat = stats.BuffableStats[targetStatName];
            if (targetStat != null) {
                sourceStat = stats.BuffableStats[sourceStatName];
                sourceStat.ValueChange += updateSource;
                targetStat.TriggerEffects += DoEffect;
                targetStat.Refresh();
                return true;
            }
            else
            {
                Debug.LogWarning("Could not get stat on StatManager: " + targetStatName);
            }
        }
        Debug.LogWarning("Failed to Attach an Effect: " + GetType().Name);
        return false;
    }

    public override void Detach()
    {
        if (targetStat != null)
        {
            targetStat.TriggerEffects -= DoEffect;
            targetStat.Refresh();
            if (sourceStat != null) sourceStat.ValueChange -= updateSource;
            targetStat = null;
            sourceStat = null;
        }
    }

    public CrossStatBuff(string targetStatName, string sourceStatName, float multiplier)
    {
        this.sourceStatName = sourceStatName;
        this.targetStatName = targetStatName;
        this.multiplier = multiplier;
    }
    public override void DoEffect(System.Object o, BuffableStatEventArgs args)
    {
        Debug.Log(sourceStat.Value * multiplier);
        args.AddBuff += sourceStat.Value * multiplier;
    }
}

public class FlatBuffHealth : AdditiveBuff {
    public FlatBuffHealth(float buffValue = 10f) : base("MaxHealth", buffValue) {}
}

/*
public class TestWeaponBuff : Effect {
    TestBehaviour testBehaviour;
    private float buffValue = 0.1f;
    private void AddStat(System.Object o, BuffableStatEventArgs args) {
        args.AddBuff += buffValue;
    }
    public override bool Attach(GameObject target) {
        if (target.TryGetComponent(out testBehaviour)) {
            testBehaviour.weapon.CritRate.TriggerEffects += AddStat;
            testBehaviour.weapon.CritRate.Refresh();
        } else {
            return false;
        }
        return true;
    }

    public override void Detach()
    {
        if (testBehaviour != null) {
            testBehaviour.weapon.CritRate.TriggerEffects -= AddStat;
            testBehaviour.weapon.CritRate.Refresh();
        }
    }
}
*/