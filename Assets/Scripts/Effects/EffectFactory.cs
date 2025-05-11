using System.Collections.Generic;
using UnityEngine;
public class EffectFactory
{
    public enum ItemName
    {
        LUNCH_BOX,
        PENCIL,
        CYCLE_A,
        CYCLE_B,
    }
    private static EffectFactory instance;
    public static EffectFactory Instance { 
        get {
            if (instance == null) instance = new EffectFactory();
            return instance;
        }
    }

    public HashSet<Effect> MakeItemEffects(ItemName name)
    {
        switch (name)
        {
        case ItemName.LUNCH_BOX:
            return new HashSet<Effect>{new FlatBuffHealth(10f)};
        case ItemName.PENCIL:
            return new HashSet<Effect>{new AdditiveBuff("Attack", 10f)};
        case ItemName.CYCLE_A:
            return new HashSet<Effect>{new CrossStatBuff("MaxHealth", "Defense", 1f)};
        case ItemName.CYCLE_B:
            return new HashSet<Effect>{new CrossStatBuff("Defense", "MaxHealth", 1f)};
        default:
            Debug.LogWarning("Effect Enum not implemented");
            return null;
        }
    }
}