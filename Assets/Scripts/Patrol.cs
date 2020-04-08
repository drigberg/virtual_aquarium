using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Be sure to set drag Drag and AngularDrag to reasonable numbers,
or else the rigidbody will overshoot and spin out of control */
public class Patrol : MonoBehaviour
{
    public Transform minXYZ;
    public Transform maxXYZ;
    public float speed;
    public float targetRadius;
    
    private Rigidbody rb;
    private Vector3 target;

    [Header ("Collisions")]
    public LayerMask obstacleMask;
    public float collisionAvoidDst;

    void Start() {
        target = GetNextTarget();
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        // switch to next target if within range or obstacle in the way
        if (HaveReachedDestination(target) || CollisionInDirection(target - transform.position))
        {
            target = GetNextTarget();
        }
    }

    void FixedUpdate() {
        FishMovementUtils.RotateUpright(rb, transform);
        MoveTowardsTarget();
    }

    bool HaveReachedDestination(Vector3 destination) {
        return Vector3.Distance(destination, transform.position) < targetRadius;
    }

    Vector3 GetNextTarget() {
        // try a few times to find a location out of range, then give up
        Vector3 newTarget = new Vector3(0, 0, 0); 
        for (int i = 0; i < 10; i++) {
            newTarget = new Vector3(
                Random.Range(minXYZ.position.x, maxXYZ.position.x),
                Random.Range(minXYZ.position.y, maxXYZ.position.y),
                Random.Range(minXYZ.position.z, maxXYZ.position.z));
            if (!HaveReachedDestination(newTarget) && !CollisionInDirection(newTarget - transform.position)) {
                return newTarget;
            }
        }
        return newTarget;
    }

    void MoveTowardsTarget() {
        Vector3 targetDirection = target - transform.position;
        FishMovementUtils.MoveTowardsTarget(rb, targetDirection, speed);
        
        // face midway between current velocity vector and target: like anticipating a turn
        Vector3 facingDirection = (rb.velocity + targetDirection) / 2;
        FishMovementUtils.TurnToFace(rb, transform, facingDirection);
    }

    bool CollisionInDirection (Vector3 direction) {
        if (Physics.Raycast (transform.position, direction, collisionAvoidDst, obstacleMask)) {
            return true;
        } else { }
        return false;
    }
}
