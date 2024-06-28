using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveTriggerWhenColliding : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) { gameObject.GetComponent<BreakOff>().BreakOffFromGround(); }
}
