using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Be sure to set drag Drag and AngularDrag to reasonable numbers,
or else the rigidbody will overshoot and spin out of control */
public class Patrol : MonoBehaviour
{
    public Transform[] targets;
    public float speed;
    public float targetRadius;
    private int currentTargetIndex;
    private Transform target;

    private Rigidbody rb;


    void Start() {
        target = targets[currentTargetIndex];
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        // switch to next target if within range
        target = targets[currentTargetIndex];
        if (Vector3.Distance(target.position, transform.position) < targetRadius)
        {
            currentTargetIndex = (currentTargetIndex + 1) % targets.Length;
            target = targets[currentTargetIndex];
        }
    }

    void FixedUpdate() {
        Vector3 targetDirection = target.position - transform.position;
        FishMovementUtils.MoveTowardsTarget(rb, targetDirection, speed);
        // face midway between current velocity vector and target: like anticipating a turn
        Vector3 facingDirection = (rb.velocity + targetDirection) / 2;
        FishMovementUtils.TurnToFace(rb, transform, facingDirection);
    }
}
