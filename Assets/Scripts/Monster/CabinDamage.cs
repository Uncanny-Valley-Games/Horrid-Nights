using System.Collections;
using UnityEngine;

public class CabinDamage : MonoBehaviour
{
    [SerializeField] GameObject[] windows;

    [SerializeField] float timeBetweenAttacks = 10f;
    bool tryingToAttack;

    void Update()
    {
        if (!tryingToAttack)
        {
            StartCoroutine(AttemptToAttack());
        }
    }

    void DamageWindow()
    {
        Debug.Log("The monster attacked one of the windows!");
        
        // Gets a random number to damage one of the windows in the array
        int randNum = Random.Range(0, windows.Length);
        windows[randNum].GetComponent<WindowBarricade>().WindowDamaged();
    }

    IEnumerator AttemptToAttack()
    {
        tryingToAttack = true;
        yield return new WaitForSecondsRealtime(timeBetweenAttacks);

        int randNum = Random.Range(0, 5);

        if (randNum >= 2)
        {
            DamageWindow();
        }
        tryingToAttack = false;
    }
}
