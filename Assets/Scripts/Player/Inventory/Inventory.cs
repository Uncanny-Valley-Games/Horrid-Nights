using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Inventory : MonoBehaviour
{
    public float maxDistance = 5f;
    public LayerMask inventoryItemLayerMask;
    public Transform headTransform;
    public GameObject handObject;
    public InputActionAsset playerInput;
    
    private InputAction interact;
    private InputAction dropItem;
    private InputAction nextItem;
    private InputAction previousItem;
    
    private InventoryItem[] _items = new InventoryItem[5];
    
    private int _currentlyHolding;
    
    void OnEnable()
    {
        playerInput.FindActionMap("Player").Enable();
    }

    void OnDisable()
    {
        playerInput.FindActionMap("Player").Disable();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interact = InputSystem.actions.FindAction("Interact");
        dropItem = InputSystem.actions.FindAction("Drop");
        nextItem = InputSystem.actions.FindAction("Next");
        previousItem = InputSystem.actions.FindAction("Previous");
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (interact.WasPressedThisFrame())
        {
            if (Physics.Raycast(headTransform.position, headTransform.forward, out hit, maxDistance, 
                    inventoryItemLayerMask)) 
                Pickup(hit.transform.GetComponent<InventoryItem>());
        } else if (dropItem.WasPressedThisFrame())
        {
            DropCurrentItem();
        } else if (nextItem.WasPressedThisFrame())
        {
            CycleItems();
        } else if (previousItem.WasPressedThisFrame())
        {
            CycleItems(false);
        }
        
    }

    private void CycleItems(bool next = true)
    {
        if (next)
        {
            _currentlyHolding += 1;
            if (_currentlyHolding >= _items.Length) _currentlyHolding = 0;
        }
        else
        {
            _currentlyHolding -= 1;
            if (_currentlyHolding < 0) _currentlyHolding = _items.Length - 1;
        }
        UpdateDisplay();
    }

    private void Pickup(InventoryItem item)
    {
        DropCurrentItem();
        item.PickUp(handObject);
        _items[_currentlyHolding] = item;
        UpdateDisplay();
    }

    private void DropCurrentItem()
    {
        if (_items[_currentlyHolding] is not null)
        {
            if (_items[_currentlyHolding].itemName != InventoryItem.ItemName.Nothing)
            {
                _items[_currentlyHolding].Drop();
                _items[_currentlyHolding] = null;
            }
        }
    }

    private void UpdateDisplay()
    {
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] is null) continue;
            if (i != _currentlyHolding)
            {
                _items[i].UnEquip();
            }
            else
            {
                _items[i].Equip();
            }
        }
    }
}
