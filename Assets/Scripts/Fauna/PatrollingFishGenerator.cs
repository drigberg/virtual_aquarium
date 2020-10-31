using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingFishGenerator : MonoBehaviour
{
    [Header ("General")]
    public Patrol fishPrefab;
    public Transform minXYZ;
    public Transform maxXYZ;

    public int numFish = 5;

    private Patrol[] allFish;


    [Header ("Spawning")]
    public Transform spawnCenter;
    public float schoolStartRadius = 5.0f;
    public float minDistanceBetweenSpawnPoints = 1.0f;


    // Start is called before the first frame update
    void Start()
    {
        allFish = new Patrol[numFish];
        for (int i = 0; i < numFish; i++) {
            Vector3 pos = validStartPosition();
            Patrol fish = Instantiate(fishPrefab);
            fish.transform.position = pos;
            fish.transform.forward = Random.insideUnitSphere;
            fish.minXYZ = minXYZ;
            fish.maxXYZ = maxXYZ;
            allFish[i] = fish;
        }
    }

    Vector3 validStartPosition() {
        bool foundValidPoint = false;
        int attempts = 0;
        Vector3 pos = new Vector3(0, 0, 0);
        while (!foundValidPoint) {
            attempts++;
            Vector3 startVector = Random.insideUnitSphere * schoolStartRadius;
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
}
