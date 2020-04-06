using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StingraySwim : MonoBehaviour
{
    public BottomFeederPatrol bottomFeeder;
    public Transform[] bones;
    public float frequency;
    public float maxRotateDegreesPerBone;
    public float maxForce;
    private float speedScale;
    private float localTime = 0.0f;
    private float targetRotation = 0.0f;
    private float offset;

    // Start is called before the first frame update
    void Start() {
        offset = Random.Range(Mathf.PI * -1, Mathf.PI);
    }

    // Update is called once per frame
    void Update()
    {
        speedScale = bottomFeeder.currentForce.magnitude / maxForce;
        localTime += Time.deltaTime * frequency * speedScale;
        targetRotation = Mathf.Sin(localTime + Mathf.PI + offset) * scaledMaxRotation;
    }

    void FixedUpdate() {
        for (int i = 0; i < bones.Length; i++) {
            bones[i].localEulerAngles = new Vector3(targetRotation, 0, 0);
        }
    }
}
