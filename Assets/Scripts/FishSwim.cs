using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSwim : MonoBehaviour
{
    public Transform[] bones;
    public float frequency;
    public float maxRotateDegreesPerBone;
    private float startTime;
    private float elapsed;
    private float targetRotation = 0.0f;
    private float lastRotation;
    private float rotationDiff;
    private float offset;

    // Start is called before the first frame update
    void Start() {
        startTime = Time.time;
        offset = Random.Range(Mathf.PI * -1, Mathf.PI);
    }

    // Update is called once per frame
    void Update()
    {
        elapsed = Time.time - startTime;
        lastRotation = targetRotation;
        targetRotation = Mathf.Sin(elapsed * frequency * Mathf.PI + offset) * maxRotateDegreesPerBone;
    }

    void FixedUpdate() {
        for (int i = 0; i < bones.Length; i++) {
            bones[i].localEulerAngles = new Vector3(0, 0, targetRotation);
        }
    }
}
