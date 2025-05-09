using UnityEngine;
using System;

public abstract class Effect {
    public abstract bool Attach(GameObject target);
    public abstract void Detach();
}

// we might want a factory for statuses, for example.

public class FlatBuffHealth : Effect {
    HealthManager healthManager;
    private float buffValue = 10f;
    public FlatBuffHealth(float buffValue) {
        this.buffValue = buffValue;
    }
    
    private void AddStat(System.Object o, BuffableStatEventArgs args) {
        args.AddBuff += buffValue;
    }
    public override bool Attach(GameObject target) {
        if (target.TryGetComponent(out healthManager)) {
            healthManager.MaxHealth.TriggerEffects += AddStat;
            healthManager.MaxHealth.Refresh();
        }
        else 
        {
            return false;
        }
        return true;
    }

    public override void Detach()
    {
        if (healthManager != null) {
            healthManager.MaxHealth.TriggerEffects -= AddStat;
            healthManager.MaxHealth.Refresh();
        }
    }
}

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
        }
        else 
        {
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


