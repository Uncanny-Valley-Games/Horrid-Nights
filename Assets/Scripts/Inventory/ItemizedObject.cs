using UnityEngine;

public class ItemizedObject : MonoBehaviour
{
    public void isHolding(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
