using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

// Holds every stat a unit has, and has triggers. May eventually need to be broken up.
public class StatManager : MonoBehaviour
{
    [SerializeField]
    private float baseHealth = 20f;
    [SerializeField]
    private float baseDefense = 3f;

    public BuffableStat MaxHealth;
    
    public BuffableStat Attack;
    public BuffableStat Defense;

    public HealthStat Health;

    public event EventHandler Damaging;
    public event EventHandler Damaged;

    public Dictionary<string, BuffableStat> BuffableStats { get => buffableStats;}
    private Dictionary<string, BuffableStat> buffableStats;
    void Start()
    { 
        // Reflection to assemble a dict of buffable stats
        Defense = new BuffableStat(baseDefense);
        MaxHealth = new BuffableStat(baseHealth);
        Health = new HealthStat(MaxHealth);

        buffableStats = 
            typeof(StatManager).GetFields().Where(
            f => f.FieldType.Equals(typeof(BuffableStat)) || f.FieldType.IsSubclassOf(typeof(BuffableStat))
            ).ToDictionary(f => f.Name, f => (BuffableStat) f.GetValue(this));
        Debug.Log("Buffable Stats: " + string.Join(", ", buffableStats.Keys.ToArray()));
    }

    public void ReceiveDamage(Damage d) {
        Damaging?.Invoke(this, new EventArgs());
        Health.ChangeHealth(-Math.Max(d.Value-Defense.Value, 0));
        Damaged?.Invoke(this, new EventArgs());
    }

#if (UNITY_EDITOR)
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
            UnityEditor.Handles.Label(transform.position+Vector3.up, $"Health: {Health.Value}\n Max: {MaxHealth.Value}");
    }
#endif

}
