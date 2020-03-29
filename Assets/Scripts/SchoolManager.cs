using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolManager : MonoBehaviour
{
    const int threadGroupSize = 1024;
    public ComputeShader compute;

    [Header ("General")]
    public SchoolingFish fishPrefab;
    public int numFish = 5;
    public SchoolSharedSettings fishSharedSettings;
    public Transform target;

    [HideInInspector]
    public SchoolingFish[] allFish;

    [Header ("Spawning")]
    public Transform spawnCenter;
    public float schoolStartRadius = 5.0f;
    public float minDistanceBetweenSpawnPoints = 1.0f;


    // Start is called before the first frame update
    void Start()
    {
        Vector3 forward = Random.insideUnitSphere;
        fishSharedSettings.target = target;
        allFish = new SchoolingFish[numFish];
        for (int i = 0; i < numFish; i++) {
            Vector3 pos = validStartPosition();
            SchoolingFish fish = Instantiate(fishPrefab);
            fish.transform.position = pos;
            fish.transform.forward = forward;
            fish.Initialize(fishSharedSettings);
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

    // Update is called once per frame
    void Update()
    {
        if (allFish != null) {
            var fishData = new FishData[allFish.Length];
            for (int i = 0; i < allFish.Length; i++) {
                fishData[i].position = allFish[i].transform.position;
                fishData[i].direction = allFish[i].transform.forward;
            }

            var fishesBuffer = new ComputeBuffer (allFish.Length, FishData.Size);
            fishesBuffer.SetData (fishData);

            compute.SetBuffer (0, "fishes", fishesBuffer);
            compute.SetInt ("numFish", allFish.Length);
            compute.SetFloat ("viewRadius", fishSharedSettings.perceptionRadius);
            compute.SetFloat ("avoidRadius", fishSharedSettings.avoidanceRadius);

            int threadGroups = Mathf.CeilToInt (allFish.Length / (float) threadGroupSize);
            compute.Dispatch (0, threadGroups, 1, 1);

            fishesBuffer.GetData (fishData);

            for (int i = 0; i < allFish.Length; i++) {
                allFish[i].avgFlockHeading = fishData[i].flockHeading;
                allFish[i].centreOfFlockmates = fishData[i].flockCentre;
                allFish[i].avgAvoidanceHeading = fishData[i].avoidanceHeading;
                allFish[i].numPerceivedFlockmates = fishData[i].numFlockmates;
                allFish[i].UpdateFish ();
            }

            fishesBuffer.Release ();
        }
    }

    public struct FishData {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;
        public int numFlockmates;

        public static int Size {
            get {
                return sizeof (float) * 3 * 5 + sizeof (int);
            }
        }
    }
}
