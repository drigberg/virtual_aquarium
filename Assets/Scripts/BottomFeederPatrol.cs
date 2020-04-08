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
    [HideInInspector]
    public Vector3 currentForce;
    private Vector3 currentTarget;
    private float restTimer = 0.0f;

    [Header ("Collisions")]
    public LayerMask obstacleMask;
    public float collisionAvoidDst;


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
        if (restTimer > 0.0f) {
            IterateRestTimer();
            return;
        }

        if (HaveReachedDestination(currentTarget)) {
            currentTarget = GetNextTarget();
            if (ShouldRest()) {
                restTimer = GetRestInterval();
                return;
            }
        }

        if (CollisionInDirection(currentTarget - transform.position)) {
            for (int i = 0; i < 5; i++) {
                currentTarget = GetNextTarget();
                if (!CollisionInDirection(currentTarget - transform.position)) {
                    break;
                }
            }
        }
    }

    void FixedUpdate() {
        FishMovementUtils.RotateUpright(rb, transform);
        if (restTimer <= 0.0f) {
            currentForce = MoveTowardsTarget();
        } else {
            currentForce = AdjustAltitude(restingHoverHeight);
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

    Vector3 MoveTowardsTarget() {
        // move towards target along XZ plane
        Vector3 targetDirection = currentTarget - transform.position;
        Vector3 force = FishMovementUtils.MoveTowardsTarget(rb, targetDirection, speed);
        force += AdjustAltitude(hoverHeight);

        // face midway between current velocity vector and target: like anticipating a turn
        Vector3 facingDirection = (rb.velocity + targetDirection) / 2;
        FishMovementUtils.TurnToFace(rb, transform, facingDirection);
        return force;
    }

    bool CollisionInDirection (Vector3 direction) {
        if (Physics.Raycast (transform.position, direction, collisionAvoidDst, obstacleMask)) {
            return true;
        } else { }
        return false;
    }
}
