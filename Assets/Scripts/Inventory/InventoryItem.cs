using UnityEngine;

public class InventoryItem
{
    private string name;
    private GameObject prefab;
    
    // todo: add item pictures too (if needed)

    public InventoryItem(string name, GameObject prefab)
    {
        this.name = name;
        this.prefab = prefab;
    }
    
    public GameObject GetPrefab()
    {
        return prefab;
    }

    public string GetName()
    {
        return name;
    }
}
