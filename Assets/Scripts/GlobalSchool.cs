using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSchool : MonoBehaviour
{

    public GameObject fishPrefab;
    public static GameObject[] allFish;
    public int numFish;
    public Transform spawnCenter;
    public float schoolStartRadius;
    public float minDistanceBetweenSpawnPoints;
    public float avoidanceThreshold;

    public static Vector3 goalPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        allFish = new GameObject[numFish];
        for (int i = 0; i < numFish; i++) {
            Vector3 pos = validStartPosition();
            allFish[i] = (GameObject) Instantiate(fishPrefab, pos, Quaternion.identity);
        }
    }

    Vector3 validStartPosition() {
        bool foundValidPoint = false;
        int attempts = 0;
        Vector3 pos = new Vector3(0, 0, 0);
        while (!foundValidPoint) {
            attempts++;
            Vector3 startVector = randomPointInSphere(schoolStartRadius);
            pos = new Vector3(spawnCenter.position.x + startVector.x, spawnCenter.position.y + startVector.y, spawnCenter.position.z + startVector.z);
            for (int i = 0; i < allFish.Length; i++) {
                if (allFish[i]) {
                    float distance = Vector3.Distance(allFish[i].transform.position, pos);
                    if (distance < minDistanceBetweenSpawnPoints) {
                        foundValidPoint = true;
                    }
                }
            }

            if (attempts > 20) {
                // just give up eventually to avoid infinite loop
                foundValidPoint = true;
            }
        }
        return pos;
    }

    Vector3 randomPointInSphere(float radius) {
        // generate random point in cube until point falls within sphere
        bool foundValidPoint = false;
        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;
        while (!foundValidPoint) {
            x = Random.Range(-1.0f, 1.0f);
            y = Random.Range(-1.0f, 1.0f);
            z = Random.Range(-1.0f, 1.0f);
            if (Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2) <= 1) {
                foundValidPoint = true;
            }
        }
        return new Vector3(x * radius, y * radius, z * radius);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
