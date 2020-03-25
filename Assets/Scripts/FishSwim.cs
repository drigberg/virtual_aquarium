using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSwim : MonoBehaviour
{
    public Transform[] bones;
    public float frequency;
    public GameObject fish;
    public float maxRotateDegreesPerBone;
    private float startTime;

    void Start() {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float elapsed = Time.time - startTime;
        float targetRotation = Mathf.Sin(elapsed * frequency * Mathf.PI) * maxRotateDegreesPerBone;
        for (int i = 0; i < bones.Length; i++) {
            // Vector3 targetVector = Quaternion.AngleAxis(targetRotation,fish.forward);
            // FishMovementUtils.TurnToFace(rb, transform, targetDirection);
            float currentRotation = bones[i].rotation.z;
            bones[i].Rotate(0, 0, targetRotation - currentRotation);
        }
    }
}
