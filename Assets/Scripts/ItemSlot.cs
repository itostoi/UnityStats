using UnityEngine;


// Needs to store state for being occupied/not occupied
// Additionally, upon being added to a slot, an item's effects need to be attached to 
// the owner of the slot, and the owner's stats need to be updated.

[RequireComponent(typeof(Collider2D))]
public class ItemSlot : MonoBehaviour
{
    [SerializeField] public GameObject Owner;
    private Item occupant = null;

    public bool Occupied {get => occupant != null;}

    public bool AcceptItem(GameObject g)
    {
        Item item = null;
        if (g.TryGetComponent(out item))
        {
            return AcceptItem(item);
        }
        return false;
    }

    // Can override to set conditions, eg. require a class.
    public bool AcceptItem(Item item)
    {
        if (Occupied) return false;
        occupant = item;
        return true;
    }
    
    public void RemoveItem() 
    {
        occupant.Detach();
        occupant = null;
    }
}