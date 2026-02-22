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
    private Transform myTransform;
    
    private bool isEquipped;
    
    public bool IsEquipped()
    {
        return isEquipped;
    }
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        myTransform = GetComponent<Transform>();
        _rb.isKinematic = true; // starts as a static object, then switches to ridgid once dropped
        _collider.isTrigger = false;
        myTransform = transform;
    }

    
    void Update()
    {
        
    }

    public void Equip(Transform equipTransform)
    {
        isEquipped = true;
        _rb.isKinematic = true;
        _collider.isTrigger = true;
        myTransform.position = equipTransform.position;
    }

    public void Drop()
    {
        isEquipped = false;
        _rb.isKinematic = false;
        _collider.isTrigger = false;
    }
}
