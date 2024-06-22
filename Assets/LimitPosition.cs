using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitPosition : MonoBehaviour
{
    public Vector3 newPosition;
    public float minimumY;
    public float maximumMagnitude;

    // Update is called once per frame
    void FixedUpdate()
    {
        float newYPosition = transform.position.y;
        if (newYPosition < minimumY)
        {
            newYPosition = minimumY;
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
        Vector3 finalPosition = new Vector3(transform.position.x, newYPosition, transform.position.z);

        transform.Translate(finalPosition - transform.position, Space.World);
    }
}
