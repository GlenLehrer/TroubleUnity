using UnityEngine;

public class Tracker : MonoBehaviour
{
    public Transform trackedObject;
    public float updateSpeed = 3;
    public Vector2 trackingOffset;
    public Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = (Vector3)trackingOffset;
        offset.z = transform.position.z - trackedObject.position.z;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, trackedObject.position + offset, updateSpeed * Time.deltaTime);
    }
}
