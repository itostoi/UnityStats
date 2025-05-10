using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class Item : MonoBehaviour
{
    [SerializeField] private ItemSlot slot;
    [SerializeField] private GameObject owner;
    [SerializeField] public EffectFactory.ItemName ItemName;

    public ItemSlot Slot {get => slot;}
    public GameObject Owner {get => owner;}

    private Camera cam;
    private Vector3 startLoc;
    private HashSet<Effect> effects;

    void Start()
    {
        effects = EffectFactory.Instance.MakeItemEffects(ItemName);
        if (Slot != null) { StartCoroutine(enterSlot()); }
        cam = Camera.main;
    }

    public bool Attach(GameObject g)
    {
        if (owner != null) Detach();
        if (g == null) return true; // TODO: ensure correctness if no owner.
        owner = g;

        foreach (Effect effect in effects) { effect.Attach(g);}
        return true;
    }
    public void Detach()
    {
        foreach (Effect effect in effects) { effect.Detach();}
        owner = null;
    }

    // private float dragTime = 0;
    void OnMouseDown()
    {
        // record start point
        if (slot != null) slot.RemoveItem();
        startLoc = transform.position;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }
    void OnMouseDrag()
    {
        Vector3 thing = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        thing.z = 0;
        transform.position = thing;
    }

    void OnMouseUp()
    {
        bool validDestination = false;
        RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Mouse.current.position.ReadValue()), Vector2.zero);

        ItemSlot s = null;
        if (hit.collider != null && 
            hit.collider.gameObject.TryGetComponent(out s))
        {
            if (s.AcceptItem(this))
            {
                slot = s;
                owner = slot.Owner;
                Attach(owner);
                validDestination = true;
            }
        }

        gameObject.layer = LayerMask.NameToLayer("Default");

        if (validDestination)
        {
            StartCoroutine(enterSlot());
        } else {
            transform.position = startLoc;
            startLoc = Vector3.zero;
        }
    }

    IEnumerator enterSlot()
    {
        Vector3 startPos = transform.position;
        float t = 0;
        float lerpTime = 0.5f;
        while (t <= lerpTime)
        {
            transform.position = Vector3.Lerp(startPos, Slot.transform.position, t/lerpTime);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = Slot.transform.position;
        yield return null;
    }
}