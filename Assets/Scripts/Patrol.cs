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
    public float boundsRadius;
    public float avoidCollisionWeight;
    public float collisionAvoidDst;
    private Vector3 collisionAvoidanceVector;
    private bool isHeadingForCollision;

    void Start() {
        target = GetNextTarget();
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        // switch to next target if within range
        if (HaveReachedDestination(target))
        {
            target = GetNextTarget();
        }

        isHeadingForCollision = IsHeadingForCollision();
        if (isHeadingForCollision) {
            Vector3 collisionAvoidDir = ObstacleRays ();
            collisionAvoidanceVector = collisionAvoidDir * speed * avoidCollisionWeight;
        } else {
            collisionAvoidanceVector = Vector3.zero;
        }
    }

    void FixedUpdate() {
        MoveTowardsTarget();
        FishMovementUtils.RotateUpright(rb, transform);
    }

    bool HaveReachedDestination(Vector3 destination) {
        return Vector3.Distance(destination, transform.position) < targetRadius;
    }

    Vector3 GetNextTarget() {
        // try a few times to find a location out of range, then give up
        Vector3 newTarget = new Vector3(0, 0, 0); 
        for (int i = 0; i < 5; i++) {
            newTarget = new Vector3(
                Random.Range(minXYZ.position.x, maxXYZ.position.x),
                Random.Range(minXYZ.position.y, maxXYZ.position.y),
                Random.Range(minXYZ.position.z, maxXYZ.position.z));
            if (!HaveReachedDestination(newTarget)) {
                return newTarget;
            }
        }
        return newTarget;
    }

    void MoveTowardsTarget() {
        Vector3 targetDirection = target - transform.position;
        Vector3 sumVector = targetDirection + collisionAvoidanceVector;
        FishMovementUtils.MoveTowardsTarget(rb, sumVector, speed);
        
        // face midway between current velocity vector and target: like anticipating a turn
        Vector3 facingDirection = (rb.velocity + sumVector) / 2;
        FishMovementUtils.TurnToFace(rb, transform, facingDirection);
    }

    bool IsHeadingForCollision () {
        RaycastHit hit;
        if (Physics.SphereCast (transform.position, boundsRadius, transform.forward, out hit, collisionAvoidDst, obstacleMask)) {
            return true;
        } else { }
        return false;
    }

    Vector3 ObstacleRays () {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++) {
            Vector3 dir = transform.TransformDirection (rayDirections[i]);
            Ray ray = new Ray (transform.position, dir);
            if (!Physics.SphereCast (ray, boundsRadius, collisionAvoidDst, obstacleMask)) {
                return dir;
            }
        }

        return transform.forward;
    }
}
