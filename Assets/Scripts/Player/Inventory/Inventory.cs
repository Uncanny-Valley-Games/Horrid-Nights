using UnityEngine;

public class Inventory : MonoBehaviour
{
    private InventoryItem[] _items = new InventoryItem[5];
    
    private int _currentlyHolding = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Pickup(InventoryItem item)
    {
        if (_items[_currentlyHolding].itemName != InventoryItem.ItemName.Nothing)
        {
            _items[_currentlyHolding].Drop();
        }
        item.PickUp(transform);
        _items[_currentlyHolding] = item;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        for (int i = 0; i < _items.Length; i++)
        {
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
