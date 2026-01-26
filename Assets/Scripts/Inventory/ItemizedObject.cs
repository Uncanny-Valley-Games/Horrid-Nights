using UnityEngine;

namespace Inventory
{
    public class ItemizedObject : MonoBehaviour
    {
        private string itemName;
        private GameObject prefab;

        public GameObject GetPrefab()
        {
            return prefab;
        }

        private void SetPrefab(GameObject prefab)
        {
            this.prefab = prefab;
        }

        public string GetItemName()
        {
            return itemName;
        }

        public void SetItemName(string itemName)
        {
            this.itemName = itemName;
        }

        private void OnUse()
        {
            // do something (must be overwritten by the child class)
        }
        
    }
}
