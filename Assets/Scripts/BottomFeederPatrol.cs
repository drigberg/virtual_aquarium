using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomFeederPatrol : MonoBehaviour
{
    public Rigidbody rb;
    public Transform minXZ;
    public Transform maxXZ;
    public float hoverHeight = 2.0f;
    public float restingHoverHeight = 0.5f;
    public float hoverMargin = 0.2f;
    public float targetRadius = 3.0f;
    public float restProbability = 0.5f;
    public float minRestTimeSeconds = 5.0f;
    public float maxRestTimeSeconds = 15.0f;
    public float speed = 500.0f;
    public Vector3 currentForce;
    private Vector3 currentTarget;
    private float restTimer = 0.0f;

    [Header ("Collisions")]
    public LayerMask obstacleMask;
    public float boundsRadius;
    public float avoidCollisionWeight;
    public float collisionAvoidDst;
    private Vector3 collisionAvoidanceVector;
    private bool isHeadingForCollision;


    // Start is called before the first frame update
    void Start()
    {
        currentTarget = GetNextTarget();
        if (ShouldRest()) {
            restTimer = GetRestInterval();
        }
    }

    // Update is called once per frame
    void Update()
    {
        FishMovementUtils.RotateUpright(rb, transform);
        if (restTimer > 0.0f) {
            IterateRestTimer();
            currentForce = AdjustAltitude(restingHoverHeight);
            return;
        }

        if (HaveReachedDestination(currentTarget)) {
            currentTarget = GetNextTarget();
            if (ShouldRest()) {
                restTimer = GetRestInterval();
                return;
            }
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
        if (restTimer <= 0.0f) {
            MoveTowardsTarget();
        }
    }

    bool HaveReachedDestination(Vector3 destination) {
        // check distance to point along XZ plane
        Vector3 destinationXZ = new Vector3(destination.x, 0, destination.z);
        Vector3 transformXZ = new Vector3(transform.position.x, 0, transform.position.z);
        return Vector3.Distance(destinationXZ, transformXZ) < targetRadius;
    }

    Vector3 GetNextTarget() {
        // try a few times to find a location out of range, then give up
        Vector3 newTarget = new Vector3(0, 0, 0); 
        for (int i = 0; i < 5; i++) {
            newTarget = new Vector3(
                Random.Range(minXZ.position.x, maxXZ.position.x),
                0,
                Random.Range(minXZ.position.z, maxXZ.position.z));
            if (!HaveReachedDestination(newTarget)) {
                return newTarget;
            }
        }
        return newTarget;
    }

    float GetRestInterval() {
        return Random.Range(minRestTimeSeconds, maxRestTimeSeconds);
    }

    void IterateRestTimer() {
        restTimer -= Time.deltaTime;
    }

    bool ShouldRest() {
        return Random.Range(0, 1) < restProbability;
    }

    float GetDistanceToTerrain() {
        RaycastHit hit;
        Ray downRay = new Ray(transform.position, -Vector3.up);
        if (Physics.Raycast(downRay, out hit)) {
            return hit.distance;
        }
        // assume at least 5 units away
        return 5.0f;
    }

    Vector3 AdjustAltitude(float targetAltitude) {
        // stay close to the terrain along the Y axis
        float distanceToTerrain = GetDistanceToTerrain();
        float hoverError = distanceToTerrain - targetAltitude;
        if (Mathf.Abs(hoverError) < hoverMargin) {
            return Vector3.zero;
        } else {
            float signError = Mathf.Abs(hoverError) / hoverError;
            return FishMovementUtils.MoveTowardsTarget(rb, -Vector3.up, speed * signError * 0.5f);
        }
    }

    void MoveTowardsTarget() {
        // move towards target along XZ plane
        Vector3 targetDirection = currentTarget - transform.position;
        Vector3 sumVector = targetDirection + collisionAvoidanceVector;
        currentForce = FishMovementUtils.MoveTowardsTarget(rb, sumVector, speed);
        if (!isHeadingForCollision) {
            currentForce += AdjustAltitude(hoverHeight);
        }

        // face midway between current velocity vector and target: like anticipating a turn
        Vector3 facingDirection = (rb.velocity + targetDirection) / 2;
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
