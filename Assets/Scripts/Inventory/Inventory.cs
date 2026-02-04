using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject dropBoxPrefab;
    
    private const int INVENTORY_SIZE = 3;
    
    private InventoryItem[] items = new InventoryItem[INVENTORY_SIZE]; // can hold a maximum of 3 items
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
