using UnityEngine;
using System;
using UnityEngine.InputSystem.Interactions;
using System.Linq;

public abstract class Effect {
    public abstract bool Attach(GameObject target);
    public abstract void Detach();
}

// we might want a factory for statuses, for example.
public abstract class StatChangeEffect : Effect 
{
    StatManager stats;
    protected string targetStatName;
    private BuffableStat targetStat = null;

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
    }
}
public class AdditiveBuff: StatChangeEffect {
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