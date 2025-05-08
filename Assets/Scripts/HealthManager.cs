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

// A lil awkward. Basically, statuses cannot directly modify how health changes.
// Any statuses which modify how much to change health should be implemented in an "upstream" layer.
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
        HealthChangeEventArgs statusArgs = new HealthChangeEventArgs(baseValue-calcValue, calcValue, maxHealth.Value);
        // UnityEngine.Debug.Log("Invoking {0} statuses, ", TriggerStatuses?.GetInvocationList().Length);
        OnTriggerStatuses(statusArgs);

        // Calc needs to clamp since it's called when the maxHealth changes.
        float newCalcValue = Calc(statusArgs);

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
