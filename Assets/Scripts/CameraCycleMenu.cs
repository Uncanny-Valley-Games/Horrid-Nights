using System.Collections;
using UnityEngine;

public class CameraCycleMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] waypoints;
    [SerializeField] private float cycleTime = 10f;
    
    private int currentWaypoint = 0;
    private bool canCycle = true;

    void Update()
    {
        if (canCycle)
        {
            StartCoroutine(CycleWaypoint());
        }
        
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, waypoints[currentWaypoint].transform.position, Time.deltaTime * cycleTime);
        gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, waypoints[currentWaypoint].transform.rotation, Time.deltaTime);
    }

    IEnumerator CycleWaypoint()
    {
        canCycle = false;
        yield return new WaitForSeconds(cycleTime);
        canCycle = true;
        currentWaypoint++;
        if (currentWaypoint >= waypoints.Length) currentWaypoint = 0;
    }
}
