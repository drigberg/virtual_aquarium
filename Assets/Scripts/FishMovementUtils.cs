using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovementUtils : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public static void MoveTowardsTarget(Rigidbody rb, Vector3 targetDirection, float speed) {
        rb.AddForce(targetDirection.normalized * speed * Time.smoothDeltaTime);
    }

    public static void TurnToFace(Rigidbody rb, Transform t, Vector3 targetDirection) {
        rb.AddTorque(Vector3.Cross(t.forward, targetDirection) * 1);
    }
}
