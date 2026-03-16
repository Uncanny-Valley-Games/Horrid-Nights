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
    private MeshRenderer itemRenderer;
    private GameObject _handObject;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    
    private bool _isEquipped;
    
    public bool IsEquipped()
    {
        return _isEquipped;
    }
    
    private void Start()
    {
        itemRenderer = GetComponentInChildren<MeshRenderer>();
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true; // starts as a static object, then switches to ridged once dropped
        itemCollider.isTrigger = false;
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

    private void Update()
    {
        if (_handObject is not null)
        {
            transform.position = _handObject.transform.position;
            transform.rotation = _handObject.transform.rotation;
        } else if (transform.position.y < -100)
        {
            // when _handObject is null, the object could be a body affected by gravity. to prevent it from falling into
            // the void forever, we have it so it resets the object's transform and state
            ResetPosAndRot();
        }
    }

    private void ResetPosAndRot()
    {
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;
        _rb.isKinematic = true;
    }

    public void PickUp(GameObject hand)
    {
        _isEquipped = true;
        _rb.isKinematic = true;
        itemCollider.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
        _handObject = hand;
    }

    public void Drop()
    {
        _isEquipped = false;
        _rb.isKinematic = false;
        itemCollider.isTrigger = false;
        gameObject.layer = LayerMask.NameToLayer("Inventory Item");
        _handObject = null;
    }

    public void Equip()
    {
        // itemRenderer.enabled = true;
        gameObject.SetActive(true);
    }

    public void UnEquip()
    {
        // itemRenderer.enabled = false;
        gameObject.SetActive(false);
    }
}
