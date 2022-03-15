using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // what the camera follows
    public Transform target;

    // zeroes out the velocity
    Vector3 velocity = Vector3.zero;

    // time to follow target
    public float smoothTime = .15f;

    private void FixedUpdate()
    {
        // target position
        Vector3 targetPos = target.position;

        //alight camera & target z position
        targetPos.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

    }

}
