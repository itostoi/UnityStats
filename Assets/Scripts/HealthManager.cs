using System;
using Unity.VisualScripting;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField]
    private float baseHealth = 20f;
    [SerializeField]
    private float baseDefense = 3f;

    public BuffableStat MaxHealth;
    
    public BuffableStat Defense;
    public HealthStat Health;

    public event EventHandler Damaging;
    public event EventHandler Damaged;

    void Start()
    { 
        Defense = new BuffableStat(baseDefense);
        MaxHealth = new BuffableStat(baseHealth);
        Health = new HealthStat(MaxHealth);
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
            UnityEditor.Handles.Label(transform.position+Vector3.up, Health.Value.ToString());
    }
#endif

}
