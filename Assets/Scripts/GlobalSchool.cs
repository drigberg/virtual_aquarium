using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSchool : MonoBehaviour
{

    public GameObject fishPrefab;
    public Transform startPosition;
    public float schoolStartRadius;
    public float avoidanceThreshold;

    public int numFish;
    public static GameObject[] allFish;
    public static Vector3 goalPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        allFish = new GameObject[numFish];
        for (int i = 0; i < numFish; i++) {
            Vector3 pos = new Vector3(3, 3, 3);
            allFish[i] = (GameObject) Instantiate(fishPrefab, pos, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
