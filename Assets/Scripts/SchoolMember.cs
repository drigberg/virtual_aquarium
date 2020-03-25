using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolMember : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetDirection = GlobalSchool.goalPos - transform.position;
        FishMovementUtils.MoveTowardsTarget(rb, targetDirection, speed);
        // avoid other fish here
        FishMovementUtils.TurnToFace(rb, transform, targetDirection);  
    }
}
