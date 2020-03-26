using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolingFish : MonoBehaviour
{
    public SchoolSharedSettings settings;
    private Rigidbody rb;

    // updated by manager
    [HideInInspector]
    public Vector3 avgFlockHeading;
    [HideInInspector]
    public Vector3 avgAvoidanceHeading;
    [HideInInspector]
    public Vector3 centreOfFlockmates;
    [HideInInspector]
    public int numPerceivedFlockmates;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(SchoolSharedSettings sharedSettings) {
        this.settings = sharedSettings;
    }

    public void UpdateFish()
    {
        Vector3 targetVector = (settings.target.position - transform.position) * settings.speed * settings.targetWeight;
        Vector3 collisionAvoidanceVector = Vector3.zero;
        Vector3 alignmentVector = Vector3.zero;
        Vector3 cohesionVector = Vector3.zero;
        Vector3 separationVector = Vector3.zero;

        if (IsHeadingForCollision()) {
            Vector3 collisionAvoidDir = ObstacleRays ();
            collisionAvoidanceVector = collisionAvoidDir * settings.speed * settings.avoidCollisionWeight;
        }

        if (numPerceivedFlockmates != 0) {
            centreOfFlockmates /= numPerceivedFlockmates;
            Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - transform.position);
            alignmentVector = avgFlockHeading * settings.speed * settings.alignWeight;
            cohesionVector = offsetToFlockmatesCentre * settings.speed * settings.cohesionWeight;
            separationVector = avgAvoidanceHeading * settings.speed * settings.separateWeight;
        }

        Vector3 sumVector = targetVector + collisionAvoidanceVector + alignmentVector + cohesionVector + separationVector;
        if (sumVector.magnitude > settings.maxSpeed) {
            sumVector = sumVector * (settings.maxSpeed / sumVector.magnitude);
        }

        FishMovementUtils.MoveTowardsTarget(rb, sumVector, settings.speed);
        FishMovementUtils.TurnToFace(rb, transform, rb.velocity);
    }

    bool IsHeadingForCollision () {
        RaycastHit hit;
        if (Physics.SphereCast (transform.position, settings.boundsRadius, transform.forward, out hit, settings.collisionAvoidDst, settings.obstacleMask)) {
            return true;
        } else { }
        return false;
    }

    Vector3 ObstacleRays () {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++) {
            Vector3 dir = transform.TransformDirection (rayDirections[i]);
            Ray ray = new Ray (transform.position, dir);
            if (!Physics.SphereCast (ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask)) {
                return dir;
            }
        }

        return transform.forward;
    }
}
