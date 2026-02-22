using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public enum ItemName
    {
        Nothing,
        Test,
        Axe,
        FishingRod,
        Hammer
    }
    
    public ItemName itemName;
    public Collider collider;

    private Rigidbody _rb;
    private Transform _myTransform;
    
    private bool _isEquipped;
    
    public bool IsEquipped()
    {
        return _isEquipped;
    }
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _myTransform = GetComponent<Transform>();
        _rb.isKinematic = true; // starts as a static object, then switches to ridged once dropped
        collider.isTrigger = false;
        _myTransform = transform;
    }

    
    void Update()
    {
        
    }

    public void PickUp(Transform equipTransform)
    {
        _isEquipped = true;
        _rb.isKinematic = true;
        collider.isTrigger = true;
        _myTransform.position = equipTransform.position;
    }

    public void Drop()
    {
        _isEquipped = false;
        _rb.isKinematic = false;
        collider.isTrigger = false;
    }

    public void Equip()
    {
        gameObject.SetActive(true);
    }

    public void UnEquip()
    {
        gameObject.SetActive(false);
    }
}
