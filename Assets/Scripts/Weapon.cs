using System;
using UnityEngine;
public class Weapon
{
    public BuffableStat Attack = new BuffableStat(5f);
    public BuffableStat CritRate = new BuffableStat(0.25f);
    public BuffableStat CritMultiplier = new BuffableStat(1.5f);
    private GameObject gameObject;

    public Weapon(GameObject g)
    {
        gameObject = g;
    }
    
    public void Use (StatManager target)
    {
        Damage outgoing = new Damage(gameObject){Value = Attack.Value};
        if (UnityEngine.Random.Range(0f, 1f) > CritRate.Value)
        {
            Debug.Log("Crit!");
            outgoing.Value *= CritMultiplier.Value;
            outgoing.IsCrit = true;
        }
        Debug.Log($"Doing {outgoing.Value} Damage");
        target.ReceiveDamage(outgoing);
    }
}