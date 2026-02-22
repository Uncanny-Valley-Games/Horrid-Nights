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
    public Collider itemCollider;

    private Rigidbody _rb;
    private GameObject handObject;
    
    private bool _isEquipped;
    
    public bool IsEquipped()
    {
        return _isEquipped;
    }
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true; // starts as a static object, then switches to ridged once dropped
        itemCollider.isTrigger = false;
    }

    private void Update()
    {
        if (handObject is not null)
        {
            transform.position = handObject.transform.position;
            transform.rotation = handObject.transform.rotation;
        }
    }

    public void PickUp(GameObject hand)
    {
        _isEquipped = true;
        _rb.isKinematic = true;
        itemCollider.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
        handObject = hand;
    }

    public void Drop()
    {
        _isEquipped = false;
        _rb.isKinematic = false;
        itemCollider.isTrigger = false;
        gameObject.layer = LayerMask.NameToLayer("Inventory Item");
        handObject = null;
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
