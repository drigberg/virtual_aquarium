using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovementUtils : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public static Vector3 MoveTowardsTarget(Rigidbody rb, Vector3 targetDirection, float speed) {
        // apply force counter to current direction proportional to magnitude of turn
        float differenceFactor = (rb.velocity.normalized - targetDirection.normalized).magnitude;
        Vector3 counterCurrentMovement = rb.velocity.normalized * differenceFactor * -1;
        Vector3 forceVector = targetDirection + counterCurrentMovement;
        Vector3 forceToAdd = forceVector.normalized * speed * Time.smoothDeltaTime;
        rb.AddForce(forceToAdd);
        return forceToAdd;
    }

    public static void TurnToFace(Rigidbody rb, Transform t, Vector3 targetDirection) {
        rb.AddTorque(Vector3.Cross(t.forward, targetDirection) * 1);
    }

    public static void RotateUpright(Rigidbody rb, Transform t) {
        // https://stackoverflow.com/questions/58419942/stabilize-hovercraft-rigidbody-upright-using-torque
        Quaternion deltaQuat = Quaternion.FromToRotation(t.up, Vector3.up);

        Vector3 axis;
        float angle;
        deltaQuat.ToAngleAxis(out angle, out axis);

        float dampenFactor = 0.8f; // this value requires tuning
        float adjustFactor = 0.5f; // this value requires tuning
        rb.AddTorque(axis.normalized * angle * adjustFactor - rb.angularVelocity * dampenFactor, ForceMode.Acceleration);
    }
}
