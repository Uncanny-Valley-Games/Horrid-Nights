using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public enum ItemName
    {
        Nothing,
        Axe,
        FishingRod,
        Hammer
    }
    
    public ItemName itemName;
    public Collider _collider;

    private Rigidbody _rb;
    private bool isEquipped;

    public bool IsEquipped()
    {
        return isEquipped;
    }
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true; // starts as a static object, then switches to ridgid once dropped
    }

    
    void Update()
    {
        
    }

    public void Drop()
    {
        isEquipped = false;
        _rb.isKinematic = false;
    }
}
