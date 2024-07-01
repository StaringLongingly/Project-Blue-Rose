using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveTriggerWhenColliding : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Grabbable") gameObject.GetComponent<BreakOff>().BreakOffFromGround();
        else Debug.Log("Collided, but the other collider isnt Grabbable");
    }
}
