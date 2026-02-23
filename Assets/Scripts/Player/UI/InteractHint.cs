using UnityEngine;
using UnityEngine.XR;

public class InteractHint : MonoBehaviour
{
    [SerializeField] private GameObject hintUI;
    
    void Start()
    {
        hintUI.SetActive(false);
    }

    public void EnableHint()
    {
        hintUI.SetActive(true);
    }
    
    public void DisableHint()
    {
        hintUI.SetActive(false);
    }

}
