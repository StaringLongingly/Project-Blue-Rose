using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform cameraTransform;
    void Start() { cameraTransform = Camera.main.transform; }

    void Update() { gameObject.transform.up = (cameraTransform.position - gameObject.transform.position).normalized; }
}
