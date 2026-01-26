using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private ItemizedObject defaultItem; // the arm
        
        private int maxCapacity = 3;

        private int currentlyHoldingIndex = 0;
        
        // item id : itemized object class
        private Dictionary<string, ItemizedObject> items = new Dictionary<string, ItemizedObject>();
        
        public void AddItem(ItemizedObject item)
        {
            items.Add(item.GetItemName(), item);
        }

        public void RemoveItem(ItemizedObject item)
        {
            items.Remove(item.GetItemName());
        }
    }
}
