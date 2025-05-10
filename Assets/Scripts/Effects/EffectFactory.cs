using System.Collections.Generic;
using UnityEngine;
public class EffectFactory
{
    public enum ItemName
    {
        LUNCH_BOX,
        PENCIL
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
        default:
            Debug.LogWarning("Effect Enum not implemented");
            return null;
        }
    }
}