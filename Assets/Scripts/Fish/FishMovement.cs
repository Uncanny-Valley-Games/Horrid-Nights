using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [SerializeField] Transform fishPoint;
    [SerializeField] Transform fishPoint2;

    Transform currentWaypoint;
    Vector3 direction;

    void Start()
    {
        currentWaypoint = fishPoint;
        direction = Vector3.Normalize(currentWaypoint.position - transform.position);
    }

    void Update()
    {
        // Makes the fish spin, leaving it commented for now
        //transform.Rotate(direction);
    }

    void FixedUpdate()
    {
        transform.Translate(direction * 1.5f * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (currentWaypoint == fishPoint)
        {
            currentWaypoint = fishPoint2;
            direction = Vector3.Normalize(currentWaypoint.position - transform.position);
        }
        else
        {
            currentWaypoint = fishPoint;
            direction = Vector3.Normalize(currentWaypoint.position - transform.position);
        }
    }

}
