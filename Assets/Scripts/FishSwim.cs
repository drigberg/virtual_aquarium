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
    private float elapsed;
    private float targetRotation = 0.0f;
    private float lastRotation;
    private float rotationDiff;

    void Start() {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        elapsed = Time.time - startTime;
        lastRotation = targetRotation;
        targetRotation = Mathf.Sin(elapsed * frequency * Mathf.PI) * maxRotateDegreesPerBone;
        rotationDiff = targetRotation - lastRotation;
        Debug.Log("Target: " + targetRotation + ", Diff: " + rotationDiff);
    }

    void FixedUpdate() {
        for (int i = 0; i < bones.Length; i++) {
            bones[i].Rotate(0, 0, rotationDiff);
        }
    }
}
