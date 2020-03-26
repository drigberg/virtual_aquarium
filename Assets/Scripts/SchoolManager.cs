using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolManager : MonoBehaviour
{
    const int threadGroupSize = 1024;
    public ComputeShader compute;

    [Header ("Number and type of fish")]
    public SchoolingFish fishPrefab;
    public int numFish = 5;

    [HideInInspector]
    public SchoolingFish[] allFish;

    [Header ("Spawning")]
    public Transform spawnCenter;
    public float schoolStartRadius = 5.0f;
    public float minDistanceBetweenSpawnPoints = 1.0f;

    [Header ("Fish shared settings: movement")]
    public Transform target;
    public float speed = 600;
    public float maxSpeed = 600;
    public float perceptionRadius = 2.5f;
    public float avoidanceRadius = 1;
    public float alignWeight = 1;
    public float cohesionWeight = 1;
    public float separateWeight = 1;
    public float targetWeight = 1;

    [Header ("Fish shared settings: collisions")]
    public LayerMask obstacleMask;
    public float boundsRadius = .27f;
    public float avoidCollisionWeight = 10;
    public float collisionAvoidDst = 5;

    private SchoolSharedSettings fishSharedSettings;



    // Start is called before the first frame update
    void Start()
    {
        fishSharedSettings = ScriptableObject.CreateInstance<SchoolSharedSettings>();
        fishSharedSettings.Initialize(
            target,
            speed,
            maxSpeed,
            perceptionRadius,
            avoidanceRadius,
            alignWeight,
            cohesionWeight,
            separateWeight,
            targetWeight,
            obstacleMask,
            boundsRadius,
            avoidCollisionWeight,
            collisionAvoidDst);
        allFish = new SchoolingFish[numFish];
        for (int i = 0; i < numFish; i++) {
            Vector3 pos = validStartPosition();
            SchoolingFish fish = Instantiate(fishPrefab);
            fish.transform.position = pos;
            fish.transform.forward = Random.insideUnitSphere;
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
