using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolSharedSettings : ScriptableObject {
    // Settings
    public Transform target;
    public float speed;
    public float maxSpeed;
    public float perceptionRadius;
    public float avoidanceRadius;
    public float alignWeight;
    public float cohesionWeight;
    public float separateWeight;
    public float targetWeight;
    public LayerMask obstacleMask;
    public float boundsRadius;
    public float avoidCollisionWeight;
    public float collisionAvoidDst;

    public void Initialize(Transform target, float speed, float maxSpeed, float perceptionRadius, float avoidanceRadius, float alignWeight, float cohesionWeight, float separateWeight, float targetWeight, LayerMask obstacleMask, float boundsRadius, float avoidCollisionWeight, float collisionAvoidDst) {
        this.target = target;
        this.speed = speed;
        this.maxSpeed = maxSpeed;
        this.perceptionRadius = perceptionRadius;
        this.avoidanceRadius = avoidanceRadius;
        this.alignWeight = alignWeight;
        this.cohesionWeight = cohesionWeight;
        this.separateWeight = separateWeight;
        this.targetWeight = targetWeight;
        this.obstacleMask = obstacleMask;
        this.boundsRadius = boundsRadius;
        this.avoidCollisionWeight = avoidCollisionWeight;
        this.collisionAvoidDst = collisionAvoidDst;
    }
}