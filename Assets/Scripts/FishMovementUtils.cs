﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovementUtils : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public static void MoveTowardsTarget(Rigidbody rb, Vector3 targetDirection, float speed) {
        // apply force counter to current direction proportional to magnitude of turn
        float differenceFactor = (rb.velocity.normalized - targetDirection.normalized).magnitude;
        Vector3 counterCurrentMovement = rb.velocity.normalized * differenceFactor * -1;
        Vector3 forceVector = targetDirection + counterCurrentMovement;
        rb.AddForce(forceVector.normalized * speed * Time.smoothDeltaTime);
    }

    public static void TurnToFace(Rigidbody rb, Transform t, Vector3 targetDirection) {
        rb.AddTorque(Vector3.Cross(t.forward, targetDirection) * 1);
    }
}
