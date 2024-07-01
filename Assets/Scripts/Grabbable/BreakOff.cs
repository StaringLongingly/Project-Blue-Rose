using UnityEngine;

public class BreakOff : MonoBehaviour
{
    public float breakOffRadius = 1f;
    public void BreakOffFromGround()
    {
        Collider[] hitCollidersOuter = Physics.OverlapSphere(transform.position, breakOffRadius + 0.5f);
        foreach (var hitCollider in hitCollidersOuter) { hitCollider.isTrigger = false; }

        Collider[] hitCollidersInner = Physics.OverlapSphere(transform.position, breakOffRadius);
        foreach (var hitCollider in hitCollidersInner) { hitCollider.attachedRigidbody.constraints = RigidbodyConstraints.None; }
    }
}
