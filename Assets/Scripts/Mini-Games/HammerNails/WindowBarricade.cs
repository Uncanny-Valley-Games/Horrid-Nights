using UnityEngine;

public class WindowBarricade : MonoBehaviour
{
    [SerializeField] Color normalColour;
    [SerializeField] Color damagedColour;

    bool isDamaged = false;

    void Start()
    {
        GetComponent<Renderer>().material.color = normalColour;
    }

    public void WindowFixed()
    {
        GetComponent<Renderer>().material.color = normalColour;
        isDamaged = false;
    }

    public void WindowDamaged()
    {
        if (!isDamaged)
        {
            GetComponent<Renderer>().material.color = damagedColour;
            isDamaged = true;
        }
        else
        {
            Debug.Log("Window has already been attacked");
        }
    }
}
