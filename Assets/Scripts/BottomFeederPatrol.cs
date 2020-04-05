using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomFeederPatrol : MonoBehaviour
{
    public Rigidbody rb;
    public Transform minXZ;
    public Transform maxXZ;
    public float targetRadius = 3.0f;
    public float restProbability = 0.5f;
    public float maxRestTimeSeconds = 15.0f;
    public float speed = 5.0f;

    private Vector3 currentTarget;
    private float restTimer = 0.0f;

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
    }

    void FixedUpdate() {
        if (restTimer <= 0.0f) {
            MoveTowardsTarget();
        }
    }

    bool HaveReachedDestination(Vector3 destination) {
        return Vector3.Distance(destination, transform.position) < targetRadius;
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
        return Random.Range(0, maxRestTimeSeconds);
    }

    void IterateRestTimer() {
        restTimer -= Time.deltaTime;
    }

    bool ShouldRest() {
        return Random.Range(0, 1) < restProbability;
    }

    void MoveTowardsTarget() {
        Vector3 targetDirection = currentTarget - transform.position;
        FishMovementUtils.MoveTowardsTarget(rb, targetDirection, speed);
        // face midway between current velocity vector and target: like anticipating a turn
        Vector3 facingDirection = (rb.velocity + targetDirection) / 2;
        FishMovementUtils.TurnToFace(rb, transform, facingDirection);
    }
}
