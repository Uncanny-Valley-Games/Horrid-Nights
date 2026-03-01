using System;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemIndex;

    private void Start()
    {
        itemIndex.text = "0";
    }
    
    public void UpdateText(int index)
    {
        itemIndex.text = index.ToString();
    }
}
