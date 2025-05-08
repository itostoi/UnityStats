using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class TestBehaviour : MonoBehaviour
{
    private HealthManager healthManager;
    public Weapon weapon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthManager = GetComponent<HealthManager>();
        weapon = new Weapon(gameObject);
    }

    void OnMouseDown()
    {
        Debug.Log("mouse clicked");
        Debug.Log("Flat Health Buff: " + healthManager.MaxHealth.AddBuff);
        Debug.Log("Mult Health Buff: " + healthManager.MaxHealth.MultBuff);
        Debug.Log("Defense: " + healthManager.Defense.Value);
        Debug.Log("Max Health: " + healthManager.MaxHealth.Value);
        Debug.Log("Cur Health: " + healthManager.Health.Value);

        Debug.Log("Weapon Attack: " + weapon.Attack.Value);
        Debug.Log("Crit Rate: " + weapon.CritRate.Value);
        Debug.Log("Crit Mult: " + weapon.CritMultiplier.Value);
    }

    // Update is called once per frame
    void Update()
    {
        // healthManager.FlatHealthBuff.Value = (float) (1 + Math.Pow(Math.Sin(Time.time/4), 2));
        if (Input.GetKeyDown(KeyCode.A))
        {
            weapon.Use(healthManager);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            BuffWeapon();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            AddTempHealth();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            AddPermHealth();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            healthManager.Health.ChangeHealth(100f);
        }
    }

    private void AddTempHealth()
    {
        Status s = gameObject.AddComponent<Status>();
        s.effect = new FlatBuffHealth(20f);
        s.duration = new TimeDuration(5f);
    }
    private void AddPermHealth()
    {
        Status s = gameObject.AddComponent<Status>();
        s.effect = new FlatBuffHealth(2f);
        s.duration = new PermanentDuration();
    }
    private void BuffWeapon()
    {
        Status s = gameObject.AddComponent<Status>();
        s.effect = new TestWeaponBuff();
        s.duration = new PermanentDuration();
    }
}

